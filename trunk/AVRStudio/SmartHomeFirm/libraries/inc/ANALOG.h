/*
* Analog.h
*
* Created: 17/11/2012 20:40:24
*  Author: Victor
*/


#ifndef ANALOG_H_
#define ANALOG_H_

#include "sysTypes.h"

/************************************************************************/
/*									ADC                                 */
/************************************************************************/
#define REF_EXTERNAL 		0 // Disables all internal references; Vref <= 1.8V
#define REF_DEFAULT			1 // Sets reference to AVDD ~ 1.8V
#define REF_INTERNAL_16		3 // Sets reference to 1 LSB precision 1.6V
#define REF_INTERNAL_15		2 // Sets reference to 1.5V

#define ADC0	0
#define ADC1	1
#define ADC2	2
#define ADC3	3
#define ADC4	4
#define ADC5	5
#define ADC6	6
#define ADC7	7
#define INTERNAL_TEMP 41

void ADC_Reference(uint8_t);
unsigned int ADC_Read(uint8_t);

/************************************************************************/
/*                                  PWM                                 */
/*	USE: pin -> (PWM0 - PWM7)		val -> (0 - 255)					*/
/*																		*/
/*	1- PWM_ENABLE(pin)	(once)											*/
/*	2- PWM_SET(pin, val)												*/
/*	3- PWM_DISABLE(pin) (If necessary)									*/
/************************************************************************/

/*
* NAME	PIN
*
* PWM0  PB4		OC2A //Slow due to external RTC crystal it's in use NOT WORKING!
* PWM1  PB5		OC1A
* PWM2  PB6		OC1B
* PWM3  PB7		OC1C
*
* PWM4  PE3		OC3A
* PWM5  PE4		OC3B
* PWM6  PE5		OC3C
*
* PWM7  PG5		OC0B //NOT WORKING
*
*/

#define PORT_AT_PIN_PWM0       PortB
#define PORT_AT_PIN_PWM1       PortB
#define PORT_AT_PIN_PWM2       PortB
#define PORT_AT_PIN_PWM3       PortB
#define PORT_AT_PIN_PWM4       PortE
#define PORT_AT_PIN_PWM5       PortE
#define PORT_AT_PIN_PWM6       PortE
#define PORT_AT_PIN_PWM7       PortG

#define PORTMSK_AT_PIN_PWM0     _BV( 4 )
#define PORTMSK_AT_PIN_PWM1     _BV( 5 )
#define PORTMSK_AT_PIN_PWM2     _BV( 6 )
#define PORTMSK_AT_PIN_PWM3     _BV( 7 )
#define PORTMSK_AT_PIN_PWM4     _BV( 3 )
#define PORTMSK_AT_PIN_PWM5     _BV( 4 )
#define PORTMSK_AT_PIN_PWM6     _BV( 5 )
#define PORTMSK_AT_PIN_PWM7     _BV( 5 )

////////////PWM pin to Timer Regs mapping
#define TIMER_AT_PIN_PWM0   2A
#define TCCR_AT_PIN_PWM0    TCCR2

#define TIMER_AT_PIN_PWM1   1A
#define TCCR_AT_PIN_PWM1    TCCR1

#define TIMER_AT_PIN_PWM2   1B
#define TCCR_AT_PIN_PWM2    TCCR1

#define TIMER_AT_PIN_PWM3   1C
#define TCCR_AT_PIN_PWM3    TCCR1

#define TIMER_AT_PIN_PWM4   3A
#define TCCR_AT_PIN_PWM4    TCCR3

#define TIMER_AT_PIN_PWM5   3B
#define TCCR_AT_PIN_PWM5    TCCR3

#define TIMER_AT_PIN_PWM6   3C
#define TCCR_AT_PIN_PWM6    TCCR3

#define TIMER_AT_PIN_PWM7   0B
#define TCCR_AT_PIN_PWM7    TCCR0

////////////PORT to DDRX mapping
#define DIR_REG_AT_PortB DDRB
#define DIR_REG_AT_PortE DDRE
#define DIR_REG_AT_PortG DDRG


#define PIN_TO_TIMERID(x) TIMER_AT_PIN_##x
#define PIN_TO_TCCRID(x) TCCR_AT_PIN_##x

#define PIN_TO_PORTID(x) PORT_AT_PIN_##x
#define PIN_TO_PORTMSK(x) PORTMSK_AT_PIN_##x

#define PORTID_TO_DIR_REG(x) DIR_REG_AT_##x

/************************************************************************/
/*                                                                      */
/************************************************************************/

#define MERGE_TO_FUNC(prefix, id)   prefix##_##id
#define EXPAND_WRAPPER( NEXTLEVEL, ...)  NEXTLEVEL( __VA_ARGS__ )

#define _PWM_SET(id, val)   OCR##id = val            

#define _PWM_ENABLE(TCCR, id)				\
TCCR##A |= _BV(COM##id##1) | _BV(WGM30);	\
TCCR##B |= _BV(WGM32) | _BV(CS30)

#define _PWM_DISABLE(TCCR, id)				\
TCCR##A |= ~(_BV(COM##id##1) | _BV(WGM30));	\
TCCR##B |= ~(_BV(WGM32) | _BV(CS30))

#define _SET_OUTPUT(port_id, msk)  PORTID_TO_DIR_REG(port_id) |= (msk)
#define _SET_INTPUT(port_id, msk)  PORTID_TO_DIR_REG(port_id) &= ~(msk)

#define PWM_DISABLE(pin)        EXPAND_WRAPPER(_PWM_DISABLE ,PIN_TO_TCCRID(pin) , PIN_TO_TIMERID(pin) ) 
#define PWM_SET(pin, val)       EXPAND_WRAPPER(_PWM_SET, PIN_TO_TIMERID(pin), val )               
	
#define PWM_ENABLE(pin)													\
EXPAND_WRAPPER(_SET_OUTPUT, PIN_TO_PORTID(pin), PIN_TO_PORTMSK(pin));	\
EXPAND_WRAPPER(_PWM_ENABLE, PIN_TO_TCCRID(pin), PIN_TO_TIMERID(pin))
	
#endif /* ANALOG_H_ */