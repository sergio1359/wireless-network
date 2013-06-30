/*
 * EEPROM.h
 *
 * Created: 12/10/2012 17:42:55
 *  Author: Victor
 */ 


#ifndef EEPROM_H_
#define EEPROM_H_

#include "RTC.h"
#include <avr/eeprom.h>
#include "config.h"


#define TIME_OPERATION_LIST_START_ADDRESS				runningConfiguration.topConfiguration.dinamicIndex.timeOperationList
#define OPERATION_LIST_START_ADDR						runningConfiguration.topConfiguration.dinamicIndex.operationList
#define OPERATION_TIME_RESTRIC_LIST_START_ADDRESS		runningConfiguration.topConfiguration.dinamicIndex.operationTimeRestrictionList
#define OPERATION_COND_RESTRIC_LIST_START_ADDRESS		runningConfiguration.topConfiguration.dinamicIndex.operationConditionRestrictionList
#define FREE_REGION_START_ADDRESS						runningConfiguration.topConfiguration.dinamicIndex.freeRegion

#define TIME_OPERATION_LIST_END_ADDRESS					OPERATION_LIST_START_ADDR
#define OPERATION_LIST_END_ADDR							OPERATION_TIME_RESTRIC_LIST_START_ADDRESS
#define OPERATION_TIME_RESTRIC_LIST_END_ADDRESS			OPERATION_COND_RESTRIC_LIST_START_ADDRESS						
#define OPERATION_COND_RESTRIC_LIST_END_ADDRESS			FREE_REGION_START_ADDRESS

#define NO_EDGE			0
#define FALLING_EDGE	1
#define RISIN_EDGE		2
#define BOTH_EDGE		3

typedef struct{
	unsigned ledDebug : 1; //LSB
	unsigned UARTDebug : 1;
	unsigned batteryInstalled : 1;
	unsigned reserved : 5; //MSB
}SYSTEM_FLAGS_t;

typedef struct{
	uint8_t firmVersion;	//Desired firmware version for the current config
	uint16_t length;		//Size of the current config
	uint16_t checkSum;		//Crc16
	DATE_t updateDate;
	TIME_t updateTime;
	SYSTEM_FLAGS_t systemFlags;
}CONFIG_HEADER_t;

typedef struct{
	uint16_t deviceAddress;
	uint8_t channel;
	uint16_t panId;
	uint8_t securityKey[16];
	uint8_t networkRetries;
}NETWORK_CONFIG_t;


typedef struct{
	uint16_t configModule_Logic;
	uint16_t configModule_RGB;
	uint16_t configModule_Presence;
	uint16_t configModule_TempHum;
	uint16_t configModule_EnergyMeter;
	uint16_t configModule_Luminosity;
	uint16_t timeOperationList;
	uint16_t operationList;
	uint16_t operationTimeRestrictionList;
	uint16_t operationConditionRestrictionList;
	uint16_t freeRegion;
}DINAMIC_INDEX_t;

typedef struct{
	uint16_t sourceAddress;
	uint16_t destinationAddress;
	uint8_t opCode;
}OPERATION_HEADER_t;

typedef struct{
	TIME_t activationTime;				  //Activation time
	OPERATION_HEADER_t operationHeader;   //Header of the operation to send on activation
}TIME_OPERATION_HEADER_t;

typedef struct{
	uint16_t operationAddress;
	DATE_t dateFrom;
	DATE_t dateTo;
	WEEKDAY_t weekDays;
	TIME_t start;
	TIME_t end;
}OPERATION_RESTRICTION_t;


typedef struct {
	CONFIG_HEADER_t configHeader;
	NETWORK_CONFIG_t networkConfig;
	DINAMIC_INDEX_t dinamicIndex;
}TOP_CONFIGURATION_t;

typedef union
{
	TOP_CONFIGURATION_t  topConfiguration;
	uint8_t raw[EEPROM_SIZE];
}RUNNING_CONFIGURATION_t;

void EEPROM_Init(void);

extern inline uint8_t EEPROM_Read_Byte(int address);
extern inline void EEPROM_Read_Block(void * buffer, const void * address, size_t length);
extern inline void EEPROM_Write_Byte(int address, uint8_t value);
extern inline void EEPROM_Write_Block(void * buffer, const void * address, size_t length);

#endif /* EEPROM_H_ */