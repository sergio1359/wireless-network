/*
 * storage.cpp
 *
 *  Created on: 23/05/2012
 *      Author: Victor
 */

#include "storage.h"

uint8_t SIGNATUREV[4] = { 1 , 3, 3, 7};

uint32_t NetworkAddress = 0x01234567;
#if defined(UNO)
uint8_t nodeAddress = 0x01;
uint8_t masterAddress = 0x02;
#elif defined(DUEMILANOVE)
uint8_t nodeAddress = 0x02;
uint8_t masterAddress = 0x01;
#else
uint8_t nodeAddress = 0x03;
uint8_t masterAddress = 0x01;
#endif

STORAGE Storage;

void STORAGE::load()
{
	int i;

	NetworkAddress = 0;

	for(i=0; i<4; i++)
	{
		NetworkAddress |= (uint32_t)EEPROM_READ(NETWORKADDRESS+i) << (i*8);
	}

	nodeAddress = EEPROM_READ(NODEADDRESS);
	masterAddress = EEPROM_READ(MASTERADDRESS);
}

void STORAGE::save()
{
	int i;

	for(i=0; i<4; i++)
		EEPROM_WRITE(SIGNATURE+i, SIGNATUREV[i]);

	for(i=0; i<4; i++)
		EEPROM_WRITE(NETWORKADDRESS+i, NetworkAddress >> (i*8));

	EEPROM_WRITE(NODEADDRESS, nodeAddress);
	EEPROM_WRITE(MASTERADDRESS, masterAddress);
}

void STORAGE::clear()
{
	for(int i=0;i<256;i++)
		EEPROM_WRITE(i,0);
}

bool STORAGE::check()
{
	return EEPROM_READ(SIGNATURE) == SIGNATUREV[0] &&
			EEPROM_READ(SIGNATURE + 1) == SIGNATUREV[1] &&
			EEPROM_READ(SIGNATURE + 2) == SIGNATUREV[2] &&
			EEPROM_READ(SIGNATURE + 3) == SIGNATUREV[3];
}


