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

		HAL_GPIO_PORT_out(portPtr, mask);
		HAL_GPIO_PORT_in(portPtr,~(mask));
		
		HAL_GPIO_PORT_set(portPtr,(configPtr->configBits.defaultValue & ~(mask)));
		HAL_GPIO_PORT_clr(portPtr,(~(configPtr->configBits.defaultValue) & ~(mask)));
	
		logic_elems[i].config = configPtr;
		logic_elems[i].portPtr = portPtr;
		logic_elems[i].mask = mask;
		logic_elems[i].timerCounter = 0;
		configPtr++;
	}
	
	//Set responses opCodes
	logicResponse.header.opCode = LogicReadResponse;
	
	//Configure Timer
	logicTimer.interval = 50; // 5 times per second
	logicTimer.mode = SYS_TIMER_PERIODIC_MODE;
	logicTimer.handler = logicTimerHandler;
	SYS_TimerStart(&logicTimer);
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
		MODULES_Notify(LogicModule, operation_header);
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
	LOGIC_CONFIG_t* configPtr;
	uint8_t* portPtr;
	uint8_t  mask;
	
	if(configIndex == 0xFF) //UNKNOWN DEVICE ADDRESS
		return false;
	
	configPtr = logic_elems[configIndex].config;
	portPtr = logic_elems[configIndex].portPtr;
	mask = logic_elems[configIndex].mask;
	
	if((read && ((~configPtr->configBits.maskIO) & mask) != mask) || (!read && ((configPtr->configBits.maskIO & mask) != mask) )) //INVALID OPERATION
		return false;
	
	if(read)
	{
		logicResponse.header.sourceAddress = runningConfiguration.topConfiguration.networkConfig.deviceAddress;
		logicResponse.header.destinationAddress = sourceAddress;
		logicResponse.response.address = deviceAddress;
		logicResponse.response.value = logic_elems[configIndex].lastValue;
		
		OM_ProccessResponseOperation(&logicResponse.header);
	}
	else
	{
		if(value == 0xFF)//Switch action
		{
			value = ~HAL_GPIO_PORT_read(portPtr, mask);
		}else
		{
			HAL_GPIO_PORT_set(portPtr, value & mask);
			HAL_GPIO_PORT_clr(portPtr, ~value & mask);
		}
		
		if(seconds > 0)
			logic_elems->timerCounter = (uint16_t)seconds*5; //Enable timer
		else
			logic_elems->timerCounter = 0; //Disable timer
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
		
		if( ~configBitsPtr->maskIO )	//Read if input
		{
			if ( (operationsInfoPtr->numberOfOperations > 0) &&		//Operations to send
				 (configBitsPtr->changeType != NO_EDGE) )		//AND ChangeType != NONE
			{
				_Bool changeOcurred = 0;
				uint8_t val = HAL_GPIO_PORT_read(currentElem->portPtr, currentElem->mask) == 0 ? 0 : 1;
				
				//Debouncer
				if(val == currentElem->debouncerValue)//Valid value
				{	
					//Check for valid changes
					switch(configBitsPtr->changeType)
					{
						case FALLING_EDGE:	changeOcurred = (  currentElem->lastValue & ~val ); break;
						case RISIN_EDGE:	changeOcurred = ( ~currentElem->lastValue &  val ); break;
						case BOTH_EDGE:		changeOcurred = (  currentElem->lastValue != val ); break;
					}
					
					currentElem->lastValue = val;
					
					if(changeOcurred)
					{
						//TODO: USE OPERATION MANAGER!
						OPERATION_HEADER_t* operationPtr = &runningConfiguration.raw[operationsInfoPtr->pointerOperationList];
						for(int c = 0; c < operationsInfoPtr->numberOfOperations; c++)
						{
							OM_ProccessInternalOperation(operationPtr, false);
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
				
				currentElem->debouncerValue = val;
			}
		}
		else// Timer check (Outputs)
		{
			if(currentElem->timerCounter > 1)
			{
				currentElem->timerCounter--;
			}else if(currentElem->timerCounter == 1) //Time to proccess
			{
				//Invert current value
				HAL_GPIO_PORT_set(currentElem->portPtr, ~currentElem->lastValue & currentElem->mask);
				HAL_GPIO_PORT_clr(currentElem->portPtr, currentElem->lastValue & currentElem->mask);
			
				currentElem->timerCounter = 0; //Disable timer
			}	
		}
	}
	
	(void)timer;
}