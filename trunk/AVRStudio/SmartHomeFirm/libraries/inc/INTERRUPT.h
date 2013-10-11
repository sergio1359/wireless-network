/*
 * INTERRUPT.h
 *
 * Created: 11/10/2013 21:03:38
 *  Author: Victor
 */ 


#ifndef INTERRUPT_H_
#define INTERRUPT_H_

#include <avr/io.h>

typedef enum
{
	CHANGE = 1,
	FALLING = 2,
	RISING = 3,
}INTRERRUPT_MODE_t;

#define EXTERNAL_NUM_INTERRUPTS 8

void INTERRUPT_Attach(uint8_t interruptNum, void (*userFunc)(void), INTRERRUPT_MODE_t mode);

void INTERRUPT_Detach(uint8_t interruptNum);

#endif /* INTERRUPT_H_ */