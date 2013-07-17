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
#include "configManager.h"

#define CONFIG_MODULE_DEFINITION  X(ConfigModule, configModule_Init, configModule_NotificationInd)

#define COMMANDS_TABLE_CONFIG \
X(Reset,						0x00, configSystem_Handler,		CONFIG_WRITE_HEADER_MESSAGE_t,				false)	\
X(FirmwareVersionRead,			0x01, configSystem_Handler,		FIRMWARE_VERSION_READ_MESSAGE_t,			false)	\
X(FirmwareVersionReadResponse,	0x02, configSystem_Handler,		FIRMWARE_VERSION_READ_RESPONSE_MESSAGE_t,	false)	\
X(ShieldModelRead,				0x03, configSystem_Handler,		SHIELD_MODEL_READ_MESSAGE_t,				false)	\
X(ShieldModelReadResponse,		0x04, configSystem_Handler,		SHIELD_MODEL_READ_RESPONSE_MESSAGE_t,		false)	\
X(BaseModelRead,				0x05, configSystem_Handler,		BASE_MODEL_READ_MESSAGE_t,					false)	\
X(BaseModelReadResponse,		0x06, configSystem_Handler,		BASE_MODEL_READ_RESPONSE_MESSAGE_t,			false)	\
X(ConfigWrite,					0x07, configWrite_Handler,		CONFIG_WRITE_HEADER_MESSAGE_t,				true)	\
X(ConfigWriteResponse,			0x08, configWrite_Handler,		CONFIG_WRITE_RESPONSE_MESSAGE_t,			false)	\
X(ConfigRead,					0x09, configRead_Handler,		CONFIG_READ_MESSAGE_t,						false)	\
X(ConfigReadResponse,			0x10, configRead_Handler,		CONFIG_READ_RESPONSE_HEADER_MESSAGE_t,		true)	\
X(ConfigReadConfirmation,		0x11, configRead_Handler,		CONFIG_READ_CONFIRMATION_MESSAGE_t,			false)	\
X(ConfigChecksum,				0x12, configChecksum_Handler,	CONFIG_CHECKSUM_MESSAGE_t,					false)	\
X(ConfigChecksumResponse,		0x13, configChecksum_Handler,	CONFIG_CHECKSUM_RESPONSE_MESSAGE_t,			false)	\

//SYSTEM
typedef struct
{
}RESET_MESSAGE_t;

typedef struct
{
}FIRMWARE_VERSION_READ_MESSAGE_t;

typedef struct
{
	uint8_t version;
}FIRMWARE_VERSION_READ_RESPONSE_MESSAGE_t;

typedef struct
{
}SHIELD_MODEL_READ_MESSAGE_t;

typedef struct
{
	uint8_t model;
}SHIELD_MODEL_READ_RESPONSE_MESSAGE_t;

typedef struct
{
}BASE_MODEL_READ_MESSAGE_t;

typedef struct
{
	uint8_t model;
}BASE_MODEL_READ_RESPONSE_MESSAGE_t;

// CONFIGURATION
typedef struct
{
	uint8_t fragment:4; //LSB
	uint8_t fragmentTotal:4;//MSB
	uint8_t length;
}CONFIG_WRITE_HEADER_MESSAGE_t;

typedef struct
{
	uint8_t fragment:4; //LSB
	uint8_t fragmentTotal:4;//MSB
	uint8_t code;
}CONFIG_WRITE_RESPONSE_MESSAGE_t;

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
	uint8_t fragment:4; //LSB
	uint8_t fragmentTotal:4;//MSB
	uint8_t code;
}CONFIG_READ_CONFIRMATION_MESSAGE_t;

typedef struct
{
}CONFIG_CHECKSUM_MESSAGE_t;

typedef struct
{
	uint16_t checksum;
}CONFIG_CHECKSUM_RESPONSE_MESSAGE_t;


void configModule_Init(void);
void configModule_NotificationInd(uint8_t sender, OPERATION_HEADER_t* notification);

void configSystem_Handler(OPERATION_HEADER_t* operation_header);
void configWrite_Handler(OPERATION_HEADER_t* operation_header);
void configRead_Handler(OPERATION_HEADER_t* operation_header);
void configChecksum_Handler(OPERATION_HEADER_t* operation_header);

#endif /* CONFIG_MODULE_H_ */