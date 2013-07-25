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
#include "configManager.h"

#define TIME_MODULE_DEFINITION  X(TimeModule, timeModule_Init, timeModule_NotificationInd)

#define COMMANDS_TABLE_TIME \
X(DateTimeWrite,				0x30, time_Handler,		timeModule_DataConf,	TIME_WRITE_MESSAGE_t,					false)	\
X(DateTimeRead,					0x31, time_Handler,		timeModule_DataConf,	TIME_READ_MESSAGE_t,					false)	\
X(DateTimeReadResponse,			0x32, time_Handler,		timeModule_DataConf,	TIME_READ_RESPONSE_MESSAGE_t,			false)	\


typedef struct
{
	WEEKDAY_t week;
	DATE_t date;
	TIME_t time;
}TIME_WRITE_MESSAGE_t;

typedef struct
{
}TIME_READ_MESSAGE_t;

typedef struct
{
	WEEKDAY_t week;
	DATE_t date;
	TIME_t time;
}TIME_READ_RESPONSE_MESSAGE_t;


void timeModule_Init(void);
void timeModule_NotificationInd(uint8_t sender, OPERATION_HEADER_t* notification);
void timeModule_DataConf(NWK_DataReq_t *req);

void time_Handler(OPERATION_HEADER_t* operation_header);


#endif /* TIME_MODULE_H_ */