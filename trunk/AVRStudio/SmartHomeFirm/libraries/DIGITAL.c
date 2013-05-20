/*
 * DIGITAL.c
 *
 * Created: 19/05/2013 12:47:08
 *  Author: Victor
 */ 
#include "DIGITAL.h"


/* EXAMPLE
 * 
 * HAL_GPIO_PORT_set(PINA, 3)  -> Set PINA3
 */


#define PIN_REG(portPtr)  *(portPtr - 2)
#define DDR_REG(portPtr)  *(portPtr - 1)
#define PORT_REG(portPtr) *(portPtr)

void HAL_GPIO_PORT_set(uint8_t* portPtr, uint8_t mask)
{
	PORT_REG(portPtr) |= mask;
}

void HAL_GPIO_PORT_clr(uint8_t* portPtr, uint8_t mask)
{
	PORT_REG(portPtr) &= ~mask;
}

void HAL_GPIO_PORT_toggle(uint8_t* portPtr, uint8_t mask)
{
	PORT_REG(portPtr) ^= mask;	
}

void HAL_GPIO_PORT_in(uint8_t* portPtr, uint8_t mask)
{
	PORT_REG(portPtr) &= ~mask;
	PORT_REG(portPtr) &= ~mask;
}

void HAL_GPIO_PORT_out(uint8_t* portPtr, uint8_t mask)
{
	DDR_REG(portPtr) |= mask;
}

void HAL_GPIO_PORT_pullup(uint8_t* portPtr, uint8_t mask)
{
	PORT_REG(portPtr) |= mask;
}

uint8_t	HAL_GPIO_PORT_read(uint8_t* portPtr, uint8_t mask)
{
	return PIN_REG(portPtr) & mask;
}

uint8_t	HAL_GPIO_PORT_state(uint8_t* portPtr, uint8_t mask)
{
	return DDR_REG(portPtr) & mask;
}