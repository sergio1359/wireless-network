/*
 * BASE_IO.c
 *
 * Created: 29/07/2013 19:57:40
 *  Author: Victor
 */ 
#include "BASE_IO.h"
#include "DIGITAL.h"
#include "globals.h"

#define SHIELD_MODEL 1

VARPIN(LED);
VARPIN(BUTTON);

void BASE_Init()
{
	if(DS2401_Init())
	{
		//BASE V2
		baseModel = 2;
		
		VARPIN_UPDATE(LED, PINADDRESS('D', 6));
		VARPIN_UPDATE(BUTTON, PINADDRESS('D', 7));
		
		VARPIN_OUT(LED);
		VARPIN_INP(BUTTON);
	}else
	{
		//BASE V1
		baseModel = 1;
		
		VARPIN_UPDATE(LED, PINADDRESS('B', 4));
		
		VARPIN_OUT(LED);
	}
	
	shieldModel = SHIELD_MODEL;	
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
