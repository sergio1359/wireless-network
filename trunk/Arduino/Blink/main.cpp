//#include <WProgram.h>
#include <arduino_lit.h>

/*
  Blink
  Turns on an LED on for one second, then off for one second, repeatedly.
 
  This example code is in the public domain.
 */

int main(void)
{
	init();

	setup();

	for (;;)
		loop();

	return 0;
}

void setup() {                
  // initialize the digital pin as an output.
  // Pin 13 has an LED connected on most Arduino boards:
  PIN_MODE(13, OUTPUT);

  //serial_begin(57600);
}

inline void loop() {
  DIGITAL_WRITE(13, HIGH);   // set the LED on
  delay(1000);              // wait for a second
  DIGITAL_WRITE(13, LOW);    // set the LED off
  delay(1000); 				// wait for a second

  //PRINT("Done");
}
