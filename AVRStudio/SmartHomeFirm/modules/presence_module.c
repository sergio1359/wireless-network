/*
* presence_module.c
*
* Created: 22/07/2013 23:59:33
*  Author: Victor
*/

#include "globals.h"
#include "modulesManager.h"
#include "DIGITAL.h"

#include "sysTimer.h"

#define MAX_PRESENCE_DEVICES 4

typedef struct
{
	PRESENCE_CONFIG_t* config;
	uint8_t* portPtr;
	uint8_t mask;
	unsigned lastValue		: 1;	//LSB
	unsigned timeCounter	: 7;	//MSB
}PRESENCE_ELEM_t;

static struct
{
	OPERATION_HEADER_t header;
	PRESENCE_READ_RESPONSE_MESSAGE_t response;
}presenceResponse;

static PRESENCE_ELEM_t presen_elems[MAX_PRESENCE_DEVICES];
static uint8_t num_of_presen_elems;

static SYS_Timer_t presenceReadTimer;

static void presenceReadTimerHandler(SYS_Timer_t *timer);
static uint8_t findPresenceElem(uint16_t deviceAddress);

void presenceModule_Init(void)
{
	PRESENCE_CONFIG_t* configPtr;
	uint8_t* portPtr;
	uint8_t mask;
	
	if(!validConfiguration)
	return;
	
	num_of_presen_elems = runningConfiguration.raw[runningConfiguration.topConfiguration.dinamicIndex.configModule_Presence];		//First byte is number of configs
	configPtr			= &runningConfiguration.raw[runningConfiguration.topConfiguration.dinamicIndex.configModule_Presence + 1];	//At second byte the list start
	
	if(num_of_presen_elems > MAX_PRESENCE_DEVICES)
	{
		//TODO: SEND ERROR (MAX NUMBER OF PRESEN DEVICES EXCEEDED)
		num_of_presen_elems = MAX_PRESENCE_DEVICES;
	}
	
	for(uint8_t i = 0; i < num_of_presen_elems; i++)
	{
		portPtr = PORT_FROM_PINADDRESS(configPtr->pinAddress);
		mask = MASK_FROM_PINADDRESS(configPtr->pinAddress);
		
		HAL_GPIO_PORT_in(portPtr, mask);
		
		presen_elems[i].config  = configPtr;
		presen_elems[i].portPtr = portPtr;
		presen_elems[i].mask  = mask;
		presen_elems[i].timeCounter  = 0;
		
		configPtr++;
	}
	
	//Set responses opCodes
	presenceResponse.header.opCode = PresenceReadResponse;
	
	if(num_of_presen_elems > 0)
	{
		//Configure Timer
		presenceReadTimer.interval = 500;
		presenceReadTimer.mode = SYS_TIMER_PERIODIC_MODE;
		presenceReadTimer.handler = presenceReadTimerHandler;
		SYS_TimerStart(&presenceReadTimer);
	}	
}

void presenceModule_NotificationInd(uint8_t sender, OPERATION_HEADER_t* notification)
{
	
}

void presenceModule_DataConf(OPERATION_DataConf_t *req)
{
	
}

void presenceRead_Handler(OPERATION_HEADER_t* operation_header)
{
	if(operation_header->opCode == PresenceRead)
	{
		PRESENCE_READ_MESSAGE_t* msg = (PRESENCE_READ_MESSAGE_t*)(operation_header + 1);
		
		uint8_t elemIndex = findPresenceElem(msg->deviceID);
		
		presenceResponse.header.destinationAddress = operation_header->sourceAddress;
		presenceResponse.header.sourceAddress = runningConfiguration.topConfiguration.networkConfig.deviceAddress;
		presenceResponse.response.deviceID = msg->deviceID;
		presenceResponse.response.detected = elemIndex == 0xFF ? 0xFF : presen_elems[elemIndex].lastValue;
		
		OM_ProccessResponseOperation(&presenceResponse.header);
	}else if(operation_header->opCode == PresenceReadResponse)
	{
		//TODO: NOTIFICATION
	}
}

uint8_t findPresenceElem(uint16_t deviceAddress)
{
	for(uint8_t i = 0; i < num_of_presen_elems; i++)
	{
		if(presen_elems[i].config->deviceID == deviceAddress)
		return i;
	}
	
	return 0xFF;
}

static void presenceReadTimerHandler(SYS_Timer_t *timer)
{
	for(uint8_t i = 0; i < num_of_presen_elems; i++)
	{
		PRESENCE_ELEM_t* currentElem = &presen_elems[i];
		
		CONFIG_MODULE_ELEM_HEADER_t* operationsInfoPtr = &currentElem->config->operationsInfo;
		
		uint8_t val = HAL_GPIO_PORT_read(currentElem->portPtr, currentElem->mask) == 0 ? 0 : 1;
		
		if(val)
		{
			if(currentElem->lastValue == 0) //RISING EDGE
			{
				currentElem->timeCounter = 0;
			
				//TODO: USE OPERATION MANAGER!
				OPERATION_HEADER_t* operationPtr = &runningConfiguration.raw[operationsInfoPtr->pointerOperationList];
				for(int c = 0; c < operationsInfoPtr->numberOfOperations; c++)
				{
					OM_ProccessInternalOperation(operationPtr);
					operationPtr++;
				}
			
				//Send to coordinator
				presenceResponse.header.sourceAddress = runningConfiguration.topConfiguration.networkConfig.deviceAddress;
				presenceResponse.header.destinationAddress = COORDINATOR_ADDRESS;
				presenceResponse.response.deviceID = currentElem->config->deviceID;
				presenceResponse.response.detected = val;
			
				OM_ProccessResponseOperation(&presenceResponse.header);
			}else
			{
				if(currentElem->timeCounter < 0x7F)
					currentElem->timeCounter++;
			}
		}		
		
		currentElem->lastValue = val;
	}
	
	(void)timer;
}