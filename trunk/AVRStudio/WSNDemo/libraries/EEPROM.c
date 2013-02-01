/*
* EEPROM.c
*
* Created: 12/10/2012 17:47:12
*  Author: Victor
*/
#include "EEPROM.h"
#include "globals.h"


void EEPROM_Init(void)
{
	//Get the header only
	EEPROM_Read_Block(runningConfiguration, 0x00, sizeof(DEVICE_INFO_t));
	uint8_t eeprom_size = 50;//runningConfiguration.topConfiguration.deviceInfo.length;
	
	//Copy Startup-configuration to Running-configuration
	if(eeprom_size != 0xFF && eeprom_size != 0x00)
	{
		EEPROM_Read_Block(runningConfiguration.raw, 0x00, eeprom_size);	
	}
}

uint8_t inline EEPROM_Read_Byte(int address)
{
	return eeprom_read_byte((unsigned char *) address);
}

void inline EEPROM_Read_Block(void * buffer, const void * address, size_t length)
{
	eeprom_read_block(buffer, address, length);
}

void inline EEPROM_Write_Byte(int address, uint8_t value)
{
	eeprom_write_byte((unsigned char *) address, value);
}

void inline EEPROM_Write_Block(const void * buffer, const void * address, size_t length)
{
	eeprom_write_block(buffer, address, length);
}
