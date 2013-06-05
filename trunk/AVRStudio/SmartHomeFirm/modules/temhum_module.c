/*
 * temhum_module.c
 *
 * Created: 01/06/2013 14:05:48
 *  Author: Victor
 */ 

#include "globals.h"
#include "modulesManager.h"
#include "portMonitor.h"

#include "DHT11.h"

#include "sysTimer.h"

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

DHT11_Read_t sensorResult;
uint8_t lastTemperature[MAX_SENSORS];
uint8_t lastHumidity[MAX_SENSORS];

SYS_Timer_t sensorReadTimer;

static void sensorReadTimerHandler(SYS_Timer_t *timer);

void temHumModule_Init(void)
{
	temperatureResponse.header.opCode = TemperatureReadResponse;
	humidityResponse.header.opCode = HumidityReadResponse;
	
	//TODO: READ CONFIG
	uint8_t* config_start_address = &runningConfiguration.raw[runningConfiguration.topConfiguration.dinamicIndex.configModule_TempHum];
	uint8_t num_of_configs = *config_start_address;
	
	if(num_of_configs > MAX_SENSORS)
	{
		//TODO: SEND ERROR (MAX NUMBER OF SENSORS EXCEEDED)
		num_of_configs = MAX_SENSORS;
	}
	
	for(num_of_sensors = 0; num_of_sensors<num_of_configs; num_of_sensors++)
	{
		config_start_address++;
		sensors[num_of_sensors] = *config_start_address;
 	}
	 
	sensorReadTimer.interval = 1000;
	sensorReadTimer.mode = SYS_TIMER_PERIODIC_MODE;
	sensorReadTimer.handler = sensorReadTimerHandler;
	SYS_TimerStart(&sensorReadTimer);
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
			temperatureResponse.response.temperature = lastTemperature[msg->addr];
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
			humidityResponse.response.humidity = lastHumidity[msg->addr];
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

static void sensorReadTimerHandler(SYS_Timer_t *timer)
{
	uint8_t currentValue = 0;
	for(uint8_t i = 0; i<num_of_sensors; i++)
	{
		if(DHT11_Read(sensors[i], &sensorResult))
		{
			if(lastTemperature[i] != sensorResult.temperature)
			{
				lastTemperature[i] = sensorResult.temperature;
				
				//TODO: HANDLE OPERATIONS
				//PortMonitor_LaunchOperations(sensors[i]);
			}

			if(lastHumidity[i] != sensorResult.humitity)
			{
				lastHumidity[i] = sensorResult.humitity;
				
				//TODO: HANDLE OPERATIONS
				//PortMonitor_LaunchOperations(sensors[i]);
			}	
		}else
		{
			//TODO: SEND ERROR (SENSOR NOT DETECTED)
		}
	}
	
	(void)timer;
}