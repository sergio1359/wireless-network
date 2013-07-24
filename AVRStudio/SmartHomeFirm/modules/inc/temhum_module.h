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

#define TEMHUM_MODULE_DEFINITION  X(TemHumModule, temHumModule_Init, temHumModule_DataConf, temHumModule_NotificationInd)

#define COMMANDS_TABLE_TEMHUM \
X(TemperatureRead,				0x5A, temhumRead_Handler,		TEMPERATURE_READ_MESSAGE_t,				false)	\
X(TemperatureReadResponse,		0x5B, temhumRead_Handler,		TEMPERATURE_READ_RESPONSE_MESSAGE_t,	false)	\
X(HumidityRead,					0x5C, temhumRead_Handler,		HUMIDITY_READ_MESSAGE_t,				false)	\
X(HumidityReadResponse,			0x5D, temhumRead_Handler,		HUMIDITY_READ_RESPONSE_MESSAGE_t,		false)	\


//TEMPERATURE
typedef struct
{
	uint16_t deviceID;
}TEMPERATURE_READ_MESSAGE_t;

typedef struct
{
	uint16_t deviceID;
	uint8_t temperature;
}TEMPERATURE_READ_RESPONSE_MESSAGE_t;

//HUMIDITY
typedef struct
{
	uint16_t deviceID;
}HUMIDITY_READ_MESSAGE_t;

typedef struct
{
	uint16_t deviceID;
	uint8_t humidity;
}HUMIDITY_READ_RESPONSE_MESSAGE_t;


//CONFIGURATION

typedef struct
{
	CONFIG_MODULE_ELEM_HEADER_t operationsInfo;
	uint16_t deviceID;
	uint8_t pinAddress;
}TEMHUM_CONFIG_t;


void temHumModule_Init(void);
void temHumModule_NotificationInd(uint8_t sender, OPERATION_HEADER_t* notification);
static void temHumModule_DataConf(NWK_DataReq_t *req);

void temhumRead_Handler(OPERATION_HEADER_t* operation_header);

#endif /* TEMHUM_MODULE_H_ */