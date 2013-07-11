/*
 * EEPROM.h
 *
 * Created: 12/10/2012 17:42:55
 *  Author: Victor
 */ 


#ifndef EEPROM_H_
#define EEPROM_H_

#include <avr/eeprom.h>

extern inline uint8_t EEPROM_Read_Byte(int address);
extern inline void EEPROM_Read_Block(void * buffer, const void * address, size_t length);
extern inline void EEPROM_Write_Byte(int address, uint8_t value);
extern inline void EEPROM_Write_Block(void * buffer, const void * address, size_t length);

#endif /* EEPROM_H_ */