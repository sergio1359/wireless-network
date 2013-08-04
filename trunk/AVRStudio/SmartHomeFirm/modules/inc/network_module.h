/*
 * network_module.h
 *
 * Created: 17/05/2013 12:28:48
 *  Author: Victor
 */ 


#ifndef NETWORK_MODULE_H_
#define NETWORK_MODULE_H_

#include <stdint.h>
#include <stdbool.h>
#include "configManager.h"
#include "DS2401.h"

#define NETWORK_MODULE_DEFINITION  X(NetworkModule, networkModule_Init, networkModule_NotificationInd)

#define COMMANDS_TABLE_NETWORK \
X(MacRead,							0x20, networkMac_Handler,		0x00,					 MAC_READ_MESSAGE_t,							false)					\
X(MacReadResponse,					0x21, networkMac_Handler,		0x00,					 MAC_READ_RESPONSE_MESSAGE_t,					false)					\
X(NextHopRead,						0x22, networkNextHop_Handler,	0x00,					 NEXT_HOP_READ_MESSAGE_t,						false)					\
X(NextHopReadResponse,				0x23, networkNextHop_Handler,	0x00,					 NEXT_HOP_READ_RESPONSE_MESSAGE_t,				false)					\
X(RouteTableRead,					0x24, networkRoute_Handler,		0x00,					 ROUTE_TABLE_READ_MESSAGE_t,					false)					\
X(RouteTableReadResponse,			0x25, networkRoute_Handler,		routeRead_DataConf,		 ROUTE_TABLE_READ_RESPONSE_HEADER_MESSAGE_t,	true)					\
X(RouteTableReadConfirmation,		0x26, networkRoute_Handler,		0x00,					 ROUTE_TABLE_READ_CONFIRMATION_MESSAGE_t,		false)					\
X(PingRequest,						0x27, networkPing_Handler,		0x00,					 PING_REQUEST_MESSAGE_t,						false)					\
X(PingResponse,						0x28, networkPing_Handler,		0x00,					 PING_RESPONSE_MESSAGE_t,						false)					\
X(JoinRequest,						0x2A, 0x00,						0x00,					 JOIN_REQUEST_MESSAGE_t,						false)					\
X(JoinRequestResponse,				0x2B, 0x00,						0x00,					 JOIN_REQUEST_RESPONSE_MESSAGE_t,				false)					\
X(JoinAbort,						0x2C, 0x00,						0x00,					 JOIN_ABORT_MESSAGE_t,							false)					\
X(JoinAccept,						0x2D, 0x00,						0x00,					 JOIN_ACCEPT_MESSAGE_t,							false)					\
X(JoinAcceptResponse,				0x2E, 0x00,						0x00,					 JOIN_ACCEPT_RESPONSE_MESSAGE_t,				false)					\


//MAC
typedef struct
{
}MAC_READ_MESSAGE_t;

typedef struct
{
	uint8_t mac[SERIAL_NUMBER_SIZE];
}MAC_READ_RESPONSE_MESSAGE_t;

//NETWORK
typedef struct
{
}ROUTE_TABLE_READ_MESSAGE_t;

typedef struct
{
	uint8_t fragment:4; //LSB
	uint8_t fragmentTotal:4;//MSB
	uint8_t length;
}ROUTE_TABLE_READ_RESPONSE_HEADER_MESSAGE_t;

typedef struct
{
	uint8_t fragment:4; //LSB
	uint8_t fragmentTotal:4;//MSB
	uint8_t code;
}ROUTE_TABLE_READ_CONFIRMATION_MESSAGE_t;

typedef struct
{
	uint16_t nodeAddress;
}NEXT_HOP_READ_MESSAGE_t;

typedef struct
{
	uint16_t nodeAddress;
	uint16_t nextHopeAddress;
	uint8_t score;
	uint8_t lqi;
}NEXT_HOP_READ_RESPONSE_MESSAGE_t;

typedef struct
{
}PING_REQUEST_MESSAGE_t;

typedef struct
{
}PING_RESPONSE_MESSAGE_t;

//NETWORK JOIN
typedef struct
{
}JOIN_REQUEST_MESSAGE_t;

typedef struct
{
	uint8_t RSA_Key[16];
}JOIN_REQUEST_RESPONSE_MESSAGE_t;

typedef struct
{
	uint8_t NumberOfResponses;
}JOIN_ABORT_MESSAGE_t;

typedef struct
{
	uint8_t MacAddress[SERIAL_NUMBER_SIZE];
	uint8_t AES_Key[16];
}JOIN_ACCEPT_MESSAGE_t;

typedef struct
{
	uint16_t Address;
	uint8_t  Network_AES_Key[16];
}JOIN_ACCEPT_RESPONSE_MESSAGE_t;


void networkModule_Init(void);
void networkModule_NotificationInd(uint8_t sender, OPERATION_HEADER_t* notification);

/*- Handlers --------------------------------------------------------*/
void networkMac_Handler(OPERATION_HEADER_t* operation_header);
void networkNextHop_Handler(OPERATION_HEADER_t* operation_header);
void networkRoute_Handler(OPERATION_HEADER_t* operation_header);
void networkPing_Handler(OPERATION_HEADER_t* operation_header);

/*- Data Confirmations --------------------------------------------------------*/
void routeRead_DataConf(OPERATION_DataConf_t *req);

#endif /* NETWORK_MODULE_H_ */