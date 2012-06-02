/*
 * storage.h
 *
 *  Created on: 23/05/2012
 *      Author: Victor
 */

#ifndef STORAGE_H_
#define STORAGE_H_

//#include "libraries/EEPROM/EEPROM.h"
#include <avr/eeprom.h>
#include "WConstants.h"
#include "global.h"

#define EEPROM_READ(x) eeprom_read_byte((unsigned char *) x)
#define EEPROM_WRITE(x, y) eeprom_write_byte((unsigned char *) x, y)

//Definitions of data address in the EEPROM Memory

#define SIGNATURE		 0x00 //4bytes
#define NETWORKADDRESS	 0x04 //4bytes
#define NODEADDRESS		 0x08 //1byte
#define MASTERADDRESS	 0x09 //1byte

class STORAGE {
public:
	void load(void);

	void save(void);

	void clear(void);

	bool check(void);
};

extern STORAGE Storage;

#endif /* STORAGE_H_ */
