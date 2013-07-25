/*
 * operationsManager.c
 *
 * Created: 11/05/2013 22:28:47
 *  Author: Victor
 */ 

#include "operationsManager.h"
#include "modulesManager.h"
#include "timeManager.h"
#include "radioManager.h"
#include "globals.h"


INPUT_UART_HEADER_t coordinator_UART_header;

_Bool checkTimeRestrictions(uint16_t operationAddress);
_Bool checkConditions(uint16_t operationAddress);


void OM_Init(void)
{
	coordinator_UART_header.endPoint	= 1; //Application endpoint
	coordinator_UART_header.nextHop		= 0; //No hop
	coordinator_UART_header.routing		= 0; //
	coordinator_UART_header.rssi		= 0; //Highest quality
	coordinator_UART_header.security	= 1; //SecurityEnabled
}

void OM_ProccessInternalOperation(OPERATION_HEADER_t* operation_header)
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
	uint16_t operationConfigAddress = CONFIG_GetOperationConfigAddress(operation_header);
	if(!checkTimeRestrictions(operationConfigAddress) || !checkConditions(operationConfigAddress)) //Restriction not satisfied
		return;
	
	if(operation_header->destinationAddress == 0) //MINE (INTERNAL)
	{
		MODULES_HandleCommand(operation_header);
	}else
	{
		Radio_AddMessageByReference(operation_header, MODULES_DataConf(&operation_header->opCode));
	}
}
	

void OM_ProccessExternalOperation(INPUT_UART_HEADER_t* input_header, OPERATION_HEADER_t* operation_header)
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
		USART_SendOperation(input_header, operation_header);
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
	//TODO: Condition "operation_header->destinationAddress == 0" added for testing...
	if(IS_COORDINATOR || operation_header->destinationAddress == 0)
	{
		USART_SendOperation(&coordinator_UART_header, operation_header);
	}else
	{
		Radio_AddMessageByCopy(operation_header, MODULES_DataConf(&operation_header->opCode));
	}
}

void OM_ProccessResponseWithBodyOperation(OPERATION_HEADER_t* operation_header, uint8_t* bodyPtr, uint8_t bodyLength)
{
	if(IS_COORDINATOR)
	{
		USART_SendOperationWithBody(&coordinator_UART_header, operation_header, bodyPtr, bodyLength);
	}else
	{
		Radio_AddMessageWithBodyByCopy(operation_header, bodyPtr, bodyLength, MODULES_DataConf(&operation_header->opCode));
	}
}

void OM_ProccessUARTOperation(OUTPUT_UART_HEADER_t* output_header, OPERATION_HEADER_t* operation_header)
{
	//TODO: EVERY NODE CAN BE USED AS DONGLE WITHOUT CONFIGURATION. RIHGT?
	
	if(operation_header->destinationAddress == 0) //MINE (INTERNAL)
	{
		MODULES_HandleCommand(operation_header);
	}else
	{
		//TODO: ELIMINAR ESTO, PARA EVITAR UNA POSIBLE VOMITONA DE RAFA :) O NO?
		operation_header->sourceAddress = runningConfiguration.topConfiguration.networkConfig.deviceAddress;
	
		Radio_AddMessageByCopy(operation_header, MODULES_DataConf(&operation_header->opCode));
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
			restriction_passed = VALID_DATETIME &&																											//System DateTime
								 ((currentWeek.raw & restric->weekDays.raw) != 0) &&																		//Day of the week
								 ((TIME_CompareTimes(restric->start, currentTime) <= 0) && (TIME_CompareTimes(restric->end, currentTime) >= 0)) &&			//In time
								 ((TIME_CompareDates(restric->dateFrom, currentDate) <= 0) && (TIME_CompareDates(restric->dateTo, currentDate) >= 0));		//In date
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