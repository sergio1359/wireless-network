/*
* EEPROM.c
*
* Created: 12/10/2012 17:47:12
*  Author: Victor
*/
#include "EEPROM.h"
#include "globals.h"
#include <util/crc16.h>

void EEPROM_Init(void)
{
	//Get the header only
	EEPROM_Read_Block(runningConfiguration.raw, 0x00, sizeof(DEVICE_INFO_t));
	uint16_t eeprom_size = runningConfiguration.topConfiguration.deviceInfo.length;
	uint16_t eeprom_crc = runningConfiguration.topConfiguration.deviceInfo.checkSum;
	
	if(eeprom_size >= EEPROM_SIZE)
	{
		//TODO:SEND ERROR MESSAGE (ERROR CONFIG SIZE TOO BIG)
		return;
	}		
	
	//Copy Startup-configuration to Running-configuration
	if(eeprom_size != 0xFFFF && eeprom_size != 0x00)
	{
		EEPROM_Read_Block(runningConfiguration.raw, 0x00, eeprom_size);	
		runningConfiguration.topConfiguration.deviceInfo.checkSum = 0;
		
		uint16_t acc = 0;
		for(int i = 0; i < eeprom_size; i++)
			acc = _crc16_update(acc, runningConfiguration.raw[i]);
		
		validConfiguration = eeprom_crc == acc;
		runningConfiguration.topConfiguration.deviceInfo.checkSum = eeprom_crc;
		
		if(!validConfiguration)
		{
			//TODO:SEND ERROR MESSAGE (ERROR EEPROM INVALID CHECKSUM)
		}
	}else
	{
		//TODO:SEND ERROR MESSAGE (ERROR EEPROM EMPTY OR CORRUPTED)
	}
}

uint8_t EEPROM_Read_Byte(int address)
{
	return eeprom_read_byte((unsigned char *) address);
}

void EEPROM_Read_Block(void * buffer, const void * address, size_t length)
{
	eeprom_read_block(buffer, address, length);
}

void EEPROM_Write_Byte(int address, uint8_t value)
{
	eeprom_write_byte((unsigned char *) address, value);
}

void EEPROM_Write_Block(void * buffer, const void * address, size_t length)
{
	eeprom_write_block(buffer, address, length);
}
