/*
* configManager.c
*
* Created: 11/07/2013 21:13:39
*  Author: Victor
*/

#include "configManager.h"
#include "timeManager.h"
#include "globals.h"
#include <util/crc16.h>

void CONFIG_Init(void)
{
	//Get the header only
	EEPROM_Read_Block(runningConfiguration.raw, 0x00, sizeof(CONFIG_HEADER_t));
	uint16_t eeprom_size = runningConfiguration.topConfiguration.configHeader.length;
	uint16_t eeprom_crc = runningConfiguration.topConfiguration.configHeader.checkSum;
	
	uint8_t base_model = runningConfiguration.topConfiguration.configHeader.baseModel;
	uint8_t shield_model = runningConfiguration.topConfiguration.configHeader.shieldModel;
	uint8_t firmware_version = runningConfiguration.topConfiguration.configHeader.firmVersion;
	
	_Bool can_be_read = 1;
	
	//Check size errors
	if(eeprom_size == 0x00 || eeprom_size == 0xFFFF)
	{
		//TODO:SEND ERROR MESSAGE (ERROR EEPROM EMPTY OR CORRUPTED)
		can_be_read = 0;
	}else if(eeprom_size > CONFIG_MAX_SIZE)
	{
		//TODO:SEND ERROR MESSAGE (ERROR CONFIG SIZE TOO BIG)
		can_be_read = 0;
	}else
	{
		if(!IS_TEMPORAL_CONFIG)
		{
			if (firmware_version != FIRMWARE_VERSION)
			{
				//TODO:SEND ERROR MESSAGE (ERROR CONFIG INVALID FIRMWARE VERSION)
				can_be_read = 0;
			}
			else if(base_model != baseModel)
			{
				//TODO:SEND ERROR MESSAGE (ERROR CONFIG INVALID BASE MODEL)
				can_be_read = 0;
			}else if (shield_model != shieldModel)
			{
				//TODO:SEND ERROR MESSAGE (ERROR CONFIG INVALID SHIELD MODEL)
				can_be_read = 0;
			}
		}
	}
	
	if(can_be_read)
	{
		//Copy Startup-configuration to Running-configuration
		EEPROM_Read_Block(runningConfiguration.raw, 0x00, eeprom_size);
		runningConfiguration.topConfiguration.configHeader.checkSum = 0;
		
		uint16_t acc = 0;
		for(int i = 0; i < eeprom_size; i++)
		acc = _crc16_update(acc, runningConfiguration.raw[i]);
		
		validConfiguration = (eeprom_crc == acc) && !IS_TEMPORAL_CONFIG;
		runningConfiguration.topConfiguration.configHeader.checkSum = eeprom_crc;
		
		if(eeprom_crc != acc)
		{
			//TODO:SEND ERROR MESSAGE (ERROR EEPROM INVALID CHECKSUM)
		}
	}
}

void CONFIG_SaveTemporalConfig()
{
	uint16_t eeprom_size = sizeof(CONFIG_HEADER_t) + sizeof(NETWORK_CONFIG_t);
	uint16_t acc = 0;
	
	runningConfiguration.topConfiguration.configHeader.checkSum = 0;
	runningConfiguration.topConfiguration.configHeader.firmVersion = 0xFF;
	runningConfiguration.topConfiguration.configHeader.length = eeprom_size;
	runningConfiguration.topConfiguration.configHeader.systemFlags.raw = 0;
	
	if(VALID_DATETIME)
	{
		runningConfiguration.topConfiguration.configHeader.updateDate = currentDate;
		runningConfiguration.topConfiguration.configHeader.updateTime = currentTime;
		runningConfiguration.topConfiguration.configHeader.updateWeekDay = currentWeek;
	}
	
	//Compute checksum
	for(int i = 0; i < eeprom_size; i++)
	acc = _crc16_update(acc, runningConfiguration.raw[i]);
	
	runningConfiguration.topConfiguration.configHeader.checkSum = acc;
	
	EEPROM_Write_Block(&runningConfiguration, 0x00, eeprom_size);
}

uint16_t CONFIG_GetOperationConfigAddress(OPERATION_HEADER_t* operation_header)
{
	return (uint16_t)operation_header - (uint16_t)&runningConfiguration;
}