/*
 * configManager.c
 *
 * Created: 11/07/2013 21:13:39
 *  Author: Victor
 */ 

#include "configManager.h"
#include "globals.h"
#include <util/crc16.h>

void CONFIG_Init(void)
{
	//Get the header only
	EEPROM_Read_Block(runningConfiguration.raw, 0x00, sizeof(CONFIG_HEADER_t));
	uint16_t eeprom_size = runningConfiguration.topConfiguration.configHeader.length;
	uint16_t eeprom_crc = runningConfiguration.topConfiguration.configHeader.checkSum;
	
	if(eeprom_size >= EEPROM_SIZE)
	{
		//TODO:SEND ERROR MESSAGE (ERROR CONFIG SIZE TOO BIG)
		return;
	}
	
	//Copy Startup-configuration to Running-configuration
	if(eeprom_size != 0xFFFF && eeprom_size != 0x00)
	{
		EEPROM_Read_Block(runningConfiguration.raw, 0x00, eeprom_size);
		runningConfiguration.topConfiguration.configHeader.checkSum = 0;
		
		uint16_t acc = 0;
		for(int i = 0; i < eeprom_size; i++)
		acc = _crc16_update(acc, runningConfiguration.raw[i]);
		
		validConfiguration = eeprom_crc == acc;
		runningConfiguration.topConfiguration.configHeader.checkSum = eeprom_crc;
		
		if(!validConfiguration)
		{
			//TODO:SEND ERROR MESSAGE (ERROR EEPROM INVALID CHECKSUM)
		}
	}else
	{
		//TODO:SEND ERROR MESSAGE (ERROR EEPROM EMPTY OR CORRUPTED)
	}
}