/*
 * operationsManager.c
 *
 * Created: 11/05/2013 22:28:47
 *  Author: Victor
 */ 

#include "operationsManager.h"
#include "modulesManager.h"
#include "globals.h"

void OM_ProccessInternalOperation(OPERATION_HEADER_t* operation_header, _Bool byCopy)
{
	/* TESTING REGION */
	if(!IS_COORDINATOR)
	{
		//For testing purposes just send throw UART port
		HAL_UartPrint("PROCESSING OPERATION >> FROM:");
		HAL_UartWriteNumberHEX(operation_header->sourceAddress);
		HAL_UartPrint("\t TO:");
		HAL_UartWriteNumberHEX(operation_header->destinationAddress);
		HAL_UartPrint("\t CODE:");
		HAL_UartWriteNumberHEX(operation_header->opCode);
		HAL_UartPrint("\t ARGS:");
	
		uint8_t length = getCommandArgsLength(&operation_header->opCode);
	
		for (uint8_t i = 0; i < length; i++)
		{
			HAL_UartWriteNumberHEX(*((uint8_t*)operation_header + sizeof(OPERATION_HEADER_t) + i));
			HAL_UartWriteByte(' ');
		}
	
		HAL_UartPrint("\r\n");
		if(operation_header->opCode == EXTENSION_OPCODE)
		return;
		/* END OF TESTING REGION */
	}
	
	if(operation_header->destinationAddress == 0) //MINE (INTERNAL)
	{
		handleCommand(operation_header);
	}else
	{
		//TODO: ELIMINAR ESTO, PARA EVITAR UNA POSIBLE VOMITONA DE RAFA :)
		if(IS_COORDINATOR)
			operation_header->sourceAddress = runningConfiguration.topConfiguration.networkConfig.deviceAddress;
		
		if(byCopy)
			Radio_AddMessageByCopy(operation_header);
		else
			Radio_AddMessageByReference(operation_header);
	}
}

void OM_ProccessExternalOperation(OPERATION_HEADER_t* operation_header)
{
	if(!IS_COORDINATOR)
	{
		/* TESTING REGION */
		//For testing purposes just send throw UART port
		HAL_UartPrint("OPERATION RECEIVED >> FROM:");
		HAL_UartWriteNumberHEX(operation_header->sourceAddress);
		HAL_UartPrint("\t TO:");
		HAL_UartWriteNumberHEX(operation_header->destinationAddress);
		HAL_UartPrint("\t CODE:");
		HAL_UartWriteNumberHEX(operation_header->opCode);
		HAL_UartPrint("\t ARGS:");
	
		uint8_t length = getCommandArgsLength(&operation_header->opCode);
	
		for (uint8_t i = 0; i < length; i++)
		{
			HAL_UartWriteNumberHEX(*((uint8_t*)operation_header + sizeof(OPERATION_HEADER_t) + i));
			HAL_UartWriteByte(' ');
		}
	
		HAL_UartPrint("\r\n");
		if(operation_header->opCode == EXTENSION_OPCODE)
		return;
		/* END OF TESTING REGION */
	}	
	

	if(IS_COORDINATOR)
	{
		USART_SendOperation(operation_header);
	}else
	{
		//TODO: Check with internal address instead of configuration address...
		if(operation_header->destinationAddress == runningConfiguration.topConfiguration.networkConfig.deviceAddress) //MINE (EXTERNAL)
		{
			handleCommand(operation_header);
		}else
		{
			//TODO: Send or log ERROR (OPERATION_DEST_ADDR_ERROR)
		}	
	}
}

void OM_ProccessResponseOperation(OPERATION_HEADER_t* operation_header)
{
	if(IS_COORDINATOR)
	{
		USART_SendOperation(operation_header);
	}else
	{
		Radio_AddMessageByCopy(operation_header);
	}
}

void OM_ProccessResponseWithBodyOperation(OPERATION_HEADER_t* operation_header, uint8_t* bodyPtr, uint8_t bodyLength)
{
	if(IS_COORDINATOR)
	{
		USART_SendOperationWithBody(operation_header, bodyPtr, bodyLength);
	}else
	{
		Radio_AddMessageWithBodyByCopy(operation_header, bodyPtr, bodyLength);
	}
}