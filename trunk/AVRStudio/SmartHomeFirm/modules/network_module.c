/*
 * network_module.c
 *
 * Created: 17/05/2013 12:28:33
 *  Author: Victor
 */ 
#include "modulesManager.h"
#include "globals.h"
#include "nwk.h"

#include "APP_SESSION.h"

#define ROUTE_TABLE_BUFFER_SIZE (NWK_ROUTE_TABLE_SIZE * sizeof(NWK_RouteTableEntry_t))

static struct
{
	OPERATION_HEADER_t header;
	MAC_READ_RESPONSE_MESSAGE_t response;
}macResponse;

static struct
{
	OPERATION_HEADER_t header;
	ROUTE_TABLE_READ_RESPONSE_HEADER_MESSAGE_t response;
}routeTableResponse;

static struct
{
	OPERATION_HEADER_t header;
	NEXT_HOP_READ_RESPONSE_MESSAGE_t response;
}nextHopResponse;

static struct
{
	OPERATION_HEADER_t header;
	PING_RESPONSE_MESSAGE_t response;
}pingResponse;

static uint8_t routeTableBuffer[ROUTE_TABLE_BUFFER_SIZE];
static uint8_t currentTableSize = 0;

static ReadSession_t readRouteSession;


void networkModule_Init(void)
{
	//Set responses opCodes
	macResponse.header.opCode			= MacReadResponse;
	routeTableResponse.header.opCode	= RouteTableReadResponse;
	nextHopResponse.header.opCode		= NextHopReadResponse;
	pingResponse.header.opCode			= PingResponse;
	
	readRouteSession.sendingState = false;
	readRouteSession.readBuffer = (uint8_t*)routeTableBuffer;
}

void networkModule_NotificationInd(uint8_t sender, OPERATION_HEADER_t* notification)
{
	
}


/*- MacRead --------------------------------------------------*/
void networkMac_Handler(OPERATION_HEADER_t* operation_header)
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


/*- NextHop --------------------------------------------------*/
void networkNextHop_Handler(OPERATION_HEADER_t* operation_header)
{
	if(operation_header->opCode == NextHopRead)
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


/*- RouteRead --------------------------------------------------*/
void networkRoute_Handler(OPERATION_HEADER_t* operation_header)
{
	if(operation_header->opCode == RouteTableRead)
	{
		
		routeTableResponse.header.sourceAddress = runningConfiguration.topConfiguration.networkConfig.deviceAddress;
		routeTableResponse.header.destinationAddress = operation_header->sourceAddress;
		
		if(readRouteSession.sendingState && operation_header->sourceAddress != readRouteSession.destinationAddress)
		{
			//BUSY SENDING CONFIG STATE
			routeTableResponse.response.fragment = 0;
			routeTableResponse.response.fragmentTotal = 0;
			routeTableResponse.response.length = 0;
		}else
		{
			NWK_CopyRouteTable(&routeTableBuffer, currentTableSize);
			
			readRouteSession.sendingState = true;
			readRouteSession.destinationAddress = operation_header->sourceAddress;
			
			readRouteSession.currentSendIndex = 0;
			readRouteSession.currentSendFragment = 0;
			readRouteSession.totalSendExpected = CEILING(currentTableSize , MAX_CONTENT_MESSAGE_SIZE);
			readRouteSession.currentSendFrameSize = MIN(MAX_CONTENT_MESSAGE_SIZE, currentTableSize - readRouteSession.currentSendIndex);
			
			routeTableResponse.response.fragment = readRouteSession.currentSendFragment;
			routeTableResponse.response.fragmentTotal = readRouteSession.totalSendExpected;
			routeTableResponse.response.length = readRouteSession.currentSendFrameSize;
			
			OM_ProccessResponseWithBodyOperation(&routeTableResponse.header, readRouteSession.readBuffer, readRouteSession.currentSendFrameSize);
		}
	}else if(operation_header->opCode == RouteTableReadConfirmation)
	{
		if(readRouteSession.sendingState)
		{
			ROUTE_TABLE_READ_CONFIRMATION_MESSAGE_t* msg = (ROUTE_TABLE_READ_CONFIRMATION_MESSAGE_t*)(operation_header + 1);
			
			if(msg->fragment == readRouteSession.currentSendFragment && msg->fragmentTotal == readRouteSession.totalSendExpected)
			{
				if(msg->code == 0x00) //'OK'
				{
					if(readRouteSession.currentSendFragment <= readRouteSession.totalSendExpected)	 //Something to send
					{
						readRouteSession.currentSendIndex += readRouteSession.currentSendFrameSize;
						readRouteSession.currentSendFragment++;
						
						readRouteSession.currentSendFrameSize = MIN(MAX_CONTENT_MESSAGE_SIZE, currentTableSize - readRouteSession.currentSendIndex);
						
						routeTableResponse.response.fragment = readRouteSession.currentSendFragment;
						routeTableResponse.response.fragmentTotal = readRouteSession.totalSendExpected;
						routeTableResponse.response.length = readRouteSession.currentSendFrameSize;
						
						OM_ProccessResponseWithBodyOperation(&routeTableResponse.header,&readRouteSession.readBuffer[readRouteSession.currentSendIndex], readRouteSession.currentSendFrameSize);
					}else
					{
						//Finish
						readRouteSession.sendingState = false;
					}
				}else
				{
					//Something wrong at server size. Abort current session
					readRouteSession.sendingState = false;
				}
			}else
			{
				readRouteSession.sendingState = false;
				//TODO: SEND OR LOG ERROR (FRAGMENT OR FRAGMENT TOTAL NOT EXPECTED)
			}
		}else
		{
			//TODO: SEND OR LOG ERROR (NOT SENDING)
		}
	}else if(operation_header->opCode == RouteTableReadResponse)
	{
		//TODO: SEND NOTIFICATION
	}				
}

void routeRead_DataConf(OPERATION_DataConf_t *req)
{
	if (!req->sendOk)
	{
		OM_ProccessResponseWithBodyOperation(&routeTableResponse.header,&readRouteSession.readBuffer[readRouteSession.currentSendIndex], readRouteSession.currentSendFrameSize);//Resend last response
	}
}


/*- Ping --------------------------------------------------*/
void networkPing_Handler(OPERATION_HEADER_t* operation_header)
{
	if(operation_header->opCode == PingRequest)
	{
		pingResponse.header.sourceAddress = runningConfiguration.topConfiguration.networkConfig.deviceAddress;
		pingResponse.header.destinationAddress = operation_header->sourceAddress;
		
		OM_ProccessResponseOperation(&pingResponse.header);
	}else if(operation_header->opCode == PingResponse)
	{
		//TODO: SEND NOTIFICATION
	}
}