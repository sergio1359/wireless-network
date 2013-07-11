/*
 * operationsManager.c
 *
 * Created: 11/05/2013 22:28:47
 *  Author: Victor
 */ 

#include "operationsManager.h"
#include "modulesManager.h"
#include "radioManager.h"
#include "uartManager.h"
#include "globals.h"

_Bool checkTimeRestrictions(uint16_t operationAddress);
_Bool checkConditions(uint16_t operationAddress);

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
	
		uint8_t length = MODULES_GetCommandArgsLength(&operation_header->opCode);
	
		for (uint8_t i = 0; i < length; i++)
		{
			HAL_UartWriteNumberHEX(*((uint8_t*)operation_header + sizeof(OPERATION_HEADER_t) + i));
			HAL_UartWriteByte(' ');
		}
	
		HAL_UartPrint("\r\n");
		if(operation_header->opCode == EXTENSION_OPCODE)
		return;
	}
	/* END OF TESTING REGION */
	
	if(!checkTimeRestrictions(operation_header) || !checkConditions(operation_header)) //Restriction not satisfied
		return;
	
	if(operation_header->destinationAddress == 0) //MINE (INTERNAL)
	{
		MODULES_HandleCommand(operation_header);
	}else
	{
		//TODO: ELIMINAR ESTO, PARA EVITAR UNA POSIBLE VOMITONA DE RAFA :) O NO?
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
	
		uint8_t length = MODULES_GetCommandArgsLength(&operation_header->opCode);
	
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
			MODULES_HandleCommand(operation_header);
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

/************************************************************************/
/*                       INTERNAL METHODS                               */
/************************************************************************/

_Bool checkTimeRestrictions(uint16_t operationAddress)
{
	uint16_t res_ptr;
	OPERATION_TIME_RESTRICTION_t* restric;
	_Bool restriction_passed = true;
	
	//Looks for the first temporary restriction applicable to the current list of operations. (Operation address greater or equal than the list start address to process)
	for(res_ptr = OPERATION_TIME_RESTRIC_LIST_START_ADDRESS; res_ptr < OPERATION_TIME_RESTRIC_LIST_END_ADDRESS; res_ptr += sizeof(OPERATION_TIME_RESTRICTION_t))
	{
		restric = (OPERATION_TIME_RESTRICTION_t*)&runningConfiguration.raw[res_ptr];
		
		if(restric->operationAddress == operationAddress && restriction_passed)
		{
			restriction_passed =  ( (TIME_CompareTimes(restric->start, currentTime) <= 0) && (TIME_CompareTimes(restric->end, currentTime) >= 0) );			//In time
			restriction_passed &= ( (currentWeek.raw & restric->weekDays.raw) != 0);																		//Day of the week
			restriction_passed &= ( (TIME_CompareDates(restric->dateFrom, currentDate) <= 0) && (TIME_CompareDates(restric->dateTo, currentDate) >= 0) );	//In date
		}else if(restric->operationAddress > operationAddress || !restriction_passed)
		{
			break;
		}
	}
	
	return restriction_passed;
}

_Bool checkConditions(uint16_t operationAddress)
{
	_Bool restriction_passed = true;
	return restriction_passed;
}