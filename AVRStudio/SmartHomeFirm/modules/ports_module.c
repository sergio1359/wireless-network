/*
* native_module.c
*
* Created: 12/05/2013 13:00:25
*  Author: Victor
*/
#include "modules.h"
#include "globals.h"
#include "portMonitor.h"

#include "sysTimer.h"

#define TIME_KEEPER_BUFFER_SIZE 16
#define TIME_KEEPER_BUFFER_SIZE_MASK TIME_KEEPER_BUFFER_SIZE-1

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
	DIGITAL_READ_RESPONSE_MESSAGE_t response;
}digitalResponse;

struct
{
	OPERATION_HEADER_t header;
	ANALOG_READ_RESPONSE_MESSAGE_t response;
}analogResponse;

SYS_Timer_t timeKeeperTimer;

CREATE_CIRCULARBUFFER(TIME_KEEPER_ELEM_t, TIME_KEEPER_BUFFER_SIZE)	timeKeeper_Buffer;

bool validatePortAction(PORT_CONFIG_t config, uint8_t mask, bool digital, bool read);
bool proccessDigitalPortAction(uint8_t dir, uint8_t mask, bool read, uint8_t value, uint8_t seconds, uint16_t sourceAddress);
static void timeKeeperTimerHandler(SYS_Timer_t *timer);

void portModule_Init()
{
	//TODO: Read and set configuration
	PORT_CONFIG_t* configPtr = &runningConfiguration.topConfiguration.portConfig_PA;
	
	uint8_t* portPtr;
	
	for(uint8_t port=0; port<NUM_PORTS; port++)
	{
		portPtr = PORT_FROM_PINADDRESS(port<<3);
		
		HAL_GPIO_PORT_out(portPtr,configPtr->maskIO);
		HAL_GPIO_PORT_in(portPtr,~(configPtr->maskIO));
		
		HAL_GPIO_PORT_set(portPtr,(configPtr->defaultValuesD & ~(configPtr->maskIO)));
		HAL_GPIO_PORT_clr(portPtr,(~(configPtr->defaultValuesD) & ~(configPtr->maskIO)));
		
		configPtr+= sizeof(PORT_CONFIG_t);			
	}
	
	digitalResponse.header.opCode = DigitalReadResponse;
	analogResponse.header.opCode = AnalogReadResponse;
	
	timeKeeperTimer.interval = 200; // 5 per seconds
	timeKeeperTimer.mode = SYS_TIMER_PERIODIC_MODE;
	timeKeeperTimer.handler = timeKeeperTimerHandler;
	SYS_TimerStart(&timeKeeperTimer);
}

void portModule_NotificationInd(uint8_t sender, OPERATION_HEADER_t* notification)
{
	
}

void portModule_TaskHandler(void)
{
	//TODO: Check the programmed off pins
}

void digitalPort_Handler(OPERATION_HEADER_t* operation_header)
{
	if(operation_header->opCode == DigitalWrite)
	{
		DIGITAL_WRITE_MESSAGE_t* msg = (DIGITAL_WRITE_MESSAGE_t*)(operation_header + 1);
		if(!proccessDigitalPortAction(msg->dir, msg->mask, false,  msg->value, msg->seconds, operation_header->sourceAddress))
		{
			//TODO:SEND ERROR MESSAGE (INVALID OPERATION)
		}
	}else if(operation_header->opCode == DigitalSwitch)
	{
		DIGITAL_SWITCH_MESSAGE_t* msg = (DIGITAL_SWITCH_MESSAGE_t*)(operation_header + 1);
		if(!proccessDigitalPortAction(msg->dir, msg->mask, false,  ~lastValuesD[msg->dir], msg->seconds, operation_header->sourceAddress))
		{
			//TODO:SEND ERROR MESSAGE (INVALID OPERATION)
		}
	}else if(operation_header->opCode == DigitalRead)
	{
		DIGITAL_READ_MESSAGE_t* msg = (DIGITAL_READ_MESSAGE_t*)(operation_header + 1);
		if(!proccessDigitalPortAction(msg->dir, 0, true,  0, 0, operation_header->sourceAddress))
		{
			//TODO:SEND ERROR MESSAGE (INVALID OPERATION)
		}
	}else if(operation_header->opCode == DigitalReadResponse)
	{
		//TODO: NOTIFICATION (check)
		modules_Notify(PortModule, operation_header);
	}
}

