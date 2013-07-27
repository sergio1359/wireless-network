/*
 * network_module.c
 *
 * Created: 17/05/2013 12:28:33
 *  Author: Victor
 */ 
#include "modulesManager.h"
#include "globals.h"
#include "nwk.h"

struct
{
	OPERATION_HEADER_t header;
	MAC_READ_RESPONSE_MESSAGE_t response;
}macResponse;

struct
{
	OPERATION_HEADER_t header;
	ROUTE_TABLE_READ_RESPONSE_HEADER_MESSAGE_t response;
}routeTableResponse;

struct
{
	OPERATION_HEADER_t header;
	NEXT_HOP_READ_RESPONSE_MESSAGE_t response;
}nextHopResponse;

static uint8_t routeTableBuffer[NWK_ROUTE_TABLE_SIZE];
static uint8_t bufferSize = 0;
static _Bool routeSendingState;
static uint8_t currentRouteFragment;
static uint8_t totalRouteExpected;
static uint16_t currentRouteIndex;
static uint16_t currentRouteFrameSize;

void networkModule_Init(void)
{
	//Set responses opCodes
	macResponse.header.opCode			= MacReadResponse;
	routeTableResponse.header.opCode	= RouteTableReadResponse;
	nextHopResponse.header.opCode		= NextHopReadResponse;
}

void networkModule_DataConf(OPERATION_DataConf_t *req)
{
	
}

void networkModule_NotificationInd(uint8_t sender, OPERATION_HEADER_t* notification)
{
	
}

void mac_Handler(OPERATION_HEADER_t* operation_header)
{
	if(operation_header->opCode == MacRead)
	{
		macResponse.header.sourceAddress = runningConfiguration.topConfiguration.networkConfig.deviceAddress;
		macResponse.header.destinationAddress = operation_header->sourceAddress;
		
		memcpy((uint8_t*)&macResponse.response.mac,(uint8_t*)&serialNumber, SERIAL_NUMBER_SIZE);
		
		OM_ProccessResponseOperation(&macResponse.header);
	}else if(operation_header->opCode == MacReadResponse)
	{
		//TODO: SEND NOTIFICATION
	}
}

void route_Handler(OPERATION_HEADER_t* operation_header)
{
	if(operation_header->opCode == RouteTableRead)
	{
		if(!routeSendingState)
		{
			routeTableResponse.header.sourceAddress = runningConfiguration.topConfiguration.networkConfig.deviceAddress;
			routeTableResponse.header.destinationAddress = operation_header->sourceAddress;
			
			NWK_CopyRouteTable(&routeTableBuffer, bufferSize);
			
			routeSendingState = true;
			
			currentRouteIndex = 0;
			currentRouteFragment = 0;
			totalRouteExpected = CEILING(bufferSize , MAX_CONTENT_MESSAGE_SIZE);
			currentRouteFrameSize = MIN(MAX_CONTENT_MESSAGE_SIZE, bufferSize - currentRouteIndex);
			
			routeTableResponse.response.fragment = currentRouteFragment;
			routeTableResponse.response.fragmentTotal = totalRouteExpected;
			routeTableResponse.response.length = currentRouteFrameSize;
			
			OM_ProccessResponseWithBodyOperation(&routeTableResponse.header,&routeTableBuffer, currentRouteFrameSize);
		}else
		{
			//TODO: SEND OR LOG ERROR (BUSY SENDING ROUTE STATE)
		}
	}else if(operation_header->opCode == RouteTableReadConfirmation)
	{
		if(routeSendingState)
		{
			ROUTE_TABLE_READ_CONFIRMATION_MESSAGE_t* msg = (ROUTE_TABLE_READ_CONFIRMATION_MESSAGE_t*)(operation_header + 1);
			
			if(msg->fragment == currentRouteFragment && msg->fragmentTotal == totalRouteExpected)
			{
				if(msg->code == 0x00) //'OK'
				{
					if(currentRouteFragment < totalRouteExpected)	 //Something to send
					{
						currentRouteIndex += currentRouteFrameSize;
						currentRouteFragment++;
						
						currentRouteFrameSize = MIN(MAX_CONTENT_MESSAGE_SIZE, bufferSize - currentRouteIndex);
						
						routeTableResponse.response.fragment = currentRouteFragment;
						routeTableResponse.response.fragmentTotal = totalRouteExpected;
						routeTableResponse.response.length = currentRouteFrameSize;
						
						OM_ProccessResponseWithBodyOperation(&routeTableResponse.header,&routeTableBuffer[currentRouteIndex], currentRouteFrameSize);
					}else
					{
						//Finish
						routeSendingState = false;
					}
				}else
				{
					//Something wrong at server size. Abort current session
					routeSendingState = false;
				}
			}else
			{
				routeSendingState = false;
				//TODO: SEND OR LOG ERROR (FRAGMENT OR FRAGMENT TOTAL NOT EXPECTED)
			}
		}else
		{
			//TODO: SEND OR LOG ERROR (NOT SENDING)
		}
	}else if(operation_header->opCode == RouteTableReadResponse)
	{
		//TODO: SEND NOTIFICATION
	}else if(operation_header->opCode == NextHopRead)
	{
		NEXT_HOP_READ_MESSAGE_t* msg = (NEXT_HOP_READ_MESSAGE_t*)(operation_header + 1);
		NWK_RouteTableEntry_t *rec = NWK_RouteFindEntry(msg->nodeAddress, 0);
		
		nextHopResponse.header.sourceAddress = runningConfiguration.topConfiguration.networkConfig.deviceAddress;
		nextHopResponse.header.destinationAddress = operation_header->sourceAddress;
		
		nextHopResponse.response.nodeAddress = msg->nodeAddress;
		nextHopResponse.response.nextHopeAddress = rec->nextHopAddr;
		nextHopResponse.response.lqi = rec->lqi;
		nextHopResponse.response.score = rec->score;
		
		OM_ProccessResponseOperation(&nextHopResponse.header);
	}else if(operation_header->opCode == NextHopReadResponse)
	{
		//TODO: SEND NOTIFICATION
	}					
}