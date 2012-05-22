/*
 * recovery.cpp
 *
 *  Created on: 18/05/2012
 *      Author: Victor
 */
#include <WProgram.h>
#include <arduino.h>
#include "libraries/SPI/SPI.h"
#include <SPI.h>
#include "libraries/RF24/RF24.h"
#include "printf.h"

void check_radio(void);

/*
 Blink
 Turns on an LED on for one second, then off for one second, repeatedly.

 This example code is in the public domain.
 */

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

RF24 radio(9, 10);

//
// Topology
//

// Radio pipe addresses for the 2 nodes to communicate.
const uint64_t pipes[2] = { 0xF0F0F0F0E1LL, 0xF0F0F0F0D2LL };

void setup(void) {
	sei();
	// Enable global interrupts
	EIMSK |= (1 << INT0); // Enable external interrupt INT0
	EICRA |= (1 << ISC01); // Trigger INT0 on falling edge

	//
	// Print preamble
	//

	Serial.begin(57600);

	//
	// Setup and configure rf radio
	//

	radio.begin();

	// optionally, increase the delay between retries & # of retries
	//radio.setRetries(15, 15);

	// optionally, reduce the payload size.  seems to
	// improve reliability
	radio.setPayloadSize(8);

	radio.setAutoAck(true);
	radio.setRetries(15, 15);

	radio.openWritingPipe(pipes[1]);
	radio.openReadingPipe(1,pipes[0]);

	//
	// Open pipes to other nodes for communication
	//

	radio.printDetails();
	radio.startListening();
}

void loop(void) {
	if (Serial.available()) {
		// First, stop listening so we can talk.
		radio.stopListening();
		Serial.print("Sending..");

		// Take the time, and send it.  This will block until complete
		char aux = Serial.read();
		bool ok = radio.write(&aux, sizeof(char));
		Serial.println(ok ? "ok" : "fail");

		// Now, continue listening
		radio.startListening();
	}
}

ISR(INT0_vect) {
	// What happened?
	bool tx, fail, rx;
	radio.whatHappened(tx, fail, rx);
	if (rx) {
		char aux;
		radio.read(&aux, sizeof(char));
		Serial.print(aux);
		Serial.print("pringao");
	} else if (tx)
		Serial.println("TX_DS raised");
	else if (fail)
		Serial.println("TX_DS raised");
	else
		Serial.println("ERROR :(");
}




