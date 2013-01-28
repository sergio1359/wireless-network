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
	//for (uint8_t i = 0; i< 10; i++)
	//{
		//EEPROM_Write_Byte(i,i);
	//}
	
	
	//Copy Startup-configuration to Running-configuration
	//EEPROM_Read_Block(runningConfiguration.raw, 0x00, EEPROM_SIZE);
	EEPROM_Read_Block(runningConfiguration.raw, 0x00, 50);
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
