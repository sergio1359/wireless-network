/*
 * operationsManager.c
 *
 * Created: 11/05/2013 22:28:47
 *  Author: Victor
 */ 

#include "operationsManager.h"
#include "modules.h"

void OM_ProccessOperation(OPERATION_HEADER_t* operation_header, _Bool byCopy)
{
	/* TESTING REGION */
	//For testing purposes just send throw UART port
	HAL_UartPrint("PROCESSIG OPERATION >> TO:");
	numWriteHEX(operation_header->destinationAddress);
	HAL_UartPrint("\t CODE:");
	numWriteHEX(operation_header->opCode);
	HAL_UartPrint("\t ARGS:");
	
	uint8_t length = getCommandArgsLength(&operation_header->opCode);
	
	for (uint8_t i = 0; i < length; i++)
	{
		numWriteHEX(*((uint8_t*)operation_header + sizeof(OPERATION_HEADER_t) + i));
		HAL_UartWriteByte(' ');
	}
	
	HAL_UartPrint("\r\n");
	if(operation_header->opCode == EXTENSION_OPCODE)
	return;
	/* END OF TESTING REGION */
	
	
	if(operation_header->destinationAddress == 0) //MINE (INTERNAL)
	{
		handleCommand(operation_header);
	}else
	{
		if(byCopy)
			Radio_AddMessageByCopy(operation_header);
		else
			Radio_AddMessageByReference(operation_header);
	}
}