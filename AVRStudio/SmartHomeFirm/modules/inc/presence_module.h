/*
 * presence_module.h
 *
 * Created: 22/07/2013 23:56:07
 *  Author: Victor
 */ 


#ifndef PRESENCE_MODULE_H_
#define PRESENCE_MODULE_H_

#include <stdint.h>
#include <stdbool.h>

#define PRESENCE_MODULE_DEFINITION  X(PresenceModule, presenceModule_Init, presenceModule_NotificationInd)

#define COMMANDS_TABLE_PRESENCE \
X(PresenceRead,				0x57, presenceRead_Handler,		PRESENCE_READ_MESSAGE_t,				false)	\
X(PresenceReadResponse,		0x58, presenceRead_Handler,		PRESENCE_READ_RESPONSE_MESSAGE_t,		false)	\

//PRESENCE
typedef struct
{
	uint16_t deviceID;
}PRESENCE_READ_MESSAGE_t;

typedef struct
{
	uint16_t deviceID;
	uint8_t detected;
}PRESENCE_READ_RESPONSE_MESSAGE_t;


//CONFIGURATION

typedef struct
{
	CONFIG_MODULE_ELEM_HEADER_t operationsInfo;
	uint16_t deviceID;
	uint8_t pinAddress;
	uint8_t sensibility;
}PRESENCE_CONFIG_t;


void presenceModule_Init(void);
void presenceModule_NotificationInd(uint8_t sender, OPERATION_HEADER_t* notification);

void presenceRead_Handler(OPERATION_HEADER_t* operation_header);

#endif /* PRESENCE_MODULE_H_ */