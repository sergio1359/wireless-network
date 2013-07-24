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

#define NETWORK_MODULE_DEFINITION  X(NetworkModule, networkModule_Init, networkModule_DataConf, networkModule_NotificationInd)

#define COMMANDS_TABLE_NETWORK \
X(MacRead,							0x20, mac_Handler,	 MAC_READ_MESSAGE_t,								false)					\
X(MacReadResponse,					0x21, mac_Handler,	 MAC_READ_RESPONSE_MESSAGE_t,						false)					\
X(NextHopRead,						0x22, route_Handler, NEXT_HOP_READ_MESSAGE_t,							false)					\
X(NextHopReadResponse,				0x23, route_Handler, NEXT_HOP_READ_RESPONSE_MESSAGE_t,					false)					\
X(RouteTableRead,					0x24, route_Handler, ROUTE_TABLE_READ_MESSAGE_t,						false)					\
X(RouteTableReadResponse,			0x25, route_Handler, ROUTE_TABLE_READ_RESPONSE_HEADER_MESSAGE_t,		true)					\
X(RouteTableReadConfirmation,		0x26, route_Handler, ROUTE_TABLE_READ_CONFIRMATION_MESSAGE_t,			false)					\


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


void networkModule_Init(void);
void networkModule_NotificationInd(uint8_t sender, OPERATION_HEADER_t* notification);
static void networkModule_DataConf(NWK_DataReq_t *req);

void mac_Handler(OPERATION_HEADER_t* operation_header);
void route_Handler(OPERATION_HEADER_t* operation_header);

#endif /* NETWORK_MODULE_H_ */