/*
* logic_module.c
*
* Created: 12/05/2013 13:00:25
*  Author: Victor
*/
#include "modulesManager.h"
#include "globals.h"
#include "DIGITAL.h"

#include "sysTimer.h"

#define MAX_LOGIC_DEVICES 32

typedef struct
{
	LOGIC_CONFIG_t* config;
	uint8_t* portPtr;
	uint8_t mask;
	uint16_t timerCounter;
	unsigned lastValue		: 1;	//LSB
	unsigned debouncerValue : 1;
}LOGIC_ELEM_t;

struct
{
	OPERATION_HEADER_t header;
	LOGIC_READ_RESPONSE_MESSAGE_t response;
}logicResponse;

LOGIC_ELEM_t logic_elems[MAX_LOGIC_DEVICES];
uint8_t num_of_logic_elems;

SYS_Timer_t logicTimer;
uint8_t timerDivider;

_Bool proccessDigitalPortAction(uint16_t address, _Bool read, uint8_t value, uint8_t seconds, uint16_t sourceAddress);
static void logicTimerHandler(SYS_Timer_t *timer);
uint8_t findLogicElem(uint16_t deviceAddress);

void logicModule_Init()
{
	LOGIC_CONFIG_t* configPtr;
	uint8_t* portPtr;
	uint8_t mask;
	
	num_of_logic_elems = runningConfiguration.raw[runningConfiguration.topConfiguration.dinamicIndex.configModule_Logic];			//First byte is number of configs
	configPtr		   = &runningConfiguration.raw[runningConfiguration.topConfiguration.dinamicIndex.configModule_Logic + 1];		//At second byte the list start
	
	if(num_of_logic_elems > MAX_LOGIC_DEVICES)
	{
		//TODO: SEND ERROR (MAX NUMBER OF DEVICES EXCEEDED)
		num_of_logic_elems = MAX_LOGIC_DEVICES;
	}
	
	for(uint8_t i = 0; i<num_of_logic_elems; i++)
	{
		portPtr = PORT_FROM_PINADDRESS(configPtr->pinAddress);
		mask = MASK_FROM_PINADDRESS(configPtr->pinAddress);

		if(configPtr->configBits.maskIO) //Output
		{
			HAL_GPIO_PORT_out(portPtr, mask);
			
			if(configPtr->configBits.defaultValue)
			HAL_GPIO_PORT_set(portPtr, mask);
			else
			HAL_GPIO_PORT_clr(portPtr,mask);
			
		}else
		{
			HAL_GPIO_PORT_in(portPtr, mask);
		}
		
		logic_elems[i].config = configPtr;
		logic_elems[i].portPtr = portPtr;
		logic_elems[i].mask = mask;
		logic_elems[i].timerCounter = 0;
		configPtr++;
	}
	
	//Set responses opCodes
	logicResponse.header.opCode = LogicReadResponse;
	
	//Configure Timer
	timerDivider = 0;
	
	logicTimer.interval = 50; // 20 times per second
	logicTimer.mode = SYS_TIMER_PERIODIC_MODE;
	logicTimer.handler = logicTimerHandler;
	SYS_TimerStart(&logicTimer);
}

void logicModule_DataConf(NWK_DataReq_t *req)
{
	
}

void logicModule_NotificationInd(uint8_t sender, OPERATION_HEADER_t* notification)
{
	
}

void logic_Handler(OPERATION_HEADER_t* operation_header)
{
	if(operation_header->opCode == LogicWrite)
	{
		LOGIC_WRITE_MESSAGE_t* msg = (LOGIC_WRITE_MESSAGE_t*)(operation_header + 1);
		if(!proccessDigitalPortAction(msg->address, false,  msg->value, msg->seconds, operation_header->sourceAddress))
		{
			//TODO:SEND ERROR MESSAGE (INVALID OPERATION)
		}
	}else if(operation_header->opCode == LogicSwitch)
	{
		LOGIC_SWITCH_MESSAGE_t* msg = (LOGIC_SWITCH_MESSAGE_t*)(operation_header + 1);
		if(!proccessDigitalPortAction(msg->address, false,  0xFF, msg->seconds, operation_header->sourceAddress))
		{
			//TODO:SEND ERROR MESSAGE (INVALID OPERATION)
		}
	}else if(operation_header->opCode == LogicRead)
	{
		LOGIC_READ_MESSAGE_t* msg = (LOGIC_READ_MESSAGE_t*)(operation_header + 1);
		if(!proccessDigitalPortAction(msg->address, true,  0, 0, operation_header->sourceAddress))
		{
			//TODO:SEND ERROR MESSAGE (INVALID OPERATION)
		}
	}else if(operation_header->opCode == LogicReadResponse)
	{
		//TODO: NOTIFICATION (check)
		//MODULES_Notify(LogicModule, operation_header);
	}
}

