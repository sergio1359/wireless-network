/*
 * network.h
 *
 *  Created on: 24/05/2012
 *      Author: Victor
 */

#ifndef NETWORK_H_
#define NETWORK_H_

#include "arduino.h"
#include "libraries/RF24/RF24.h"
#include "global.h"

#define BUFFER_SIZE 128

struct ring_buffer;

class NETWORK {
private:
	//RF24 Radio;
	unsigned long lastUpdate;
	uint8_t lastTXAddress;

protected:
	bool inline sendToNeighbor(uint8_t address, uint8_t* msg, uint8_t len);

	void inline sendPresenceSignal(void);

	void inline updateNeightbors(void);

	 //RootInterested represent the first interesed, who start the interrogation
	void lookFor(byte reference, byte neighborInterested, byte rootInterested, byte distance);

public:
	void init(void);

	void update(void);

	int available(void);

	void flush(void);

	//bool sendMsg(uint8_t address, uint8_t* msg, uint8_t len);
	bool sendMsg(uint8_t address, DATA_MSG* msg);

	//bool readMsg(uint8_t* msg, uint8_t& len);
	bool readMsg(DATA_MSG* msg);

	void printNeighbors(void);

	void printRouteTable(void);

	void printLookTable(void);
};

extern NETWORK Network;

#endif /* NETWORK_H_ */
