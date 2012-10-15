/*
 * EEPROM.c
 *
 * Created: 12/10/2012 17:47:12
 *  Author: Victor
 */ 
#include <avr/eeprom.h>
#include "EEPROM.h"

uint8_t EEPROM_Read(int address)
{
	return eeprom_read_byte((unsigned char *) address);
}

void EEPROM_Write(int address, uint8_t value)
{
	eeprom_write_byte((unsigned char *) address, value);
}
