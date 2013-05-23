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

/*
 * get data from dht11
 */
uint8_t getdata(uint8_t select) {
	uint8_t bits[5];
	uint8_t i,j = 0;

	memset(bits, 0, sizeof(bits));

	//reset port
	VARPIN_OUT(DATA); //output
	VARPIN_SET(DATA); //high
	_delay_ms(200);

	//send request
	VARPIN_CLR(DATA); //low
	_delay_ms(18);
	_delay_ms(5);
	VARPIN_SET(DATA); //high
	_delay_us(1);
	VARPIN_INP(DATA); //input
	VARPIN_SET(DATA); //high
	_delay_us(40);

	//check start condition 1
	if(VARPIN_READ(DATA)) {
		return DHT11_ERROR;
	}
	_delay_us(80);
	//check start condition 2
	if(!VARPIN_READ(DATA)) {
		return DHT11_ERROR;
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
	VARPIN_CLR(DATA); //low

	//check checksum
	if (bits[0] + bits[1] + bits[2] + bits[3] == bits[4]) {
		if (select == 0) { //return temperature
			return(bits[2]);
		} else if(select == 1){ //return humidity
			return(bits[0]);
		}
	}

	return DHT11_ERROR;
}

/*
 * get temperature (0..50C)
 */
uint8_t DHT11_ReadTemperature(uint16_t pinAddress) {
	VARPIN_UPDATE(DATA, pinAddress);
	return getdata(0);
}

/*
 * get humidity (20..90%)
 */
uint8_t DHT11_ReadHumidity(uint16_t pinAddress) {
	VARPIN_UPDATE(DATA, pinAddress);
	return getdata(1);
}