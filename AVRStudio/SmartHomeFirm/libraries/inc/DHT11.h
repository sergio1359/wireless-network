/*
 * DHT11.h
 *
 * Created: 18/05/2013 20:26:04
 *  Author: Victor
 */ 


#ifndef DHT11_H_
#define DHT11_H_

#include <sysTypes.h>

#define DHT11_ERROR 255

uint8_t DHT11_ReadTemperature(uint16_t pinAddress);
uint8_t DHT11_ReadHumidity(uint16_t pinAddress);

#endif /* DHT11_H_ */