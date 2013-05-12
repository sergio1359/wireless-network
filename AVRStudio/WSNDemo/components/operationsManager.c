/*
 * operationsManager.c
 *
 * Created: 11/05/2013 22:28:47
 *  Author: Victor
 */ 

#include "operationsManager.h"

void OM_Proccess_Operation(OPERATION_HEADER_t* operation_header)
{
	if(operation_header->destinationAddress == 0) //MINE
	{
		handleCommand((operation_header->opCode));
	}else
	{
		RF_Send_Message(operation_header);
	}
}