uint8_t findLogicElem(uint16_t deviceAddress)
{
	for(uint8_t i = 0; i < num_of_logic_elems; i++)
	{
		if(logic_elems[i].config->deviceID == deviceAddress)
		return i;
	}
	
	return 0xFF;//Address not found
}

_Bool proccessDigitalPortAction(uint16_t deviceAddress, _Bool read, uint8_t value, uint8_t seconds, uint16_t sourceAddress)
{
	uint8_t configIndex = findLogicElem(deviceAddress);

	if(configIndex == 0xFF) //UNKNOWN DEVICE ADDRESS
	return false;
	
	LOGIC_ELEM_t* currentElem = &logic_elems[configIndex];
	
	if(read)
	{
		logicResponse.header.sourceAddress = runningConfiguration.topConfiguration.networkConfig.deviceAddress;
		logicResponse.header.destinationAddress = sourceAddress;
		logicResponse.response.address = deviceAddress;
		logicResponse.response.value = currentElem->lastValue;
		
		OM_ProccessResponseOperation(&logicResponse.header);
	}
	else
	{
		if( currentElem->config->configBits.maskIO == 0 ) //If is Input -> INVALID OPERATION
		return false;
		
		if(value == 0xFF)//Switch action
		{
			HAL_GPIO_PORT_toggle(currentElem->portPtr, currentElem->mask);
		}else
		{
			if(value)
			HAL_GPIO_PORT_set(currentElem->portPtr, currentElem->mask);
			else
			HAL_GPIO_PORT_clr(currentElem->portPtr, currentElem->mask);
		}

		if(seconds > 0)
			currentElem->timerCounter = (uint16_t)seconds*5; //Enable timer
		else
			currentElem->timerCounter = 0; //Disable timer
	}
	
	return true;
}

static void logicTimerHandler(SYS_Timer_t *timer)
{
	for(uint8_t i = 0; i < num_of_logic_elems; i++)
	{
		LOGIC_ELEM_t* currentElem = &logic_elems[i];
		CONFIG_MODULE_ELEM_HEADER_t* operationsInfoPtr = &currentElem->config->operationsInfo;
		LOGIC_BITS_CONFIG_t* configBitsPtr = &currentElem->config->configBits;

		_Bool changeOcurred = 0;
		uint8_t val = HAL_GPIO_PORT_read(currentElem->portPtr, currentElem->mask) == 0 ? 0 : 1;
				
		//Debouncer
		if(val == currentElem->debouncerValue)//Valid value
		{
			//Check changes for inputs with changeType distinct of NO_EDGE
			if (configBitsPtr->maskIO == 0 && configBitsPtr->changeType != NO_EDGE )
			{
				//Check for valid changes
				switch(configBitsPtr->changeType)
				{
					case FALLING_EDGE:	changeOcurred = (  currentElem->lastValue & ~val ); break;
					case RISIN_EDGE:	changeOcurred = ( ~currentElem->lastValue &  val ); break;
					case BOTH_EDGE:		changeOcurred = (  currentElem->lastValue != val ); break;
				}
					
				if(changeOcurred)
				{
					//TODO: USE OPERATION MANAGER!
					OPERATION_HEADER_t* operationPtr = &runningConfiguration.raw[operationsInfoPtr->pointerOperationList];
					for(int c = 0; c < operationsInfoPtr->numberOfOperations; c++)
					{
						OM_ProccessInternalOperation(operationPtr);
						operationPtr++;
					}
						
						
					//Send to coordinator
					logicResponse.header.sourceAddress = runningConfiguration.topConfiguration.networkConfig.deviceAddress;
					logicResponse.header.destinationAddress = COORDINATOR_ADDRESS;
					logicResponse.response.address = currentElem->config->deviceID;
					logicResponse.response.value = val;
						
					OM_ProccessResponseOperation(&logicResponse.header);
				}
			}
					
			currentElem->lastValue = val;					
		}
				
		currentElem->debouncerValue = val;
		
		//If is OUTPUT Check TimerToggle
		if( configBitsPtr->maskIO == 1 && timerDivider++ == 3) //4 * 50ms = 200 ms
		{
			timerDivider = 0;
			
			// Timer check (Outputs)
			if(currentElem->timerCounter > 1)
			{
				currentElem->timerCounter--;
			}else if(currentElem->timerCounter == 1) //Time to proccess
			{
				//Invert current value
				HAL_GPIO_PORT_toggle(currentElem->portPtr, currentElem->mask);
				
				currentElem->timerCounter = 0; //Disable timer
			}
		}
	}
	
	(void)timer;
}