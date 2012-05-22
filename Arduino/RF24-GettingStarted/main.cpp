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

//
// Role management
//
// Set up role.  This sketch uses the same software for all the nodes
// in this system.  Doing so greatly simplifies testing.
//

// The various roles supported by this sketch
typedef enum {
	role_ping_out = 1, role_pong_back
} role_e;

// The debug-friendly names of those roles
const char* role_friendly_name[] = { "invalid", "Ping out", "Pong back" };

// The role of the current running sketch
role_e role = role_pong_back;

//uint64_t ADDRESS[] = { 0xE7E7E7E7E7, 0xC2C2C2C2C2 };

void setup(void) {
	pinMode(7, OUTPUT);
	pinMode(2, INPUT);
	//
	// Print preamble
	//

	Serial.begin(57600);

	//
	// Setup and configure rf radio
	//

	radio.begin();

	//radio.setIRQMask(false,true, false);

	// optionally, increase the delay between retries & # of retries
	//radio.setRetries(15, 15);

	// optionally, reduce the payload size.  seems to
	// improve reliability
	//radio.setPayloadSize(8);

	//radio.setAutoAck(false);
	//radio.setAutoAck(0, true);
	//radio.setRetries(0, 6);

	//radio.openWritingPipe(pipes[0]);
	//radio.openReadingPipe(1,pipes[1]);

	//
	// Open pipes to other nodes for communication
	//
	//radio.openReadingPipe(1, ADDRESS[0]);

	cli();
	EIMSK |= (1 << INT0); // Enable external interrupt INT0
	EICRA |= (1 << ISC01); // Trigger INT0 on falling edge
	sei();
	// Enable global interrupts

	radio.printDetails();
	//radio.startListening();
	radio.switchtoRXMode();
}

bool receiving = false;
bool validISR = false;

void loop(void) {
	if (Serial.available()) {
		Serial.print("Sending..");

		// Take the time, and send it.  This will block until complete
		char aux = Serial.read();
		bool ok = radio.RF_SendData(&aux, sizeof(char));
		Serial.println(ok ? "ok" : "fail");
	}

//	if (Serial.available()) {
//			// First, stop listening so we can talk.
//			radio.stopListening();
//			Serial.print("Sending..");
//
//			// Take the time, and send it.  This will block until complete
//			char aux = Serial.read();
//			bool ok = radio.write(&aux, sizeof(char));
//			Serial.println(ok ? "ok" : "fail");
//
//			// Now, continue listening
//			radio.startListening();
//		}
}

//void RF_Recv_Process( void )
//{
//    uint8_t sta;
//    uint8_t rlen;
//    uint8_t *pPkt;
//
//    if( validISR )
//    {
//		validISR = false;
//
//        sta = radio.get_status();      //Get the RF status
//
//        if( sta & STATUS_RX_DR )    //Receive OK?
//        {
//            //Readout the received data from RX FIFO
//            rlen = RF_ReadRxPayload( (UINT8 *)&g_rf_packet, sizeof(S_RF_PKT) );
//			RF_FLUSH_RX();
//
//            //Is a resend packet?
//            if( g_rf_packet.sn_pkt != g_rf.sn_recv )
//            {
//                //records the packet sn
//                g_rf.sn_recv = g_rf_packet.sn_pkt;
//
//                //Limit the data length of received packet
//                rlen = g_rf_packet.len;
//                if( rlen > RF_PKT_LEN )
//                {
//                    rlen = RF_PKT_LEN;
//                }
//
//                //fill the data of received packet to usb send fifo
//                pPkt = (UINT8 *)g_rf_packet.param;
//                while( rlen-- )
//                {
//                    g_usb_fifo[g_usb.pos_w++] = *pPkt++;
//                    if( g_usb.pos_w >= FIFO_LEN_USB )
//                    {
//                        g_usb.pos_w = 0;
//                    }
//                    g_usb.length++;
//                }
//            }
//        }
//        if( sta & STATUS_MAX_RT )  //Send fail?
//        {
//            RF_FLUSH_TX();  //Flush the TX FIFO
//        }
//
//        RF_CLR_IRQ( sta );  //Clear the IRQ flag
//    }
//}

ISR(INT0_vect) {
	digitalWrite(7, HIGH);

	bool tx, fail, rx;
	radio.whatHappened(tx, fail, rx);

	if (fail)
		radio.flush_tx();

	if (rx) {

	}

	radio.ClearAllIRQFlags();

	digitalWrite(7, LOW);

//	validISR = true;
//
//	if(!receiving)
//	{
//		RF_Recv_Process();
//	}

	/*
	 digitalWrite(7,HIGH);
	 // What happened?
	 bool tx, fail, rx;
	 //radio.whatHappened(tx, fail, rx);

	 if (rx) {
	 char aux;
	 radio.read(&aux, sizeof(char));
	 Serial.print(aux);

	 }

	 if (tx)
	 {
	 Serial.println("TX_DS raised");
	 }

	 if (fail)
	 {
	 radio.flush_tx();
	 Serial.println("MAX_RT raised");
	 }else if(!tx && !rx)
	 Serial.println("ERROR :(");
	 digitalWrite(7,LOW);*/
}
