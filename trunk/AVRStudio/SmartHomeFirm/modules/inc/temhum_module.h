/*
 * temhum_module.h
 *
 * Created: 01/06/2013 14:05:35
 *  Author: Victor
 */ 


#ifndef TEMHUM_MODULE_H_
#define TEMHUM_MODULE_H_

#include <stdint.h>
#include <stdbool.h>
#include "EEPROM.h"

#define TEMHUM_MODULE_DEFINITION  X(TemHumModule, temHumModule_Init, temHumModule_NotificationInd)

#define COMMANDS_TABLE_TEMHUM \
X(TemperatureRead,				0x5A, temhumRead_Handler,		CONFIG_WRITE_HEADER_MESSAGE_t,			false)	\
X(TemperatureReadResponse,		0x5B, temhumRead_Handler,		CONFIG_READ_MESSAGE_t,					false)	\
X(HumidityRead,					0x5C, temhumRead_Handler,		CONFIG_READ_RESPONSE_HEADER_MESSAGE_t,	false)	\
X(HumidityReadResponse,			0x5D, temhumRead_Handler,		CONFIG_CHECKSUM_MESSAGE_t,				false)	\


//TEMPERATURE
typedef struct
{
	uint8_t addr;
}TEMPERATURE_READ_MESSAGE_t;

typedef struct
{
	uint8_t addr;
	uint8_t temperature;
}TEMPERATURE_READ_RESPONSE_MESSAGE_t;

//HUMIDITY
typedef struct
{
	uint8_t addr;
}HUMIDITY_READ_MESSAGE_t;

typedef struct
{
	uint8_t addr;
	uint8_t humidity;
}HUMIDITY_READ_RESPONSE_MESSAGE_t;



void temHumModule_Init(void);
void temHumModule_NotificationInd(uint8_t sender, OPERATION_HEADER_t* notification);

void temhumRead_Handler(OPERATION_HEADER_t* operation_header);

#endif /* TEMHUM_MODULE_H_ */