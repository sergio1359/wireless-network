/*
 * network.cpp
 *
 *  Created on: 24/05/2012
 *      Author: Victor
 */

#include "network.h"

NETWORK Network;

struct ring_buffer
{
  unsigned readIndex : 7;
  unsigned writeIndex : 7;
  uint8_t buffer[BUFFER_SIZE];
};

RF24 Radio(9,10);
ring_buffer dataBuffer  =  { 0, 0, { 0 } };
ring_buffer routeBuffer  =  { 0, 0, { 0 } };

dictionary neighborsTable = {{0}, 0};

unsigned long lastUpdate = 0;

void inline NETWORK::sendPresenceSignal()
{
	DATA_MSG broadcast;
	broadcast.header.type = 1;
	broadcast.header.reserved = 1; //Broadcast
	broadcast.header.id = 0;
	broadcast.from = nodeAddress;
	broadcast.to = BROADCAST_ADDRESS;

	sendMsg(BROADCAST_ADDRESS, broadcast.raw_bytes, 3);
}

void inline NETWORK::updateNeightbors()
{
	int removePointer = -1;
	for(int i=0; i<neighborsTable.end; i++)
	{
		if(neighborsTable.array[i][1]++ > 3) // 4 chances limit exceeded
		{
			if(removePointer<0)
				removePointer = i;
		}else if(removePointer>=0)
		{
			neighborsTable.array[removePointer][0] = neighborsTable.array[i][0];
			neighborsTable.array[removePointer++][1] = neighborsTable.array[i][1];
		}
	}
	if(removePointer>=0)
		neighborsTable.end = removePointer;
}

void NETWORK::init()
{
	Radio.begin();

	Radio.setIRQMask(false, true, false);

	Radio.openReadingPipe(1, FULL_ADDRESS(0xFF));
	Radio.openReadingPipe(2, FULL_ADDRESS(nodeAddress));

	cli();
	EIMSK |= (1 << INT0);   // Enable external interrupt INT0
	EICRA |= (1 << ISC01);  // Trigger INT0 on falling edge
	sei();					// Enable global interrupts

	Radio.printDetails();
	Radio.startListening();
}

void NETWORK::update(){
	unsigned long now = millis();

	if(now - lastUpdate > 5000 || lastUpdate == 0) // Every 5 seconds
	{
		sendPresenceSignal();
		updateNeightbors();
		printNeighbors();
		lastUpdate = now;
	}
}

int NETWORK::available()
{
	//TODO: If we use a fixed size of messages, implement a counter of messages by division.
	//If not, use a private counter incremented in the ISR and decremented in readMsg function
	return dataBuffer.writeIndex - dataBuffer.readIndex;
}

void NETWORK::flush()
{
	dataBuffer.readIndex = dataBuffer.writeIndex;
}

bool NETWORK::sendMsg(uint8_t address, uint8_t* msg, uint8_t len)
{
	EIMSK &= ~(1 << INT0); // Disable external interrupt INT0
	Radio.stopListening(); // Stop listening so we can talk.

	if(lastTXAddress != address)
	{
		Radio.openWritingPipe(FULL_ADDRESS(address));
		lastTXAddress = address;
	}

	// This will block until complete
	bool result = Radio.write(msg, len);

	Radio.startListening(); // Now, continue listening
	EIMSK |= (1 << INT0); // Enable external interrupt INT0
	return result;
}

bool NETWORK::readMsg(uint8_t* msg, uint8_t& len)
{
	if(dataBuffer.readIndex == dataBuffer.writeIndex)
		return false;

	//TODO: DinamicPayloadSize
	memcpy(msg, (uint8_t*)&dataBuffer.buffer + dataBuffer.readIndex, 8);
	dataBuffer.readIndex += 8;
	return true;
}

void NETWORK::printNeighbors()
{
	Serial.print("NEIGHBORS: ");
	if(neighborsTable.end > 0)
	{
		for(int i = 0; i<neighborsTable.end; i++)
		{
			Serial.print(neighborsTable.array[i][0], HEX);
			Serial.print(" (");
			Serial.print(neighborsTable.array[i][1], HEX);
			Serial.print(") ");
		}
		Serial.println("");
	}else
	{
		Serial.println("EMPTY");
	}
}

STATE* state;
FIFO_STATE* fifoState;

ISR(INT0_vect) {
	digitalWrite(7, HIGH);

	state = Radio.getState();

	if (state->rx_dr) {
		uint8_t auxBuffer[8];
		int aux, i;
		do{
			Radio.ClearIRQFlags(false,true, false);

			Radio.read(&auxBuffer, 0, 8); //TODO: DinamicPayloadSize

			aux = (auxBuffer[0] & 0xC0);

			for(i=0;i<8;i++)
			{
				Serial.print(auxBuffer[i], HEX);
				Serial.print(" ");
			}

			Serial.print(" -> ");

			if(aux == DATA)
			{
				memcpy((uint8_t*)&dataBuffer.buffer + dataBuffer.writeIndex, (uint8_t*)&auxBuffer, 8);
				dataBuffer.writeIndex += 8;
				Serial.print("DATA from ");
				Serial.println(auxBuffer[7], HEX);
			}else if (aux == BROADCAST)
			{
					aux = auxBuffer[1];
					Serial.print("Broadcast received from ");
					Serial.println(aux, HEX);

					for(i=0; i<neighborsTable.end; i++)
					{
						if(neighborsTable.array[i][0] == aux)
						{
							neighborsTable.array[i][1] = 0; //Reset the counter
							break;
						}
					}

					if(i == neighborsTable.end) //Not exists
					{
						neighborsTable.array[neighborsTable.end][0] = aux;
						neighborsTable.array[neighborsTable.end++][1] = 0;
					}
			}
			else
			{
				memcpy((uint8_t*)&routeBuffer.buffer + routeBuffer.writeIndex, (uint8_t*)&auxBuffer, 5);
				routeBuffer.writeIndex += 5;
				Serial.print("ROUTE from ");
				Serial.println(auxBuffer[4], HEX);
			}
		}while(!(fifoState = Radio.getFifoState())->rx_empty);//RX FIFO NOT EMPTY
	}

	digitalWrite(7, LOW);
}




