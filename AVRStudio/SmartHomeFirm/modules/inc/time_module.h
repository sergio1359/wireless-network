/*
 * time_module.h
 *
 * Created: 01/06/2013 14:11:36
 *  Author: Victor
 */ 


#ifndef TIME_MODULE_H_
#define TIME_MODULE_H_


#include <stdint.h>
#include <stdbool.h>
#include "EEPROM.h"

#define TIME_MODULE_DEFINITION  X(TimeModule, timeModule_Init, timeModule_NotificationInd)

#define COMMANDS_TABLE_TIME \
X(TimeWrite,					0x30, time_Handler,		TIME_WRITE_MESSAGE_t,					false)	\
X(TimeRead,						0x31, time_Handler,		TIME_READ_MESSAGE_t,					false)	\
X(TimeReadResponse,				0x32, time_Handler,		TIME_READ_RESPONSE_MESSAGE_t,			false)	\
X(DateWrite,					0x33, date_Handler,		DATE_WRITE_MESSAGE_t,					false)	\ 
X(DateRead,						0x34, date_Handler,		DATE_READ_MESSAGE_t,					false)	\
X(DateReadResponse,				0x35, date_Handler,		DATE_READ_RESPONSE_MESSAGE_t,			false)	\
X(DateTimeRequest,				0x36, request_Handler,	DATE_TIME_REQUEST_MESSAGE_t,			false)	\
X(DateTimeRequestResponse,		0x37, request_Handler,	DATE_TIME_REQUEST_RESPONSE_MESSAGE_t,	false)	\

//TIME
typedef struct
{
	TIME_t time;
}TIME_WRITE_MESSAGE_t;

typedef struct
{
}TIME_READ_MESSAGE_t;

typedef struct
{
	TIME_t time;
}TIME_READ_RESPONSE_MESSAGE_t;

//DATE
typedef struct
{
	DATE_t date;
}DATE_WRITE_MESSAGE_t;

typedef struct
{
}DATE_READ_MESSAGE_t;

typedef struct
{
	DATE_t date;
}DATE_READ_RESPONSE_MESSAGE_t;

//REQUEST
typedef struct
{
}DATE_TIME_REQUEST_MESSAGE_t;

typedef struct
{
	DATE_t date;
	TIME_t time;
}DATE_TIME_REQUEST_RESPONSE_MESSAGE_t;



void timeModule_Init(void);
void timeModule_NotificationInd(uint8_t sender, OPERATION_HEADER_t* notification);

void time_Handler(OPERATION_HEADER_t* operation_header);
void date_Handler(OPERATION_HEADER_t* operation_header);
void request_Handler(OPERATION_HEADER_t* operation_header);


#endif /* TIME_MODULE_H_ */