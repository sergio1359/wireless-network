/*
* INTERRUPT.c
*
* Created: 11/10/2013 21:05:52
*  Author: Victor
*/

#include "INTERRUPT.h"
#include "DIGITAL.h"

#include <avr/interrupt.h>

/*
 * [PIN : INTn]
 *
 * [PD0	: INT0]
 * [PD1	: INT1]
 * [PD2	: INT2]
 * [PD3	: INT3]
 * [PE4	: INT4]
 * [PE5	: INT5]
 * [PE6	: INT6]
 * [PE7 : INT7]
 */

static volatile void (*intFunc[EXTERNAL_NUM_INTERRUPTS])();

void INTERRUPT_Attach(uint8_t interruptNum, void (*userFunc)(void), INTRERRUPT_MODE_t mode)
{
	if(interruptNum < EXTERNAL_NUM_INTERRUPTS) {
		intFunc[interruptNum] = userFunc;
		
		// Configure the interrupt mode (trigger on low input, any change, rising
		// edge, or falling edge).  The mode constants were chosen to correspond
		// to the configuration bits in the hardware register, so we simply shift
		// the mode into place.
		
		// Set pin as input and enable the interrupt.
		switch (interruptNum) {
			case 0:
			HAL_GPIO_PD0_in();
			EICRA = (EICRA & ~((1 << ISC00) | (1 << ISC01))) | ((uint8_t)mode << ISC00);
			EIMSK |= (1 << INT0);
			break;
			case 1:
			HAL_GPIO_PD1_in();
			EICRA = (EICRA & ~((1 << ISC10) | (1 << ISC11))) | ((uint8_t)mode << ISC10);
			EIMSK |= (1 << INT1);
			break;
			case 2:
			HAL_GPIO_PD2_in();
			EICRA = (EICRA & ~((1 << ISC20) | (1 << ISC21))) | ((uint8_t)mode << ISC20);
			EIMSK |= (1 << INT2);
			break;
			case 3:
			HAL_GPIO_PD3_in();
			EICRA = (EICRA & ~((1 << ISC30) | (1 << ISC31))) | ((uint8_t)mode << ISC30);
			EIMSK |= (1 << INT3);
			break;
			case 4:
			HAL_GPIO_PE4_in();
			EICRB = (EICRB & ~((1 << ISC40) | (1 << ISC41))) | ((uint8_t)mode << ISC40);
			EIMSK |= (1 << INT4);
			break;
			case 5:
			HAL_GPIO_PE5_in();
			EICRB = (EICRB & ~((1 << ISC50) | (1 << ISC51))) | ((uint8_t)mode << ISC50);
			EIMSK |= (1 << INT5);
			break;
			case 6:
			HAL_GPIO_PE6_in();
			EICRB = (EICRB & ~((1 << ISC60) | (1 << ISC61))) | ((uint8_t)mode << ISC60);
			EIMSK |= (1 << INT6);
			break;
			case 7:
			HAL_GPIO_PE7_in();
			EICRB = (EICRB & ~((1 << ISC70) | (1 << ISC71))) | ((uint8_t)mode << ISC70);
			EIMSK |= (1 << INT7);
			break;
		}
	}
}

void INTERRUPT_Detach(uint8_t interruptNum)
{
	if(interruptNum < EXTERNAL_NUM_INTERRUPTS) {
		intFunc[interruptNum] = 0;
		
		// Disable the interrupt.
		switch (interruptNum) {
			case 0:
			EIMSK &= ~(1 << INT0);
			break;
			case 1:
			EIMSK &= ~(1 << INT1);
			break;
			case 2:
			EIMSK &= ~(1 << INT2);
			break;
			case 3:
			EIMSK &= ~(1 << INT3);
			break;
			case 4:
			EIMSK &= ~(1 << INT4);
			break;
			case 5:
			EIMSK &= ~(1 << INT5);
			break;
			case 6:
			EIMSK &= ~(1 << INT6);
			break;
			case 7:
			EIMSK &= ~(1 << INT7);
			break;
		}
	}	
}

ISR(INT0_vect) {
	if(intFunc[0])
	intFunc[0]();
}

ISR(INT1_vect) {
	if(intFunc[1])
	intFunc[1]();
}

ISR(INT2_vect) {
	if(intFunc[2])
	intFunc[2]();
}

ISR(INT3_vect) {
	if(intFunc[3])
	intFunc[3]();
}

ISR(INT4_vect) {
	if(intFunc[4])
	intFunc[4]();
}

ISR(INT5_vect) {
	if(intFunc[5])
	intFunc[5]();
}

ISR(INT6_vect) {
	if(intFunc[6])
	intFunc[6]();
}

ISR(INT7_vect) {
	if(intFunc[7])
	intFunc[7]();
}
