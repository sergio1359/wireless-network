/*
 * radio.c
 *
 * Created: 28/01/2013 14:28:08
 *  Author: Victor
 */ 

#include "radio.h"

uint16_t eoeoe;
void RF_Send_Message(OPERATION_HEADER_t* operationHeader)
{
	//For testing purposes just send throw UART port
	HAL_UartPrint("RF Send Message Request: ");
	numWrite(operationHeader->destinationAddress);
	HAL_UartPrint("\t");
	numWrite(operationHeader->opCode);
	HAL_UartPrint("\t");
	
	uint8_t length = getCommandArgsLength(&operationHeader->opCode);
	for (uint8_t i = 0; i < length; i++)
	{
		eoeoe = &operationHeader;
		numWrite(*((uint8_t*)operationHeader + 3 + i));
	}
	
	HAL_UartPrint("\r\n");
}