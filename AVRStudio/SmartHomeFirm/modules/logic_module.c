/*
* logic_module.c
*
* Created: 12/05/2013 13:00:25
*  Author: Victor
*/
#include "modulesManager.h"
#include "globals.h"
#include "portMonitor.h"

#include "sysTimer.h"


#define MAX_DEVICES 32

typedef struct
{
	uint16_t counter;
	uint8_t* portPtr;
	uint8_t mask;
	uint8_t value;
}TIME_KEEPER_ELEM_t;

struct
{
	OPERATION_HEADER_t header;
	LOGIC_READ_RESPONSE_MESSAGE_t response;
}logicResponse;

LOGIC_CONFIG_t* logic_configs[MAX_DEVICES];
uint8_t num_of_logic_configs;

SYS_Timer_t timeKeeperTimer;

TIME_KEEPER_ELEM_t timeKeeper_Buffer[MAX_DEVICES];

_Bool proccessDigitalPortAction(uint16_t address, _Bool read, uint8_t value, uint8_t seconds, uint16_t sourceAddress);
static void timeKeeperTimerHandler(SYS_Timer_t *timer);

void logicModule_Init()
{
	uint8_t* configPtr = &runningConfiguration.raw[runningConfiguration.topConfiguration.dinamicIndex.configModule_Logic];
	uint8_t* portPtr;
	uint8_t mask;
	num_of_logic_configs = *configPtr;
	configPtr++;
	
	if(num_of_logic_configs > MAX_DEVICES)
	{
		//TODO: SEND ERROR (MAX NUMBER OF DEVICES EXCEEDED)
		num_of_logic_configs = MAX_DEVICES;
	}
	
	for(uint8_t i = 0; i<num_of_logic_configs; i++)
	{
		logic_configs[i] = (LOGIC_CONFIG_t*)configPtr;
		configPtr += sizeof(LOGIC_CONFIG_t);
		
		portPtr = PORT_FROM_PINADDRESS(logic_configs[i]->pinPort);
		
		mask = MASK_FROM_PINADDRESS(logic_configs[i]->pinPort);

		HAL_GPIO_PORT_out(portPtr, mask);
		HAL_GPIO_PORT_in(portPtr,~(mask));
		
		HAL_GPIO_PORT_set(portPtr,(logic_configs[i]->configBits->defaultValue & ~(mask)));
		HAL_GPIO_PORT_clr(portPtr,(~(logic_configs[i]->configBits->defaultValue) & ~(mask)));
	}
	
	logicResponse.header.opCode = LogicReadResponse;
	
	timeKeeperTimer.interval = 200; // 5 per seconds
	timeKeeperTimer.mode = SYS_TIMER_PERIODIC_MODE;
	timeKeeperTimer.handler = timeKeeperTimerHandler;
	SYS_TimerStart(&timeKeeperTimer);
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
		if(!proccessDigitalPortAction(msg->address, 0, true,  0, 0, operation_header->sourceAddress))
		{
			//TODO:SEND ERROR MESSAGE (INVALID OPERATION)
		}
	}else if(operation_header->opCode == LogicReadResponse)
	{
		//TODO: NOTIFICATION (check)
		modules_Notify(LogicModule, operation_header);
	}
}

uint8_t findLogicConfig(uint16_t deviceAddress)
{
	for(uint8_t i = 0; i < num_of_logic_configs; i++)
	{
		if(logic_configs[i]->deviceID == deviceAddress)
		return i;
	}
	
	return 0xFF;
}

_Bool proccessDigitalPortAction(uint16_t deviceAddress, _Bool read, uint8_t value, uint8_t seconds, uint16_t sourceAddress)
{
	uint8_t configIndex = findLogicConfig(deviceAddress);
	LOGIC_CONFIG_t* configPtr;
	uint8_t* portPtr;
	uint8_t  mask;
	
	if(configIndex == 0xFF) //UNKNOWN DEVICE ADDRESS
		return false;
	
	configPtr = logic_configs[configIndex];
	
	if((read && ((~configPtr->configBits->maskIO) & mask) != mask) || (!read && ((configPtr->configBits->maskIO & mask) != mask) )) //INVALID OPERATION
		return false;

	portPtr = PORT_FROM_PINADDRESS(configPtr->pinPort);
	mask = MASK_FROM_PINADDRESS(configPtr->pinPort);
	
	if(read)
	{
		logicResponse.header.sourceAddress = runningConfiguration.topConfiguration.networkConfig.deviceAddress;
		logicResponse.header.destinationAddress = sourceAddress;
		logicResponse.response.address = deviceAddress;
		logicResponse.response.value = HAL_GPIO_PORT_read(portPtr, configPtr) == 0 ? 0 : 1;
		
		OM_ProccessResponseOperation(&logicResponse.header);
	}
	else
	{
		if(value == 0xFF)//Switch action
		{
			value = ~HAL_GPIO_PORT_read(portPtr, configPtr);
		}else
		{
			HAL_GPIO_PORT_set(portPtr, value & mask);
			HAL_GPIO_PORT_clr(portPtr, ~value & mask);
		}
		
		if(seconds > 0)
		{
			timeKeeper_Buffer[configIndex].counter = (uint16_t)seconds*5;
			timeKeeper_Buffer[configIndex].portPtr = portPtr;
			timeKeeper_Buffer[configIndex].mask = mask;
			timeKeeper_Buffer[configIndex].value = ~value;
		}
	}
	
	return true;
}

static void timeKeeperTimerHandler(SYS_Timer_t *timer)
{
	for(uint8_t i = 0; i < num_of_logic_configs; i++)
	{
		TIME_KEEPER_ELEM_t* currentElem = &timeKeeper_Buffer.buffer[i];
		
		if(currentElem->counter > 0)
		{
			currentElem->counter--;
		}else if(currentElem->portPtr != 0) //Is a valid elem
		{
			HAL_GPIO_PORT_set(currentElem->portPtr, currentElem->value & currentElem->mask);
			HAL_GPIO_PORT_clr(currentElem->portPtr, ~currentElem->value & currentElem->mask);
			currentElem->portPtr = 0;
		}
	}
	
	(void)timer;
}