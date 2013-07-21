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

#define MAX_TEMHUM_DEVICES 4


typedef struct
{
	TEMHUM_CONFIG_t* config;
	uint8_t lastTemperature;
	uint8_t currentTemperature;
}TEMPERATURE_ELEM_t;

typedef struct
{
	TEMHUM_CONFIG_t* config;
	uint8_t lastHumidity;
	uint8_t currentHumidity;
}HUMIDITY_ELEM_t;

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

TEMPERATURE_ELEM_t temp_elems[MAX_TEMHUM_DEVICES];
uint8_t num_of_temp_elems;

HUMIDITY_ELEM_t hum_elems[MAX_TEMHUM_DEVICES];
uint8_t num_of_hum_elems;

DHT11_Read_t DHT11Result;

SYS_Timer_t sensorReadTimer;

static void sensorReadTimerHandler(SYS_Timer_t *timer);
uint8_t findTempElem(uint16_t deviceAddress);
uint8_t findHumElem(uint16_t deviceAddress);

void temHumModule_Init(void)
{
	TEMHUM_CONFIG_t* configTempPtr;
	TEMHUM_CONFIG_t* configHumPtr;
	
	num_of_temp_elems  = runningConfiguration.raw[runningConfiguration.topConfiguration.dinamicIndex.configModule_Temperature];		//First byte is number of configs
	num_of_hum_elems   = runningConfiguration.raw[runningConfiguration.topConfiguration.dinamicIndex.configModule_Humidity];		
	
	configTempPtr	   = &runningConfiguration.raw[runningConfiguration.topConfiguration.dinamicIndex.configModule_Temperature + 1];	//At second byte the list start
	
	if(num_of_temp_elems > MAX_TEMHUM_DEVICES)
	{
		//TODO: SEND ERROR (MAX NUMBER OF TEMP DEVICES EXCEEDED)
		num_of_temp_elems = MAX_TEMHUM_DEVICES;
	}
	
	if(num_of_hum_elems > MAX_TEMHUM_DEVICES)
	{
		//TODO: SEND ERROR (MAX NUMBER OF HUM DEVICES EXCEEDED)
		num_of_hum_elems = MAX_TEMHUM_DEVICES;
	}
	
	for(uint8_t i = 0; i<num_of_temp_elems; i++)
	{
		temp_elems[i].config = configTempPtr;
		
		configHumPtr = &runningConfiguration.raw[runningConfiguration.topConfiguration.dinamicIndex.configModule_Humidity + 1];	
		
		for(uint8_t i = 0; i<num_of_hum_elems; i++)
		{
			if(configTempPtr->pinAddress == configHumPtr->pinAddress)
			{
				hum_elems[i].config = configHumPtr;	
				break;
			}
			configHumPtr++;
		}
		
		configTempPtr++;
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
		uint8_t elemIndex = findTempElem(msg->deviceID);
		if(elemIndex != 0xFF)
		{
			temperatureResponse.header.destinationAddress = operation_header->sourceAddress;
			temperatureResponse.header.sourceAddress = runningConfiguration.topConfiguration.networkConfig.deviceAddress;
			temperatureResponse.response.deviceID = msg->deviceID;
			temperatureResponse.response.temperature = temp_elems[elemIndex].currentTemperature;
			
			OM_ProccessResponseOperation(&temperatureResponse.header);
		}
		else
		{
			//TODO: SEND ERROR (UNKNOWN SENSOR ADDRESS) USING ERROR RESPONSE INSTEAD
			
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
		uint8_t elemIndex = findHumElem(msg->deviceID);
		if(elemIndex != 0xFF)
		{
			humidityResponse.header.destinationAddress = operation_header->sourceAddress;
			humidityResponse.header.sourceAddress = runningConfiguration.topConfiguration.networkConfig.deviceAddress;
			humidityResponse.response.deviceID = msg->deviceID;
			humidityResponse.response.humidity = hum_elems[elemIndex].currentHumidity;
			
			OM_ProccessResponseOperation(&humidityResponse.header);
		}
		else
		{
			//TODO: SEND ERROR (UNKNOWN SENSOR ADDRESS) USING ERROR RESPONSE INSTEAD
			
			humidityResponse.header.destinationAddress = operation_header->sourceAddress;
			humidityResponse.header.sourceAddress = runningConfiguration.topConfiguration.networkConfig.deviceAddress;
			humidityResponse.response.deviceID = msg->deviceID;
			humidityResponse.response.humidity = 0xFF;
			
			OM_ProccessResponseOperation(&humidityResponse.header);
		}		
	}else if(operation_header->opCode == HumidityReadResponse)
	{
		//TODO: NOTIFICATION
	}
}

uint8_t findTempElem(uint16_t deviceAddress)
{
	for(uint8_t i = 0; i < num_of_temp_elems; i++)
	{
		if(temp_elems[i].config->deviceID == deviceAddress)
		return i;
	}
	
	return 0xFF;
}

uint8_t findHumElem(uint16_t deviceAddress)
{
	for(uint8_t i = 0; i < num_of_hum_elems; i++)
	{
		if(hum_elems[i].config->deviceID == deviceAddress)
		return i;
	}
	
	return 0xFF;
}

static void sensorReadTimerHandler(SYS_Timer_t *timer)
{	
	for(uint8_t i = 0; i < num_of_temp_elems; i++)
	{
		TEMPERATURE_ELEM_t* currentTempElem = &temp_elems[i];
		HUMIDITY_ELEM_t* currentHumElem = &hum_elems[i];
		CONFIG_MODULE_ELEM_HEADER_t* operationsInfoTemPtr = &currentTempElem->config->operationsInfo;
		CONFIG_MODULE_ELEM_HEADER_t* operationsInfoHumPtr = &currentHumElem->config->operationsInfo;
		
		if(DHT11_Read(currentTempElem->config->pinAddress, &DHT11Result))
		{
			if(currentTempElem->currentTemperature != DHT11Result.temperature)
			{
				currentTempElem->currentTemperature = DHT11Result.temperature;
				currentHumElem->currentHumidity = DHT11Result.humitity;				
				
				if(currentTempElem->currentTemperature != currentTempElem->lastTemperature)
				{
					CONFIG_MODULE_ELEM_HEADER_t* operationsInfoPtr = operationsInfoTemPtr;
					OPERATION_HEADER_t* operationPtr = &runningConfiguration.raw[operationsInfoTemPtr->pointerOperationList];
					for(int c = 0; c < operationsInfoPtr->numberOfOperations; c++)
					{
						OM_ProccessInternalOperation(operationPtr, false);
						operationPtr++;
					}
					
					currentTempElem->lastTemperature = currentTempElem->currentTemperature;
					
					//Send to coordinator
					temperatureResponse.header.destinationAddress = COORDINATOR_ADDRESS;
					temperatureResponse.header.sourceAddress = runningConfiguration.topConfiguration.networkConfig.deviceAddress;
					temperatureResponse.response.deviceID = currentTempElem->config->deviceID;
					temperatureResponse.response.temperature = currentTempElem->currentTemperature;
					
					OM_ProccessResponseOperation(&temperatureResponse.header);
				}
			}

			if(currentHumElem->currentHumidity != DHT11Result.humitity)
			{
				currentHumElem->currentHumidity = DHT11Result.humitity;
				
				if(currentHumElem->currentHumidity != currentHumElem->lastHumidity)
				{
					CONFIG_MODULE_ELEM_HEADER_t* operationsInfoPtr = operationsInfoHumPtr;
					OPERATION_HEADER_t* operationPtr = &runningConfiguration.raw[operationsInfoHumPtr->pointerOperationList];
					for(int c = 0; c < operationsInfoPtr->numberOfOperations; c++)
					{
						OM_ProccessInternalOperation(operationPtr, false);
						operationPtr++;
					}
					
					currentHumElem->lastHumidity = currentHumElem->currentHumidity;
					
					//Send to coordinator
					humidityResponse.header.destinationAddress = COORDINATOR_ADDRESS;
					humidityResponse.header.sourceAddress = runningConfiguration.topConfiguration.networkConfig.deviceAddress;
					humidityResponse.response.deviceID = currentHumElem->config->deviceID;
					humidityResponse.response.humidity = currentHumElem->currentHumidity;
					
					OM_ProccessResponseOperation(&humidityResponse.header);
				}
			}	
		}else
		{
			//TODO: SEND ERROR (SENSOR NOT DETECTED)
		}
	}
	
	(void)timer;
}