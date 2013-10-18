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

#define OVR_COUNTER TIME_FRAGMENT * 129

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
static inline void changeElemValue(uint8_t elemIndex, uint8_t newValue);
static inline void checkCompElems(uint8_t firstElemIndex, uint16_t* compPtr);

void dimmerModule_Init()
{
	DIMMER_CONFIG_t* configPtr;
	uint8_t* portPtr;
	uint8_t mask;
	uint8_t zeroCrossPin;
	
	if(!validConfiguration)
		return;
	
	num_of_dimmer_elems = runningConfiguration.raw[runningConfiguration.topConfiguration.dinamicIndex.configModule_Dimmable];			//First byte is number of configs
	zeroCrossPin		= runningConfiguration.raw[runningConfiguration.topConfiguration.dinamicIndex.configModule_Dimmable + 1];		//Second byte is the Zero Cross pin
	configPtr		   = &runningConfiguration.raw[runningConfiguration.topConfiguration.dinamicIndex.configModule_Dimmable + 2];		//At third byte the list start
	
	if(num_of_dimmer_elems > MAX_DIMMER_DEVICES)
	{
		//TODO: SEND ERROR (MAX NUMBER OF DEVICES EXCEEDED)
		num_of_dimmer_elems = MAX_DIMMER_DEVICES;
	}
	
	zeroCrossPin = BASE_PinAddressToINT(zeroCrossPin);
	if(zeroCrossPin == 0xFF)
	{
		//TODO: SEND ERROR (INVALID ZEROCROSS PINPORT ADDRESS)
		return;
	}
	
	for(uint8_t i = 0; i<num_of_dimmer_elems; i++)
	{
		portPtr = PORT_FROM_PINADDRESS(configPtr->pinAddress);
		mask = MASK_FROM_PINADDRESS(configPtr->pinAddress);

		HAL_GPIO_PORT_out(portPtr, mask);
		HAL_GPIO_PORT_clr(portPtr, mask); // Off at startup
		
		dimmer_elems[i].config = configPtr;
		dimmer_elems[i].portPtr = portPtr;
		dimmer_elems[i].mask = mask;
		dimmer_elems[i].currentValue = 0;
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
		
		//Initialize TIMER5 in normal mode
		OCR5A = OVR_COUNTER;		// For elem 0 & 3
		OCR5B = OVR_COUNTER;		// For elem 1 & 4
		OCR5C = OVR_COUNTER;		// For elem 2 & 5
		TCCR5B |= (1 << CS51);		// Prescaler 8
		TIMSK5 |= (1 << TOIE5);		// Enable overflow interrupt
		
		//Initialize zero crossing interrupt
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

void changeElemValue(uint8_t elemIndex, uint8_t newValue)
{
	DIMMER_ELEM_t* elem = &dimmer_elems[elemIndex];
	
	elem->previousValue = elem->currentValue;
	
	if(newValue < 5)
	{
		//Full OFF
		HAL_GPIO_PORT_clr(elem->portPtr, elem->mask);
		elem->currentValue = 0;
		elem->timerValue = 0; //Never changes
	}else if(newValue >= 128)
	{
		//Full ON
		HAL_GPIO_PORT_set(elem->portPtr, elem->mask);
		elem->currentValue = 128;
		elem->timerValue = 0; //Never changes
	}else
	{
		//Dimming
		elem->currentValue = newValue;
		elem->timerValue = TIME_FRAGMENT * (128 - elem->currentValue);
	
		if((elemIndex == 0 || elemIndex == 3) && (TIMSK5 & (1 << OCIE5A))== 0)
		{
			OCR5A = elem->timerValue;
			TIMSK5 |= (1 << OCIE5A);			// Enable TC5A interrupt		
		}
		else if((elemIndex == 1 || elemIndex == 4) && (TIMSK5 & (1 << OCIE5B))== 0)
		{
			OCR5B = elem->timerValue;
			TIMSK5 |= (1 << OCIE5B);            // Enable TC5B interrupt
		}
		else if((elemIndex == 2 || elemIndex == 5) && (TIMSK5 & (1 << OCIE5C))== 0)
		{
			OCR5C = elem->timerValue;
			TIMSK5 |= (1 << OCIE5C);            // Enable TC5C interrupt
		}
	}	
}

_Bool proccessDimmerPortAction(uint16_t deviceAddress, _Bool read, uint8_t newValue, uint8_t seconds, uint16_t sourceAddress)
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
		changeElemValue(configIndex, newValue);
		
		if(seconds > 0)
		{
			currentElem->timerCounter = (uint16_t)seconds*5; //Enable timer
		}
		else
		{
			currentElem->timerCounter = 0; //Disable timer
		}
		
		//Notify to the coordinator
		sendDimmerResponse(COORDINATOR_ADDRESS, deviceAddress, newValue);
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
		}else if(currentElem->timerCounter == 1) //Time to process
		{
			//Disable timer
			currentElem->timerCounter = 0;
			
			//Set previous value
			changeElemValue(i, currentElem->previousValue);
		}
	}
	
	(void)timer;
}

void checkCompElems(uint8_t firstElemIndex, uint16_t* compPtr)
{
	DIMMER_ELEM_t* firstElem = &dimmer_elems[firstElemIndex];
	DIMMER_ELEM_t* secondElem = num_of_dimmer_elems >= (firstElemIndex + 3) ? &dimmer_elems[firstElemIndex + 3] : 0;
	_Bool updateSecondElem = (secondElem && secondElem->timerValue == (*compPtr));
	
	if(firstElem->timerValue == (*compPtr))
	{
		//Send a pulse
		HAL_GPIO_PORT_set(firstElem->portPtr, firstElem->mask);
		
		if(secondElem)
		(*compPtr) = secondElem->timerValue;
	}
	
	if(updateSecondElem)
	{
		//Send a pulse
		HAL_GPIO_PORT_set(secondElem->portPtr, secondElem->mask);
		
		(*compPtr) = firstElem->timerValue;
	}
	
	_delay_us(10);
	
	HAL_GPIO_PORT_clr(firstElem->portPtr, firstElem->mask);
	
	if(updateSecondElem)
	{
		HAL_GPIO_PORT_clr(secondElem->portPtr, secondElem->mask);
	}
}

void dimmerZeroCrossInterrupt()
{
	//Restart counter
	TCNT5 = 0;
}

ISR(TIMER5_COMPA_vect)
{
	checkCompElems(0, &OCR5A);
}

ISR(TIMER5_COMPB_vect)
{
	checkCompElems(1, &OCR5B);
}

ISR(TIMER5_COMPC_vect)
{
	checkCompElems(2, &OCR5C);
}

ISR(TIMER1_OVF_vect)
{
	//If timer overflows, then zero-crossing detection is not working as spected reload the timer with the overflow value.
	//Then only zero-crossing interrupt can reset the counter again. Another option to avoid overhead is disable the timer itself, and enable it
	//in the setValue function
	
	TCNT5 = OVR_COUNTER;
}	