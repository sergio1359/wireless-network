/*
 * radio.c
 *
 * Created: 28/01/2013 14:28:08
 *  Author: Victor
 */ 

#include "radio.h"

uint16_t eoeoe;
void RF_Send_Event(EVENT_HEADER_t* eventHeader)
{
	//For testing purposes just send throw UART port
	HAL_UartPrint("RF Send Event Request: ");
	numWrite(eventHeader->destinationAddress);
	HAL_UartPrint("\t");
	numWrite(eventHeader->operation);
	HAL_UartPrint("\t");
	
	uint8_t length = getCommandArgsLenght(&eventHeader->operation);
	for (uint8_t i = 0; i < length; i++)
	{
		eoeoe = &eventHeader;
		numWrite(*((uint8_t*)eventHeader + 3 + i));
	}
	
	HAL_UartPrint("\r\n");
}