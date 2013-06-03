/*
 * network_module.c
 *
 * Created: 17/05/2013 12:28:33
 *  Author: Victor
 */ 
#include "modulesManager.h"
#include "globals.h"
#include "nwk.h"

void networkModule_Init(void)
{
	
}

void networkModule_NotificationInd(uint8_t sender, OPERATION_HEADER_t* notification)
{
	
}

void route_Handler(OPERATION_HEADER_t* operation_header)
{
	if(operation_header->opCode == RouteTableRead)
	{
		uint8_t table_length = NWK_ROUTE_TABLE_SIZE * sizeof(NwkRouteTableRecord_t);
		ROUTE_TABLE_READ_RESPONSE_HEADER_t* response;
		response->length = table_length;
		NWK_RouteTable();
	}		
}