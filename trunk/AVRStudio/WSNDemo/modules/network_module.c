/*
 * network_module.c
 *
 * Created: 17/05/2013 12:28:33
 *  Author: Victor
 */ 
#include "modules.h"
#include "globals.h"

void networkInit(void)
{
}

void networkHandler(OPERATION_HEADER_t* operation_header, uint16_t sourceAddress)
{
	if(operation_header->opCode == 0)
	{
	}		
}