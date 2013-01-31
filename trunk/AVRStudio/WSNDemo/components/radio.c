/*
 * radio.c
 *
 * Created: 28/01/2013 14:28:08
 *  Author: Victor
 */ 

#include "radio.h"

void RF_Send_Event(EVENT_HEADER_t* eventHeader)
{
	//For testing purposes just send throw UART port
	HAL_UartPrint("RF Send Event Request: ");
	HAL_UartWriteByte(eventHeader->destinationAddress>>8);
	HAL_UartWriteByte(eventHeader->destinationAddress);
	HAL_UartWriteByte(eventHeader->operation);
	HAL_UartPrint("\n");
}