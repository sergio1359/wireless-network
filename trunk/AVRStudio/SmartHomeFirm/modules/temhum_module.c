/*
 * temhum_module.c
 *
 * Created: 01/06/2013 14:05:48
 *  Author: Victor
 */ 

#include "globals.h"
#include "modulesManager.h""

#include "DHT11.h"

#include "sysTimer.h"

#define MAX_TEMHUM_DEVICES 8


typedef struct
{
	TEMHUM_CONFIG_t* config;
	uint8_t lastTemperature;
	uint8_t lastHumidity;
	uint8_t currentTemperature;
	uint8_t currentHumidity;
}TEMHUM_ELEM_t;

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

TEMHUM_ELEM_t temhum_elems[MAX_TEMHUM_DEVICES];
uint8_t num_of_temhum_elems;

DHT11_Read_t DHT11Result;

SYS_Timer_t sensorReadTimer;

static void sensorReadTimerHandler(SYS_Timer_t *timer);
uint8_t findTemHumElem(uint16_t deviceAddress);

void temHumModule_Init(void)
{
	TEMHUM_CONFIG_t* configPtr;
	
	num_of_temhum_elems = runningConfiguration.raw[runningConfiguration.topConfiguration.dinamicIndex.configModule_TempHum];			//First byte is number of configs
	configPtr		   = &runningConfiguration.raw[runningConfiguration.topConfiguration.dinamicIndex.configModule_TempHum + 1];		//At second byte the list start
	
	if(num_of_temhum_elems > MAX_TEMHUM_DEVICES)
	{
		//TODO: SEND ERROR (MAX NUMBER OF DEVICES EXCEEDED)
		num_of_temhum_elems = MAX_TEMHUM_DEVICES;
	}
	
	for(uint8_t i = 0; i<num_of_temhum_elems; i++)
	{
		if(configPtr->sensibilityHumidity == 0)
		{
			//TODO: SEND ERROR (INVALID SENSIBILITY VALUE)
			configPtr->sensibilityHumidity = 1;
		}
		
		if(configPtr->sensibilityTemperature == 0)
		{
			//TODO: SEND ERROR (INVALID SENSIBILITY VALUE)
			configPtr->sensibilityTemperature = 1;
		}
		
		temhum_elems[i].config = configPtr;
		configPtr++;
 	}
	 
	//Set responses opCodes
	temperatureResponse.header.opCode = TemperatureReadResponse;
	humidityResponse.header.opCode = HumidityReadResponse; 
	 
	//Configure Timer 
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
		uint8_t elemIndex = findTemHumElem(msg->deviceID);
		if(elemIndex != 0xFF)
		{
			temperatureResponse.header.destinationAddress = operation_header->sourceAddress;
			temperatureResponse.header.sourceAddress = runningConfiguration.topConfiguration.networkConfig.deviceAddress;
			temperatureResponse.response.deviceID = msg->deviceID;
			temperatureResponse.response.temperature = temhum_elems[elemIndex].currentTemperature;
			
			OM_ProccessResponseOperation(&temperatureResponse.header);
		}
		else
		{
			//TODO: SEND ERROR (UNKNOWN SENSOR ADDRESS)
			
			temperatureResponse.header.destinationAddress = operation_header->sourceAddress;
			temperatureResponse.header.sourceAddress = runningConfiguration.topConfiguration.networkConfig.deviceAddress;
			temperatureResponse.response.deviceID = msg->deviceID;
			temperatureResponse.response.temperature = 0xFF;
			
			OM_ProccessResponseOperation(&temperatureResponse.header);
		}
	}else if(operation_header->opCode == TemperatureReadResponse)
	{
		//TODO: NOTIFICATION
	}else if(operation_header->opCode == HumidityRead)
	{
		HUMIDITY_READ_MESSAGE_t* msg = (HUMIDITY_READ_MESSAGE_t*)(operation_header + 1);
		uint8_t elemIndex = findTemHumElem(msg->deviceID);
		if(elemIndex != 0xFF)
		{
			humidityResponse.header.destinationAddress = operation_header->sourceAddress;
			humidityResponse.header.sourceAddress = runningConfiguration.topConfiguration.networkConfig.deviceAddress;
			humidityResponse.response.deviceID = msg->deviceID;
			humidityResponse.response.humidity = temhum_elems[elemIndex].currentHumidity;
			
			OM_ProccessResponseOperation(&humidityResponse.header);
		}
		else
		{
			//TODO: SEND ERROR (UNKNOWN SENSOR ADDRESS)
			
			humidityResponse.header.destinationAddress = operation_header->sourceAddress;
			humidityResponse.header.sourceAddress = runningConfiguration.topConfiguration.networkConfig.deviceAddress;
			humidityResponse.response.deviceID = msg->deviceID;
			humidityResponse.response.humidity = 0xFF;
			
			OM_ProccessResponseOperation(&temperatureResponse.header);
		}		
	}else if(operation_header->opCode == HumidityReadResponse)
	{
		//TODO: NOTIFICATION
	}
}

uint8_t findTemHumElem(uint16_t deviceAddress)
{
	for(uint8_t i = 0; i < num_of_temhum_elems; i++)
	{
		if(temhum_elems[i].config->deviceID == deviceAddress)
		return i;
	}
	
	return 0xFF;
}

static void sensorReadTimerHandler(SYS_Timer_t *timer)
{	
	for(uint8_t i = 0; i < num_of_temhum_elems; i++)
	{
		TEMHUM_ELEM_t* currentElem = &temhum_elems[i];
		CONFIG_MODULE_ELEM_HEADER_t* operationsInfoTemPtr = &currentElem->config->operationsInfoTemperature;
		CONFIG_MODULE_ELEM_HEADER_t* operationsInfoHumPtr = &currentElem->config->operationsInfoHumidity;
		
		if(DHT11_Read(currentElem->config->pinAddress, &DHT11Result))
		{
			if(currentElem->currentTemperature != DHT11Result.temperature)
			{
				currentElem->currentTemperature = DHT11Result.temperature;				
				
				if(abs(currentElem->currentTemperature - currentElem->lastTemperature) > currentElem->config->sensibilityTemperature)
				{
					CONFIG_MODULE_ELEM_HEADER_t* operationsInfoPtr = &currentElem->config->operationsInfoTemperature;
					OPERATION_HEADER_t* operationPtr = &runningConfiguration.raw[operationsInfoPtr->pointerOperationList];
					for(int c = 0; c < operationsInfoPtr->numberOfOperations; c++)
					{
						OM_ProccessInternalOperation(operationPtr, false);
						operationPtr++;
					}
				}
			}

			if(currentElem->currentHumidity != DHT11Result.humitity)
			{
				currentElem->currentHumidity = DHT11Result.humitity;
				
				if(abs(currentElem->currentHumidity - currentElem->lastHumidity) > currentElem->config->sensibilityHumidity)
				{
					CONFIG_MODULE_ELEM_HEADER_t* operationsInfoPtr = &currentElem->config->operationsInfoHumidity;
					OPERATION_HEADER_t* operationPtr = &runningConfiguration.raw[operationsInfoPtr->pointerOperationList];
					for(int c = 0; c < operationsInfoPtr->numberOfOperations; c++)
					{
						OM_ProccessInternalOperation(operationPtr, false);
						operationPtr++;
					}
				}
			}	
		}else
		{
			//TODO: SEND ERROR (SENSOR NOT DETECTED)
		}
	}
	
	(void)timer;
}