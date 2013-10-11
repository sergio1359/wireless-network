/*
* dimmer_module.c
*
* Created: 11/10/2013 16:33:06
*  Author: Victor
*/

#include "modulesManager.h"
#include "globals.h"
#include "DIGITAL.h"
#include "INTERRUPT.h"

#include "sysTimer.h"

/*---------TIMING DEFINITIONS------*/
#define TIMER_INTERVAL_50Hz 75
#define TIMER_INTERVAL_60Hz 65
#define TIMER_PRESCALER 8

#define TIME_FRAGMENT ((F_CPU / 1000000ul) / TIMER_PRESCALER) * TIMER_INTERVAL_50Hz

#define MIN_COUNTER TIME_FRAGMENT * 5
#define MAX_COUNTER TIME_FRAGMENT * 128

/*---------MODULE DEFINITIONS-------*/
#define MAX_DIMMER_DEVICES 6

typedef struct
{
	DIMMER_CONFIG_t* config;
	uint8_t* portPtr;
	uint8_t mask;
	uint16_t timerCounter;
	uint8_t previousValue;
	uint8_t currentValue;//0-128
	uint16_t timerValue;
}DIMMER_ELEM_t;

static struct
{
	OPERATION_HEADER_t header;
	DIMMER_READ_RESPONSE_MESSAGE_t response;
}dimmerResponse;

static _Bool waitingForResponseConf;

static DIMMER_ELEM_t dimmer_elems[MAX_DIMMER_DEVICES];
static uint8_t num_of_dimmer_elems;

static SYS_Timer_t dimmerTimer;

static void dimmerZeroCrossInterrupt();
static void dimmerTimerHandler(SYS_Timer_t *timer);
static _Bool proccessDimmerPortAction(uint16_t address, _Bool read, uint8_t value, uint8_t seconds, uint16_t sourceAddress);
static uint8_t findDimmerElem(uint16_t deviceAddress);
static inline void sendDimmerResponse(uint16_t destinationAddress, uint16_t deviceAddress, uint8_t value);

void dimmerModule_Init()
{
	DIMMER_CONFIG_t* configPtr;
	uint8_t* portPtr;
	uint8_t mask;
	uint8_t zeroCrossPin;
	
	num_of_dimmer_elems = runningConfiguration.raw[runningConfiguration.topConfiguration.dinamicIndex.configModule_Dimmable];			//First byte is number of configs
	zeroCrossPin		= runningConfiguration.raw[runningConfiguration.topConfiguration.dinamicIndex.configModule_Dimmable + 1];		//Second byte is the Zero Cross pin
	configPtr		   = &runningConfiguration.raw[runningConfiguration.topConfiguration.dinamicIndex.configModule_Dimmable + 2];		//At thrid byte the list start
	
	if(num_of_dimmer_elems > MAX_DIMMER_DEVICES)
	{
		//TODO: SEND ERROR (MAX NUMBER OF DEVICES EXCEEDED)
		num_of_dimmer_elems = MAX_DIMMER_DEVICES;
	}
	
	for(uint8_t i = 0; i<num_of_dimmer_elems; i++)
	{
		portPtr = PORT_FROM_PINADDRESS(configPtr->pinAddress);
		mask = MASK_FROM_PINADDRESS(configPtr->pinAddress);

		HAL_GPIO_PORT_out(portPtr, mask);
		HAL_GPIO_PORT_clr(portPtr, mask);
		
		dimmer_elems[i].config = configPtr;
		dimmer_elems[i].portPtr = portPtr;
		dimmer_elems[i].mask = mask;
		dimmer_elems[i].currentValue = 130; //Off at startup
		configPtr++;
	}
	
	//Set responses opCodes
	dimmerResponse.header.opCode = DimmerReadResponse;
	
	waitingForResponseConf = false;
	
	if(num_of_dimmer_elems > 0)
	{
		//Configure Timers
		dimmerTimer.interval = 200; // 5 times per second
		dimmerTimer.mode = SYS_TIMER_PERIODIC_MODE;
		dimmerTimer.handler = dimmerTimerHandler;
		SYS_TimerStart(&dimmerTimer);
		
		//Initialize TIMER5
		OCR5A = 0xFFFF;
		//Nortmal mode
		TCCR5B |= (1 << CS11);              // Prescaler 8
		TIMSK5 |= (1 << OCIE5A);            // Enable TC5 interrupt
		
		//TODO: Initialize zero crossing interrupt
		INTERRUPT_Attach(zeroCrossPin, dimmerZeroCrossInterrupt, RISING);
	}
}

void dimmerModule_NotificationInd(uint8_t sender, OPERATION_HEADER_t* notification)
{
	
}

