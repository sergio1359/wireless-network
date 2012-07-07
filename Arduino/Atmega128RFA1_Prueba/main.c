/*
 * main.c
 *
 *  Created on: 24/06/2012
 *      Author: Victor
 */

#include <stdlib.h>
#include <avr/io.h>

#define LED PB4

#define output_low(port,pin) port &= ~(1<<pin)
#define output_high(port,pin) port |= (1<<pin)
#define set_input(portdir,pin) portdir &= ~(1<<pin)
#define set_output(portdir,pin) portdir |= (1<<pin)

void delay_ms(uint8_t ms)
{
	uint16_t delay_count = F_CPU / 17500;
	volatile uint16_t i;
	while (ms != 0) {
	  for (i=0; i != delay_count; i++);
	  ms--;
	}
}

int main(void)
{
	// initialize the direction of PORTD #6 to be an output
	set_output(DDRB, LED);

	while (1) {
		// turn on the LED for 200ms
		output_high(PORTB, LED);
		delay_ms(200);
		// now turn off the LED for another 200ms
		output_low(PORTB, LED);
		delay_ms(200);
		// now start over
	}

	return 0;
}


