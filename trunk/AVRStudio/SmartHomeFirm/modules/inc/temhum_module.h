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

#define TEMHUM_MODULE_DEFINITION  X(TemHumModule, temHumModule_Init, temHumModule_NotificationInd)

#define COMMANDS_TABLE_TEMHUM \
X(TemperatureRead,				0x5A, temperatureRead_Handler,	temperatureRead_DataConf,	TEMPERATURE_READ_MESSAGE_t,				false)	\
X(TemperatureReadResponse,		0x5B, temperatureRead_Handler,	temperatureRead_DataConf,	TEMPERATURE_READ_RESPONSE_MESSAGE_t,	false)	\
X(HumidityRead,					0x5C, humidityRead_Handler,		humidityRead_DataConf,		HUMIDITY_READ_MESSAGE_t,				false)	\
X(HumidityReadResponse,			0x5D, humidityRead_Handler,		humidityRead_DataConf,		HUMIDITY_READ_RESPONSE_MESSAGE_t,		false)	\


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

void temperatureRead_Handler(OPERATION_HEADER_t* operation_header);
void humidityRead_Handler(OPERATION_HEADER_t* operation_header);

void temperatureRead_DataConf(OPERATION_DataConf_t *req);
void humidityRead_DataConf(OPERATION_DataConf_t *req);

#endif /* TEMHUM_MODULE_H_ */