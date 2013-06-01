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
#include "EEPROM.h"

#define NETWORK_MODULE_DEFINITION  X(NetworkModule, networkModule_Init, networkModule_NotificationInd)

#define COMMANDS_TABLE_NETWORK \
X(RouteTableRead,			0x01, route_Handler, ROUTE_TABLE_READ_t,	false)					\
X(RouteTableReadResponse,	0x02, route_Handler, ROUTE_TABLE_READ_RESPONSE_HEADER_t,	true)	\

//NETWORK
typedef struct
{
}ROUTE_TABLE_READ_t;

typedef struct
{
	uint8_t fragment:4; //LSB
	uint8_t fragmentTotal:4;//MSB
	uint8_t length;
}ROUTE_TABLE_READ_RESPONSE_HEADER_t;


void networkModule_Init(void);
void networkModule_NotificationInd(uint8_t sender, OPERATION_HEADER_t* notification);

void route_Handler(OPERATION_HEADER_t* operation_header);

#endif /* NETWORK_MODULE_H_ */