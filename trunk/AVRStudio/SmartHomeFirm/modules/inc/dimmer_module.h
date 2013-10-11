/*
 * dimmer_module.h
 *
 * Created: 11/10/2013 16:22:51
 *  Author: Victor
 */ 


#ifndef DIMMER_MODULE_H_
#define DIMMER_MODULE_H_

#include <stdint.h>
#include <stdbool.h>
#include <util/delay.h>
#include "RTC.h"
#include "configManager.h"
#include "operationsManager.h"

#define DIMMER_MODULE_DEFINITION  X(DimmerModule, dimmerModule_Init, dimmerModule_NotificationInd)

#define COMMANDS_TABLE_DIMMER																					\
X(DimmerWrite,			0x46, dimmer_Handler,	0x00,					DIMMER_WRITE_MESSAGE_t,			false)	\
X(DimmerRead,			0x47, dimmer_Handler,	0x00,					DIMMER_READ_MESSAGE_t,			false)	\
X(DimmerReadResponse,	0x48, dimmer_Handler,	dimmerRead_DataConf,	DIMMER_READ_RESPONSE_MESSAGE_t,	false)	\

//DIMMER CONTROL MESSAGES

typedef struct
{
	uint16_t address;
	uint8_t value;
	uint8_t seconds;
}DIMMER_WRITE_MESSAGE_t;

typedef struct
{
	uint16_t address;
}DIMMER_READ_MESSAGE_t;

typedef struct
{
	uint16_t address;
	uint8_t value;
}DIMMER_READ_RESPONSE_MESSAGE_t;


//CONFIGURATION

typedef struct
{
	CONFIG_MODULE_ELEM_HEADER_t operationsInfo;
	uint16_t deviceID;
	uint8_t pinAddress;
}DIMMER_CONFIG_t;

void dimmerModule_Init(void);
void dimmerModule_NotificationInd(uint8_t sender, OPERATION_HEADER_t* notification);

void dimmer_Handler(OPERATION_HEADER_t* operation_header);

void dimmerRead_DataConf(OPERATION_DataConf_t *req);

#endif /* DIMMER_MODULE_H_ */