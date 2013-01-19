/*
 * ANALOG.c
 *
 * Created: 19/11/2012 1:20:09
 *  Author: Victor
 */ 
#include "ANALOG.h"

uint8_t analog_reference = REF_DEFAULT;

void ADC_Reference(uint8_t mode)
{
	// can't actually set the register here because the default setting
	// will connect AVCC and the AREF pin, which would cause a short if
	// there's something connected to AREF.
	analog_reference = mode;
}

unsigned int ADC_Read(uint8_t pin)
{
	uint8_t low, high;
	
	// set the analog reference (high two bits of ADMUX) and select the
	// channel (low 4 bits).  this also sets ADLAR (left-adjust result)
	// to 0 (the default).
	if(pin <8)
	{
		ADMUX = (analog_reference << 6) | (pin & 0x07);
	}else
	{
		ADMUX = (analog_reference << 6) | INTERNAL_TEMP;		
	}
	
	ADCSRA |= _BV(ADEN);

	// without a delay, we seem to read from the wrong channel
	HAL_Delay(20);

	// start the conversion
	ADCSRA |= _BV(ADSC) | _BV(ADPS2) | _BV(ADPS1) | _BV(ADPS0); // prescaler = 128;

	// ADSC is cleared when the conversion finishes
	while (bit_is_set(ADCSRA, ADSC));

	// we have to read ADCL first; doing so locks both ADCL
	// and ADCH until ADCH is read.  reading ADCL second would
	// cause the results of each conversion to be discarded,
	// as ADCL and ADCH would be locked when it completed.
	low = ADCL;
	high = ADCH;

	// combine the two bytes
	return (high << 8) | low;
	//return ADC;
}