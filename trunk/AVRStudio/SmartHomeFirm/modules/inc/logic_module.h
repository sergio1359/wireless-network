/*
 * logic_module.h
 *
 * Created: 12/05/2013 13:00:15
 *  Author: Victor
 */ 


#ifndef LOGIC_MODULE_H_
#define LOGIC_MODULE_H_

#include <stdint.h>
#include <stdbool.h>
#include "RTC.h"
#include "configManager.h"

#define LOGIC_MODULE_DEFINITION  X(LogicModule, logicModule_Init, logicModule_NotificationInd)

#define COMMANDS_TABLE_LOGIC														\
X(LogicWrite,			0x40, logic_Handler,	logicModule_DataConf,	LOGIC_WRITE_MESSAGE_t,			false)	\
X(LogicSwitch,			0x41, logic_Handler,	logicModule_DataConf,	LOGIC_SWITCH_MESSAGE_t,			false)	\
X(LogicRead,			0x42, logic_Handler,	logicModule_DataConf,	LOGIC_READ_MESSAGE_t,			false)	\
X(LogicReadResponse,	0x43, logic_Handler,	logicModule_DataConf,	LOGIC_READ_RESPONSE_MESSAGE_t,	false)	\

//LOGIC PORT MESSAGES
typedef struct
{
	uint16_t address;
	uint8_t value;
	uint8_t seconds;
}LOGIC_WRITE_MESSAGE_t;

typedef struct
{
	uint16_t address;
	uint8_t seconds;
}LOGIC_SWITCH_MESSAGE_t;

typedef struct
{
	uint16_t address;
}LOGIC_READ_MESSAGE_t;

typedef struct
{
	uint16_t address;
	uint8_t value;
}LOGIC_READ_RESPONSE_MESSAGE_t;


//CONFIGURATION

typedef struct
{
	unsigned changeType		: 2; //LSB
	unsigned defaultValue	: 1;
	unsigned maskIO			: 1; //Input -> 0	Output -> 1
	unsigned reserved		: 4; //MSB	
}LOGIC_BITS_CONFIG_t;

typedef struct
{
	 CONFIG_MODULE_ELEM_HEADER_t operationsInfo;
	 uint16_t deviceID;
	 uint8_t pinAddress;
	 LOGIC_BITS_CONFIG_t configBits;
}LOGIC_CONFIG_t;

void logicModule_Init(void);
void logicModule_NotificationInd(uint8_t sender, OPERATION_HEADER_t* notification);
void logicModule_DataConf(NWK_DataReq_t *req);

void logic_Handler(OPERATION_HEADER_t* operation_header);

#endif /* LOGIC_MODULE_H_ */