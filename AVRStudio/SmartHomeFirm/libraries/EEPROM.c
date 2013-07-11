/*
* EEPROM.c
*
* Created: 12/10/2012 17:47:12
*  Author: Victor
*/
#include "EEPROM.h"

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