void dimmer_Handler(OPERATION_HEADER_t* operation_header)
{
	if(operation_header->opCode == DimmerWrite)
	{
		DIMMER_WRITE_MESSAGE_t* msg = (DIMMER_WRITE_MESSAGE_t*)(operation_header + 1);
		if(!proccessDimmerPortAction(msg->address, false,  msg->value, msg->seconds, operation_header->sourceAddress))
		{
			//TODO:SEND ERROR MESSAGE (INVALID OPERATION)
		}
	}else if(operation_header->opCode == DimmerRead)
	{
		DIMMER_READ_MESSAGE_t* msg = (DIMMER_READ_MESSAGE_t*)(operation_header + 1);
		if(!proccessDimmerPortAction(msg->address, true,  0, 0, operation_header->sourceAddress))
		{
			//TODO:SEND ERROR MESSAGE (INVALID OPERATION)
		}
	}else if(operation_header->opCode == DimmerReadResponse)
	{
		//TODO: NOTIFICATION (check)
		//MODULES_Notify(DimmerModule, operation_header);
	}
}

void dimmerRead_DataConf(OPERATION_DataConf_t *req)
{
	waitingForResponseConf = false;
}

uint8_t findDimmerElem(uint16_t deviceAddress)
{
	for(uint8_t i = 0; i < num_of_dimmer_elems; i++)
	{
		if(dimmer_elems[i].config->deviceID == deviceAddress)
		return i;
	}
	
	return 0xFF;//Address not found
}

void sendDimmerResponse(uint16_t destinationAddress, uint16_t deviceAddress, uint8_t value)
{
	if(waitingForResponseConf)
	{
		//TODO:SEND ERROR MESSAGE (DIMMER RESPONSE OVERBOOKING)
	}
	
	waitingForResponseConf = true;
	
	dimmerResponse.header.sourceAddress = runningConfiguration.topConfiguration.networkConfig.deviceAddress;
	dimmerResponse.header.destinationAddress = destinationAddress;
	dimmerResponse.response.address = deviceAddress;
	dimmerResponse.response.value = value;
	
	OM_ProccessResponseOperation(&dimmerResponse.header);
}

_Bool proccessDimmerPortAction(uint16_t deviceAddress, _Bool read, uint8_t value, uint8_t seconds, uint16_t sourceAddress)
{
	uint8_t configIndex = findDimmerElem(deviceAddress);

	if(configIndex == 0xFF) //UNKNOWN DEVICE ADDRESS
	{
		sendDimmerResponse(sourceAddress, deviceAddress, 0xFF);
		return false;
	}
	
	DIMMER_ELEM_t* currentElem = &dimmer_elems[configIndex];
	
	if(read)
	{
		sendDimmerResponse(sourceAddress, deviceAddress, currentElem->currentValue);
	}
	else
	{
		//TODO: Maybe its better to use a conversion, to avoid inverted logic.
		uint8_t correctedValue = value;
		
		if(seconds > 0)
		{
			currentElem->previousValue = currentElem->currentValue;
			currentElem->timerCounter = (uint16_t)seconds*5; //Enable timer
		}
		else
		{
			currentElem->timerCounter = 0; //Disable timer
		}
		
		currentElem->currentValue = correctedValue;
		
		
		
		//Notify to the coordinator
		sendDimmerResponse(COORDINATOR_ADDRESS, deviceAddress, correctedValue);
	}
}

static void dimmerTimerHandler(SYS_Timer_t *timer)
{
	for(uint8_t i = 0; i < num_of_dimmer_elems; i++)
	{
		DIMMER_ELEM_t* currentElem = &dimmer_elems[i];
		
		// Timer check
		if(currentElem->timerCounter > 1)
		{
			currentElem->timerCounter--;
		}else if(currentElem->timerCounter == 1) //Time to proccess
		{
			//Set previous value
			currentElem->currentValue = currentElem->previousValue;
			
			currentElem->timerCounter = 0; //Disable timer
		}
	}
	
	(void)timer;
}

static uint8_t updatedElements;

void dimmerZeroCrossInterrupt()
{
	updatedElements = 0;
	
	//Restart counter
	TCNT5 = 0;
}

ISR(TIMER5_COMPA_vect)
{
	uint8_t minimumValue = 130 * TIME_FRAGMENT;
	
	//Find the next elem with the nearest objective value
	for(uint8_t i = 0; i < num_of_dimmer_elems; i++)
	{
		DIMMER_ELEM_t* currentElem = &dimmer_elems[i];
		
		if(currentElem->timerValue == OCR5A)
		{
			updatedElements++;
			//Send a pulse
			HAL_GPIO_PORT_set(currentElem->portPtr, currentElem->mask);
			_delay_us(10);
			HAL_GPIO_PORT_clr(currentElem->portPtr, currentElem->mask);
		}else if(currentElem->timerValue > OCR5A)
		{
			//Pending elems
			
		}
		
		if(minimumValue > currentElem->timerValue)
			minimumValue = currentElem->timerValue;
	}
	
	//If all updated. Set the minimun value for the next round
	if(updatedElements == num_of_dimmer_elems)
	{
		OCR5A = minimumValue;
	}
}