void analogPort_Handler(OPERATION_HEADER_t* operation_header)
{
	if(operation_header->opCode == AnalogWrite)
	{
		ANALOG_WRITE_MESSAGE_t* msg = (ANALOG_WRITE_MESSAGE_t*)(operation_header + 1);
		//TODO: To consider the time param
		/*if(!proccessDigitalPortAction(msg->dir, msg->mask, false,  msg->value, msg->seconds,  operation_header->sourceAddress))
		{
			//TODO:SEND ERROR MESSAGE
		}*/
	}else if(operation_header->opCode == AnalogRead)
	{
		ANALOG_READ_MESSAGE_t* msg = (ANALOG_READ_MESSAGE_t*)(operation_header + 1);
		//TODO: To consider the time param
		/*if(!proccessDigitalPortAction(msg->dir, msg->mask, false, ~lastValuesD[msg->dir], msg->seconds, operation_header->sourceAddress))
		{
			//TODO:SEND ERROR MESSAGE
		}*/
	}else if(operation_header->opCode == AnalogReadResponse)
	{
		//TODO: NOTIFICATION
	}
}	

_Bool validatePortAction(PORT_CONFIG_t config, uint8_t mask, _Bool digital, _Bool read)
{
	_Bool result;
		
	if(digital)
		result = (config.maskAD & mask) == mask;
	else
		result = ((~config.maskAD) & mask) == mask;
		
	if(read)
		result &= ((~config.maskIO) & mask) == mask;
	else
		result &= (config.maskIO & mask) == mask;
		
	return result;
}

_Bool proccessDigitalPortAction(uint8_t portAddress, uint8_t mask, _Bool read, uint8_t value, uint8_t seconds, uint16_t sourceAddress)
{
	uint8_t* portPtr = PORT_FROM_PINADDRESS(portAddress<<3);
	PORT_CONFIG_t* configPtr = (&runningConfiguration.topConfiguration.portConfig_PA + portAddress);
	
	if(!validatePortAction(*configPtr, mask, true, read))
		return false;

	if(read)
	{
		digitalResponse.header.sourceAddress = runningConfiguration.topConfiguration.networkConfig.deviceAddress;
		digitalResponse.header.destinationAddress = sourceAddress;
		digitalResponse.response.dir = portAddress;
		digitalResponse.response.value = lastValuesD[portAddress];
		//TODO: Send a DIGITAL_READ_RESPONSE_MESSAGE_t (check)
		Radio_AddMessageByCopy(&digitalResponse.header);
	}
	else
	{
		HAL_GPIO_PORT_set(portPtr, value & mask);
		HAL_GPIO_PORT_clr(portPtr, ~value & mask);
		
		if(seconds > 0)
		{
			timeKeeper_Buffer.buffer[timeKeeper_Buffer.end].counter = (uint16_t)seconds*5;
			timeKeeper_Buffer.buffer[timeKeeper_Buffer.end].portPtr = portPtr;
			timeKeeper_Buffer.buffer[timeKeeper_Buffer.end].mask = mask;
			timeKeeper_Buffer.buffer[timeKeeper_Buffer.end].value = ~value;
			timeKeeper_Buffer.end++;
		}
	}
	
	return true;
}
TIME_KEEPER_ELEM_t* currentElem;
static void timeKeeperTimerHandler(SYS_Timer_t *timer)
{
	unsigned int index = timeKeeper_Buffer.start;
	
	while(index != timeKeeper_Buffer.end)
	{
		currentElem = &timeKeeper_Buffer.buffer[index];
		if(currentElem->counter > 0)
		{
			currentElem->counter--;
		}else if(currentElem->portPtr != 0) //Is a valid elem
		{
			HAL_GPIO_PORT_set(currentElem->portPtr, currentElem->value & currentElem->mask);
			HAL_GPIO_PORT_clr(currentElem->portPtr, ~currentElem->value & currentElem->mask);
			currentElem->portPtr = 0;
		}
		
		if(currentElem->portPtr == 0 && timeKeeper_Buffer.start == index)
		{
			timeKeeper_Buffer.start = (index+1) & TIME_KEEPER_BUFFER_SIZE_MASK;
		}
		
		index = (index+1) & TIME_KEEPER_BUFFER_SIZE_MASK;
	}
	
	(void)timer;
}	