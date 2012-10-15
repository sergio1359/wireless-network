/*
 * EEPROM.h
 *
 * Created: 12/10/2012 17:42:55
 *  Author: Victor
 */ 


#ifndef EEPROM_H_
#define EEPROM_H_

uint8_t EEPROM_Read(int address);
void EEPROM_Write(int address, uint8_t value);

#endif /* EEPROM_H_ */