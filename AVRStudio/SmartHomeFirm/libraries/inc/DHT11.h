/*
 * DHT11.h
 *
 * Created: 18/05/2013 20:26:04
 *  Author: Victor
 */ 


#ifndef DHT11_H_
#define DHT11_H_

#include <sysTypes.h>

typedef struct 
{
	uint8_t temperature;	//temperature (0..50C)
	uint8_t humitity;		//humidity (20..90%)
}DHT11_Read_t;

/*
 * Get data from dht11
 * 
 * Return 1 if done, 0 otherwise
 */
_Bool DHT11_Read(uint16_t pinAddress, DHT11_Read_t* result);

#endif /* DHT11_H_ */