/*
 * temhum_module.c
 *
 * Created: 01/06/2013 14:05:48
 *  Author: Victor
 */ 
#include "modules.h"
#include "globals.h"

#define MAX_SENSORS 8

struct
{
	OPERATION_HEADER_t header;
	TEMPERATURE_READ_RESPONSE_MESSAGE_t response;
}temperatureResponse;

struct
{
	OPERATION_HEADER_t header;
	HUMIDITY_READ_RESPONSE_MESSAGE_t response;
}humidityResponse;

uint8_t sensors[MAX_SENSORS];
uint8_t num_of_sensors;

void temHumModule_Init(void)
{
	temperatureResponse.header.opCode = TemperatureReadResponse;
	humidityResponse.header.opCode = HumidityReadResponse;
	
	//TODO: READ CONFIG
	uint8_t* config_start_address = runningConfiguration.raw[runningConfiguration.topConfiguration.dinamicIndex.configModule_TempHum];
	uint8_t num_of_configs = *config_start_address;
	
	for(num_of_sensors = 0; num_of_sensors<num_of_configs; num_of_sensors++)
	{
		config_start_address++;
		sensors[num_of_sensors] = *config_start_address;
 	}
}

void temHumModule_NotificationInd(uint8_t sender, OPERATION_HEADER_t* notification)
{
	
}

void temhumRead_Handler(OPERATION_HEADER_t* operation_header)
{
	if(operation_header->opCode == TemperatureRead)
	{
		TEMPERATURE_READ_MESSAGE_t* msg = (TEMPERATURE_READ_MESSAGE_t*)(operation_header + 1);
		if(msg->addr<num_of_sensors)
		{
			temperatureResponse.header.destinationAddress = operation_header->sourceAddress;
			temperatureResponse.header.sourceAddress = runningConfiguration.topConfiguration.networkConfig.deviceAddress;
			temperatureResponse.response.addr = msg->addr;
			temperatureResponse.response.temperature = DHT11_ReadTemperature(sensors[msg->addr]);
		}
		else
		{
			//TODO: SEND ERROR (UNKNOWN SENSOR ADDRESS)
		}
	}else if(operation_header->opCode == TemperatureReadResponse)
	{
		//TODO: NOTIFICATION
	}else if(operation_header->opCode == HumidityRead)
	{
		HUMIDITY_READ_MESSAGE_t* msg = (HUMIDITY_READ_MESSAGE_t*)(operation_header + 1);
		if(msg->addr<num_of_sensors)
		{
			humidityResponse.header.destinationAddress = operation_header->sourceAddress;
			humidityResponse.header.sourceAddress = runningConfiguration.topConfiguration.networkConfig.deviceAddress;
			humidityResponse.response.addr = msg->addr;
			humidityResponse.response.humidity = DHT11_ReadHumidity(sensors[msg->addr]);
		}
		else
		{
			//TODO: SEND ERROR (UNKNOWN SENSOR ADDRESS)
		}		
	}else if(operation_header->opCode == HumidityReadResponse)
	{
		//TODO: NOTIFICATION
	}
}