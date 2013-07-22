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
	unsigned debouncerValue : 1;
}PRESENCE_ELEM_t;

struct
{
	OPERATION_HEADER_t header;
	PRESENCE_READ_RESPONSE_MESSAGE_t response;
}presenceResponse;

PRESENCE_ELEM_t presen_elems[MAX_PRESENCE_DEVICES];
uint8_t num_of_presen_elems;

SYS_Timer_t sensorReadTimer;

static void sensorReadTimerHandler(SYS_Timer_t *timer);
uint8_t findPresenceElem(uint16_t deviceAddress);

void presenceModule_Init(void)
{
	PRESENCE_CONFIG_t* configPtr;
	uint8_t* portPtr;
	uint8_t mask;
	
	num_of_presen_elems = runningConfiguration.raw[runningConfiguration.topConfiguration.dinamicIndex.configModule_Presence];		//First byte is number of configs
	configPtr			= &runningConfiguration.raw[runningConfiguration.topConfiguration.dinamicIndex.configModule_Presence + 1];	//At second byte the list start
	
	if(num_of_presen_elems > MAX_PRESENCE_DEVICES)
	{
		//TODO: SEND ERROR (MAX NUMBER OF PRESEN DEVICES EXCEEDED)
		num_of_presen_elems = MAX_PRESENCE_DEVICES;
	}
	
	for(uint8_t i = 0; i<num_of_presen_elems; i++)
	{
		portPtr = PORT_FROM_PINADDRESS(configPtr->pinAddress);
		mask = MASK_FROM_PINADDRESS(configPtr->pinAddress);
		
		HAL_GPIO_PORT_in(portPtr, mask);
		
		presen_elems[i].config  = configPtr;
		presen_elems[i].portPtr = portPtr;
		presen_elems[i].config  = mask;
		
		configPtr++;
	}
	
	//Set responses opCodes
	presenceResponse.header.opCode = PresenceReadResponse;
	
	//Configure Timer
	sensorReadTimer.interval = 1000;
	sensorReadTimer.mode = SYS_TIMER_PERIODIC_MODE;
	sensorReadTimer.handler = sensorReadTimerHandler;
	SYS_TimerStart(&sensorReadTimer);
}

void presenceModule_NotificationInd(uint8_t sender, OPERATION_HEADER_t* notification)
{
	
}

void presenceRead_Handler(OPERATION_HEADER_t* operation_header)
{
	if(operation_header->opCode == PresenceRead)
	{
		PRESENCE_READ_MESSAGE_t* msg = (PRESENCE_READ_MESSAGE_t*)(operation_header + 1);
		uint8_t elemIndex = findPresenceElem(msg->deviceID);
		if(elemIndex != 0xFF)
		{
			presenceResponse.header.destinationAddress = operation_header->sourceAddress;
			presenceResponse.header.sourceAddress = runningConfiguration.topConfiguration.networkConfig.deviceAddress;
			presenceResponse.response.deviceID = msg->deviceID;
			presenceResponse.response.detected = presen_elems[elemIndex].lastValue;
			
			OM_ProccessResponseOperation(&presenceResponse.header);
		}
		else
		{
			//TODO: SEND ERROR (UNKNOWN SENSOR ADDRESS) USING ERROR RESPONSE INSTEAD
			
			presenceResponse.header.destinationAddress = operation_header->sourceAddress;
			presenceResponse.header.sourceAddress = runningConfiguration.topConfiguration.networkConfig.deviceAddress;
			presenceResponse.response.deviceID = msg->deviceID;
			presenceResponse.response.detected = 0xFF;
			
			OM_ProccessResponseOperation(&presenceResponse.header);
		}
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

static void sensorReadTimerHandler(SYS_Timer_t *timer)
{
	for(uint8_t i = 0; i < num_of_presen_elems; i++)
	{
		PRESENCE_ELEM_t* currentElem = &presen_elems[i];
		
		CONFIG_MODULE_ELEM_HEADER_t* operationsInfoPtr = &currentElem->config->operationsInfo;
		
		uint8_t val = HAL_GPIO_PORT_read(currentElem->portPtr, currentElem->mask) == 0 ? 0 : 1;
		
		//Debouncer
		if(val == currentElem->debouncerValue)//Valid value
		{
			if(~currentElem->lastValue &  val) //RISING EDGE
			{
				//TODO: USE OPERATION MANAGER!
				OPERATION_HEADER_t* operationPtr = &runningConfiguration.raw[operationsInfoPtr->pointerOperationList];
				for(int c = 0; c < operationsInfoPtr->numberOfOperations; c++)
				{
					OM_ProccessInternalOperation(operationPtr, false);
					operationPtr++;
				}
				
				
				//Send to coordinator
				presenceResponse.header.sourceAddress = runningConfiguration.topConfiguration.networkConfig.deviceAddress;
				presenceResponse.header.destinationAddress = COORDINATOR_ADDRESS;
				presenceResponse.response.deviceID = currentElem->config->deviceID;
				presenceResponse.response.detected = val;
				
				OM_ProccessResponseOperation(&presenceResponse.header);
			}
			
			currentElem->lastValue = val;
		}			
	}
	
	(void)timer;
}