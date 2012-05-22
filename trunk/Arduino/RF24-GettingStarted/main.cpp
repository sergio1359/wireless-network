#include <WProgram.h>
#include <arduino.h>
#include "libraries/SPI/SPI.h"
#include <SPI.h>
#include "libraries/RF24/RF24.h"

int main(void) {
	init();

	setup();

	for (;;)
		loop();

	return 0;
}

//
// Hardware configuration
//

// Set up nRF24L01 radio on SPI bus plus pins 9 & 10

RF24 Radio(9, 10);

// Radio pipe addresses for the 2 nodes to communicate.
#ifdef UNO
const uint64_t pipes[2] = { 0xF0F0F0F0D2LL, 0xF0F0F0F0E1LL };
#else
const uint64_t pipes[2] = {0xF0F0F0F0E1LL, 0xF0F0F0F0D2LL};
#endif

void setup(void) {
	pinMode(7, OUTPUT);
	pinMode(2, INPUT);

	digitalWrite(7, LOW);

	Serial.begin(57600);
	Radio.begin();

	Radio.setIRQMask(false, true, true);

	Radio.openWritingPipe(pipes[0]);
	Radio.openReadingPipe(1, pipes[1]);

	cli();
	EIMSK |= (1 << INT0);  // Enable external interrupt INT0
	EICRA |= (1 << ISC01); // Trigger INT0 on falling edge
	sei();				   // Enable global interrupts

	Radio.printDetails();
	Radio.startListening();
}

char aux;
uint8_t sender;

void loop(void) {

	if (Serial.available()) {
		EIMSK &= ~(1 << INT0); // Enable external interrupt INT0
		// First, stop listening so we can talk.
		Radio.stopListening();
		Serial.print("Sending..");

		// Take the time, and send it.  This will block until complete
		aux = Serial.read();
		bool ok = Radio.write(&aux, sizeof(char));
		Serial.println(ok ? "ok" : "fail");

		// Now, continue listening
		Radio.startListening();
		EIMSK |= (1 << INT0); // Enable external interrupt INT0
	}
}

ISR(INT0_vect) {
	digitalWrite(7, HIGH);

	bool tx, fail, rx;
	Radio.whatHappened(tx, fail, rx);

	if (fail) {
		Radio.flush_tx();
	}

	if (rx) {
		char aux;
		Radio.read(&aux, sizeof(char));
		Serial.print(aux);
	}

	Radio.ClearAllIRQFlags();

	digitalWrite(7, LOW);
}
