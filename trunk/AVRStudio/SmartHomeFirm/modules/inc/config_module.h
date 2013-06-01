/*
 * config_module.h
 *
 * Created: 31/05/2013 19:40:55
 *  Author: Victor
 */ 


#ifndef CONFIG_MODULE_H_
#define CONFIG_MODULE_H_

#include <stdint.h>
#include <stdbool.h>
#include "EEPROM.h"

#define CONFIG_MODULE_DEFINITION  X(ConfigModule, configModule_Init, configModule_NotificationInd)

#define COMMANDS_TABLE_CONFIG \
X(ConfigWrite,				0x40, configWrite_Handler,		CONFIG_WRITE_HEADER_MESSAGE_t,			true)	\
X(ConfigRead,				0x41, configRead_Handler,		CONFIG_READ_MESSAGE_t,					false)	\
X(ConfigReadResponse,		0x42, configRead_Handler,		CONFIG_READ_RESPONSE_HEADER_MESSAGE_t,	true)	\
X(ConfigChecksum,			0x43, configChecksum_Handler,	CONFIG_CHECKSUM_MESSAGE_t,				false)	\
X(ConfigChecksumResponse,	0x44, configChecksum_Handler,	CONFIG_CHECKSUM_RESPONSE_MESSAGE_t,		false)	\


// CONFIGURATION
typedef struct
{
	uint8_t fragment:4; //LSB
	uint8_t fragmentTotal:4;//MSB
	uint8_t length;
}CONFIG_WRITE_HEADER_MESSAGE_t;

typedef struct
{
}CONFIG_READ_MESSAGE_t;

typedef struct
{
	uint8_t fragment:4; //LSB
	uint8_t fragmentTotal:4;//MSB
	uint8_t length;
}CONFIG_READ_RESPONSE_HEADER_MESSAGE_t;

typedef struct
{
}CONFIG_CHECKSUM_MESSAGE_t;

typedef struct
{
	uint16_t checksum;
}CONFIG_CHECKSUM_RESPONSE_MESSAGE_t;


void configModule_Init(void);
void configModule_NotificationInd(uint8_t sender, OPERATION_HEADER_t* notification);

void configWrite_Handler(OPERATION_HEADER_t* operation_header);
void configRead_Handler(OPERATION_HEADER_t* operation_header);
void configChecksum_Handler(OPERATION_HEADER_t* operation_header);

#endif /* CONFIG_MODULE_H_ */