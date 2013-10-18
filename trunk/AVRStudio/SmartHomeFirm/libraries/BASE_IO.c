/*
 * BASE_IO.c
 *
 * Created: 29/07/2013 19:57:40
 *  Author: Victor
 */ 
#include "BASE_IO.h"
#include "DIGITAL.h"
#include "globals.h"

VARPIN(LED);
VARPIN(BUTTON);

void BASE_Init()
{
	if(DS2401_Init())
	{
		//BASE V2
		baseModel = ATMega128RFA1_V2;
		
		VARPIN_UPDATE(LED, PINADDRESS('D', 6));
		VARPIN_UPDATE(BUTTON, PINADDRESS('D', 7));
		
		VARPIN_OUT(LED);
		VARPIN_INP(BUTTON);
	}else
	{
		//BASE V1
		baseModel = ATMega128RFA1_V1;
		
		VARPIN_UPDATE(LED, PINADDRESS('B', 4));
		
		VARPIN_OUT(LED);
	}
	
	shieldModel = DEBUG_SHIELD;	
}

void BASE_LedOn()
{
	VARPIN_SET(LED);
}

void BASE_LedOff()
{
	VARPIN_CLR(LED);
}

void BASE_LedToggle()
{
	VARPIN_TOG(LED);
}

_Bool BASE_ButtonPressed()
{
	return VARPIN_ISSET(BUTTON) && !VARPIN_READ(BUTTON);
}

uint8_t BASE_PinAddressToINT(uint8_t pinAddress)
{
	switch(pinAddress)
	{
		case PINADDRESS('D', 0):
		return 0;
		
		case PINADDRESS('D', 1):
		return 1;
		
		case PINADDRESS('D', 2):
		return 2;
		
		case PINADDRESS('D', 3):
		return 3;
		
		case PINADDRESS('E', 4):
		return 4;
		
		case PINADDRESS('E', 5):
		return 5;
		
		case PINADDRESS('E', 6):
		return 6;
		
		case PINADDRESS('E', 7):
		return 7;
		
		default:
		return 255;
	}
}
