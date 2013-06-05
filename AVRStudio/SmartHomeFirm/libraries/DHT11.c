/*
 * DHT11.c
 *
 * Created: 18/05/2013 20:26:14
 *  Author: Victor
 */ 

#include <util/delay.h>

#include "dht11.h"
#include "DIGITAL.h"

/*
#define DATA _PD0

#define CCAT(a, b, c) a ## b ## c
#define SET(x) CCAT(HAL_GPIO, x, _set())
#define CLR(x) CCAT(HAL_GPIO, x, _clr())
#define OUT(x) CCAT(HAL_GPIO, x, _out())
#define INP(x) CCAT(HAL_GPIO, x, _in())
#define READ(x) CCAT(HAL_GPIO, x, _read())
*/

VARPIN(DATA);

_Bool DHT11_Read(uint16_t pinAddress, DHT11_Read_t* result)
{
	uint8_t bits[5];
	uint8_t i,j = 0;
	
	VARPIN_UPDATE(DATA, pinAddress);

	memset(bits, 0, sizeof(bits));
	
	//Error Codes
	result->humitity = 255;
	result->temperature = 255;

	//send request
	VARPIN_OUT(DATA); //output
	VARPIN_CLR(DATA); //low
	_delay_ms(18);
	//VARPIN_SET(DATA); //high
	VARPIN_INP(DATA); //input
	VARPIN_SET(DATA); //high
	_delay_us(30);

	//check start condition 1
	if(VARPIN_READ(DATA)) {
		return 0;
	}
	_delay_us(80);
	//check start condition 2
	if(!VARPIN_READ(DATA)) {
		return 0;
	}
	_delay_us(80);

	//read the data
	for (j=0; j<5; j++) { //read 5 byte
		uint8_t result=0;
		for(i=0; i<8; i++) {//read every bit
			while(!VARPIN_READ(DATA)); //wait for an high input
			_delay_us(30);
			if(VARPIN_READ(DATA)) //if input is high after 30 us, get result
				result |= (1<<(7-i));
			while(VARPIN_READ(DATA)); //wait until input get low
		}
		bits[j] = result;
	}

	//reset port
	VARPIN_OUT(DATA); //output
	VARPIN_SET(DATA); //low

	//check checksum
	if (bits[0] + bits[1] + bits[2] + bits[3] == bits[4]) 
	{
		result->humitity = bits[0];
		result->temperature = bits[2];
		
		return 1;
	}else
	{
		return 0;	
	}
}
