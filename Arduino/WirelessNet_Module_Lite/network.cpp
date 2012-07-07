/*
 * network.cpp
 *
 *  Created on: 24/05/2012
 *      Author: Victor
 */

#include "network.h"

#define MAX_RETRIES 3
#define TIMEOUT 10 //In seconds

NETWORK Network;

struct ring_buffer_custom
{
  unsigned readIndex : 7;
  unsigned writeIndex : 7;
  uint8_t buffer[BUFFER_SIZE];
};

RF24 Radio(9,10);
ring_buffer_custom dataInBuffer  =  { 0, 0, { 0 } };
ring_buffer_custom dataOutBuffer  =  { 0, 0, { 0 } };
ring_buffer_custom routeBuffer  =  { 0, 0, { 0 } };

dictionary neighborsTable = {{0}, 0};
dictionary2 lookTable = {{0}, 0}; 		//Nodo buscado(reference) -> {interesado, contador, backWayLenght}
dictionary3 routeTable = {{0}, 0};		//Nodo destino -> { Gateway, Length }

unsigned long lastUpdate = 0;

DATA_MSG broadcast;

ROUTE_MSG routeMsg;

int i;

bool neighborsTableContains(uint8_t address)
{
	for(i=0; i<neighborsTable.end; i++)
	{
		if(neighborsTable.array[i][0] == address)
			break;
	}
	return i != neighborsTable.end;
}

bool routeTableContains(uint8_t address)
{
	for(i=0; i<routeTable.end; i++)
	{
		if(routeTable.array[i][0] == address)
			break;
	}
	return i != routeTable.end;
}

bool lookTableContains(uint8_t address)
{
	for(i=0; i<lookTable.end; i++)
	{
		if(lookTable.array[i][0] == address)
			break;
	}
	return i != lookTable.end;
}

void addToRouteTable(uint8_t nodeReference, uint8_t nodeGateway, uint8_t distance)
{
	for(i=0; i<routeTable.end; i++)
	{
		if(routeTable.array[i][0] == nodeReference || distance < routeTable.array[i][1])
		{
			routeTable.array[i][0] = nodeGateway;
			routeTable.array[i][1] = distance;

			//TODO: BORRAR DEBUG
			PRINTF("MODIFIED\r\n");
		}
	}

	if (i == routeTable.end)
	{
		routeTable.array[routeTable.end][0] = nodeReference;
		routeTable.array[routeTable.end][1] = nodeGateway;
		routeTable.array[routeTable.end++][2] = distance;

		//TODO: BORRAR DEBUG
		PRINTF("ADD NEW, TOTAL: ");
		PRINTF(routeTable.end, DEC);
		PRINTF("\r\n");
	}
}

void removeFromRouteTable(uint8_t address)
{
	int removePointer = -1;
	for(int i=0; i<routeTable.end; i++)
	{
		if(routeTable.array[i][0] == address)
		{
			if(removePointer<0)
				removePointer = i;
		}else if(removePointer>=0)
		{
			routeTable.array[removePointer][0] = routeTable.array[i][0];
			routeTable.array[removePointer][1] = routeTable.array[i][1];
			routeTable.array[removePointer++][2] = routeTable.array[i][2];
		}
	}
	if(removePointer>=0)
	{
		routeTable.end = removePointer;

		//TODO: BORRAR DEBUG
		PRINTF("REMOVED RT, TOTAL: ");
		PRINTF(routeTable.end, DEC);
		PRINTF("\r\n");
	}
}

void removeFromLookTable(uint8_t address)
{
	int removePointer = -1;
	for(int i=0; i<lookTable.end; i++)
	{
		if(lookTable.array[i][0] == address)
		{
			if(removePointer<0)
				removePointer = i;
		}else if(removePointer>=0)
		{
			lookTable.array[removePointer][0] = routeTable.array[i][0];
			lookTable.array[removePointer][1] = routeTable.array[i][1];
			lookTable.array[removePointer][2] = routeTable.array[i][2];
			lookTable.array[removePointer++][3] = routeTable.array[i][3];
		}
	}
	if(removePointer>=0)
		lookTable.end = removePointer;
}

bool inline NETWORK::sendToNeighbor(uint8_t address, uint8_t* msg, uint8_t len)
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

void inline NETWORK::sendPresenceSignal()
{
	broadcast.header.type = 1;
	broadcast.header.reserved = 1; //Broadcast
	broadcast.header.id = 0;
	broadcast.from = nodeAddress;
	broadcast.to = BROADCAST_ADDRESS;

	sendToNeighbor(BROADCAST_ADDRESS, broadcast.raw_bytes, 3);
}

void inline NETWORK::updateNeightbors()
{
	for(i=0; i<neighborsTable.end; i++)
	{
		if(neighborsTable.array[i][1] & 0x80)//Mark
		{
			if(((neighborsTable.array[i][1] & 0x7F) >> 2) < MAX_RETRIES)
			{
				neighborsTable.array[i][1]++;
			}
		}else
			neighborsTable.array[i][1] |= 0x80;
	}
}

void NETWORK::lookFor(uint8_t reference, uint8_t neighborInterested, uint8_t rootInterested, uint8_t distance)
{
	//This occur when many neighbors ask for the same node
	if (lookTableContains(reference))
	    return;

	//This check prevents a node with no neighbors store a search in lookTable
	if (neighborInterested == 0 || neighborsTable.end > 1)
	{
		lookTable.array[lookTable.end][0] = reference;
		lookTable.array[lookTable.end][1] = neighborInterested;
		lookTable.array[lookTable.end][2] = 0;
		lookTable.array[lookTable.end++][3] = distance!=0 ? (distance - 1) : 0;

		PRINTF("ADD TO LOOK: ");
		PRINTF(reference, HEX);
		PRINTF(" ");
		PRINTF(neighborInterested, HEX);
		PRINTF(" ");
		PRINTF(reference, HEX);
		PRINTF(" ");
		PRINTF(lookTable.array[lookTable.end-1][3], HEX);
		PRINTF("\r\n");

		//Send RouteAnswer to everyone in NeighborsTable != neighborInterested
		for(i = 0; i < neighborsTable.end; i++)
		{
			if (neighborsTable.array[i][0] != neighborInterested)
			{
				routeMsg.header.distance = distance;
				routeMsg.header.type = 0;//ROUTE
				routeMsg.header.restype = 1;//ANSWER
				routeMsg.from = rootInterested;
				routeMsg.to = neighborsTable.array[i][0];
				routeMsg.parent = nodeAddress;
				routeMsg.reference = reference;

				sendToNeighbor(neighborsTable.array[i][0], routeMsg.raw_bytes, sizeof(ROUTE_MSG));
			}
		}
	}
}

void NETWORK::init()
{
	Radio.begin();

	Radio.setIRQMask(false, true, false);

	Radio.enableDynamicPayloads();

	Radio.openReadingPipe(1, FULL_ADDRESS(0xFF));
	Radio.openReadingPipe(2, FULL_ADDRESS(nodeAddress));

	cli();
	EIMSK |= (1 << INT0);   // Enable external interrupt INT0
	EICRA |= (1 << ISC01);  // Trigger INT0 on falling edge
	sei();					// Enable global interrupts

#ifdef PRINTMODE
	Radio.printDetails();
#endif
	Radio.startListening();
}
bool turn = true;
void NETWORK::update() {
	if(millis() - lastNeighborsUpdate > 5000 || lastNeighborsUpdate == 0) // Every 5 seconds
	{
		lastNeighborsUpdate = millis();

		sendPresenceSignal();

		//TODO: BORRAR
#ifdef UNO
		if(turn)
			printNeighbors();
		else
		{
			printRouteTable();
			printLookTable();
		}
		turn = !turn;
#else
		//printNeighbors();
		printRouteTable();
		printLookTable();
#endif
		updateNeightbors();
	}

	if (routeBuffer.writeIndex > routeBuffer.readIndex) //check the outstanding route messages
	{
		ROUTE_MSG* routeMsg = ((ROUTE_MSG*) &routeBuffer.buffer[routeBuffer.readIndex]);

		//TODO: BORRAR DEBUG
		PRINTF("READING ROUTE: ");
		for(i=0; i<sizeof(ROUTE_MSG); i++)
		{
			PRINTF(routeMsg->raw_bytes[i], HEX);
			PRINTF(" ");
		}

	     if (routeMsg->header.restype == 1) //Route Answer
	     {
	    	 //TODO: BORRAR DEBUG
	    	 PRINTF("ANSWER\r\n");

	         bool isNeighbor = neighborsTableContains(routeMsg->reference);

			//Check if the first variable returns true, because in this case the second search is not needed
			if (isNeighbor || routeTableContains(routeMsg->reference))
			{
				ROUTE_MSG routeResponse;
				if (isNeighbor)
					routeResponse.header.distance = 1;
				else
					routeResponse.header.distance = (routeTable.array[routeMsg->reference][1] + 1);

				routeResponse.header.restype = 0;//RESPONSE
				routeResponse.header.type = 0;//ROUTE
				routeResponse.header.ok = 1;

				routeResponse.from = nodeAddress;
				routeResponse.to = routeMsg->from;
				routeResponse.parent = nodeAddress;
				routeResponse.reference = routeMsg->reference;

				sendToNeighbor(routeMsg->parent, routeResponse.raw_bytes, sizeof(ROUTE_MSG));

				//TODO: Notify the node sought to avoid a possible subsequent search in the opposite
				routeResponse.header.distance = (byte)(routeMsg->header.distance + 1);
				routeResponse.from = nodeAddress;
				routeResponse.to = routeMsg->reference;
				routeResponse.parent = nodeAddress;
				routeResponse.reference = routeMsg->from;
				//Dest = Program.GetNode(isNeighbor ? routeMsg.reference : RouteTable[message.reference][0]);
				//Dest.SendMessage(routeResponse);
				sendToNeighbor(isNeighbor ? routeMsg->reference : routeTable.array[routeMsg->reference][0],
						routeResponse.raw_bytes, sizeof(ROUTE_MSG));

				addToRouteTable(routeMsg->from, routeMsg->parent, routeMsg->header.distance);
			}
			else
			{
				lookFor(routeMsg->reference, routeMsg->parent, routeMsg->from, (uint8_t)(routeMsg->header.distance + 1));
			}
		}
		else //Route Response
		{
			//TODO: BORRAR DEBUG
			PRINTF("RESPONSE\r\n");

			if (routeMsg->header.ok)
			{
				PRINTF("YEAH!\r\n");
				addToRouteTable(routeMsg->reference, routeMsg->parent, routeMsg->header.distance);

				if (lookTableContains(routeMsg->reference))
				{
					uint8_t interested =  lookTable.array[routeMsg->reference][0];
					if (interested != 0)
					{
						ROUTE_MSG routeResponse;
						routeResponse.header.distance = (routeMsg->header.distance + 1);
						routeResponse.header.restype = 0;//RESPONSE
						routeResponse.header.type = 0;//ROUTE
						routeResponse.header.ok = 1;

						routeResponse.from = routeMsg->from;
						routeResponse.to = routeMsg->to;
						routeResponse.parent = nodeAddress;
						routeResponse.reference = routeMsg->reference;

						//NodeuC Dest = Program.GetNode(interested);
						//Dest.SendMessage(routeResponse);
						sendToNeighbor(interested, routeResponse.raw_bytes, sizeof(ROUTE_MSG));

						//Store the way back if distance > 0 (message.to is not a neighbor)
						uint8_t distance = lookTable.array[routeResponse.reference][2];
						if(distance > 0)
							addToRouteTable(routeResponse.to, interested, distance);
					}
					removeFromLookTable(routeMsg->reference);
				}
			}
			//This case is given when it tried to route a message from a node that is no longer able to reach the recipient.
			else if (routeTableContains(routeMsg->reference) && routeTable.array[routeMsg->reference][0] == routeMsg->parent)
			{
				removeFromRouteTable(routeMsg->reference);

				//Notify the neighbors that I can't reach the intended recipient
				for(i = 0; i < neighborsTable.end; i++)
				{
					if (routeMsg->parent != neighborsTable.array[i][0])
					{
						ROUTE_MSG routeResponse;
						routeResponse.header.restype = 0;//RESPONSE
						routeResponse.header.type = 0;//ROUTE
						routeResponse.header.ok = 0;//FAIL
						routeResponse.header.distance = routeMsg->header.distance;
						routeResponse.from = nodeAddress;
						routeResponse.to = routeMsg->to;
						routeResponse.parent = nodeAddress;
						routeResponse.reference = routeMsg->reference;

						//NodeuC Dest = Program.GetNode(neighborAddress);
						//Dest.SendMessage(routeResponse);
						 sendToNeighbor(neighborsTable.array[i][0], routeResponse.raw_bytes, sizeof(ROUTE_MSG));
					}
				}

				//TODO: If there are pending messages for this node, try to trace a new route
				if (routeMsg->to == nodeAddress)
				{
					PRINTF("Ha cambiado la topologia de la red. Reenviar\r\n");
				}
			}
		}
	     routeBuffer.readIndex += sizeof(ROUTE_MSG);
	}

	while (dataOutBuffer.readIndex != dataOutBuffer.writeIndex)//Messages to send
	{
		DATA_MSG* dataMsg = ((DATA_MSG*) &dataOutBuffer.buffer[dataOutBuffer.readIndex]);

		//TODO:CHECK!! mine
		bool mine = dataMsg->parent == nodeAddress;
		if (!mine || !lookTableContains(dataMsg->to))//Look finished
		{
			if(!mine)
				dataMsg->parent = nodeAddress;

			//TODO: BORRAR DEBUG
			PRINTF("SENDING DATA: ");
			for(i=0; i<sizeof(ROUTE_MSG); i++)
			{
				PRINTF(dataMsg->raw_bytes[i], HEX);
				PRINTF(" ");
			}
			PRINT("\r\n");

			//TODO:USE sendMSG
			bool isNeighbor = neighborsTableContains(dataMsg->to);
			if (isNeighbor || routeTableContains(dataMsg->to))
			{
				if(!isNeighbor)//find Gateway
				{
					for(int i = 0; i<routeTable.end; i++)
					{
						if(routeTable.array[i][0] == dataMsg->to)
							sendToNeighbor(routeTable.array[i][1], dataMsg->raw_bytes, sizeof(DATA_MSG));
					}
				}else
				{
					sendToNeighbor(dataMsg->to, dataMsg->raw_bytes, sizeof(DATA_MSG));
				}
				// TODO: Dynamic DATA-MESSAGE length
			}

//			for(i = 0; i<routeTable.end; i++)
//			{
//				if(routeTable.array[i][0] == dataMsg->to)
//				{
//					sendToNeighbor(routeTable.array[i][1], dataMsg->raw_bytes, sizeof(DATA_MSG));
//					break;
//				}
//			}
//
//			if(i == routeTable.end)
//			{
//				PRINT("ERROR!\r\n");
//			}

			dataOutBuffer.readIndex += sizeof(DATA_MSG);
		}
		else
		{
			int removePointer = -1;
			for(i = 0; i<lookTable.end; i++)
			{
				if(removePointer>=0)
				{
					lookTable.array[removePointer][0] = routeTable.array[i][0];
					lookTable.array[removePointer][1] = routeTable.array[i][1];
					lookTable.array[removePointer][2] = routeTable.array[i][2];
					lookTable.array[removePointer++][3] = routeTable.array[i][3];
				}else if(lookTable.array[i][0] == dataMsg->to && lookTable.array[i][1] == TIMEOUT)
				{
					PRINTF("Timeout superado: MESSAGE FROM ");
					PRINTF(dataMsg->from);
					PRINTF(" TO ");
					PRINTF(dataMsg->to);
					PRINTF("\r\n");
					//removeFromLookTable(dataMsg->to);
					removePointer = i;
					dataOutBuffer.readIndex += sizeof(DATA_MSG);
				}
			}
			if(removePointer == -1)
				break;
		}
	}

	if(millis() - lastLookUpdate > 1000 && lookTable.end != 0) // Every 1 second
	{
		lastLookUpdate = millis();
		int removePointer = -1;

		for(i=0; i<lookTable.end; i++)
		{
			uint8_t reference = lookTable.array[i][0];
			uint8_t counter = lookTable.array[i][2];

			if (counter != TIMEOUT)
			{
				lookTable.array[i][2]++;
				if(removePointer>=0)
				{
					lookTable.array[removePointer][0] = routeTable.array[i][0];
					lookTable.array[removePointer][1] = routeTable.array[i][1];
					lookTable.array[removePointer][2] = routeTable.array[i][2];
					lookTable.array[removePointer++][3] = routeTable.array[i][3];
				}
			}
			else if(removePointer<0)//Remove
			{
				removePointer = i;
			}
		}

		if(removePointer>=0)
			lookTable.end = removePointer;
	}
}

int NETWORK::available()
{
	//TODO: If we use a fixed size of messages, implement a counter of messages by division.
	//If not, use a private counter incremented in the ISR and decremented in readMsg function
	return dataInBuffer.writeIndex - dataInBuffer.readIndex;
}

void NETWORK::flush()
{
	dataInBuffer.readIndex = dataInBuffer.writeIndex;
}

bool NETWORK::sendMsg(uint8_t address, DATA_MSG* msg)
{
	bool isNeighbor = neighborsTableContains(address);
	if (isNeighbor || routeTableContains(address))
	{
		if(!isNeighbor)//find Gateway
		{
			for(int i = 0; i<routeTable.end; i++)
			{
				if(routeTable.array[i][0] == address)
					address = routeTable.array[i][1];
			}
		}
		return sendToNeighbor(address, msg->raw_bytes, sizeof(DATA_MSG));// TODO: Dynamic DATA-MESSAGE length
	}
	else
	{
	    lookFor(address, 0, nodeAddress, 0);
	    //Add message to dataOutBuffer
	    memcpy((uint8_t*)&dataOutBuffer.buffer + dataOutBuffer.writeIndex, msg->raw_bytes, sizeof(DATA_MSG));
		return false;
	}
}

bool NETWORK::readMsg(DATA_MSG* msg)
{
	if(dataInBuffer.readIndex == dataInBuffer.writeIndex)
		return false;

	memcpy(msg->raw_bytes, (uint8_t*)&dataInBuffer.buffer + dataInBuffer.readIndex, sizeof(DATA_MSG));
	dataInBuffer.readIndex += 8;
	return true;
}

void NETWORK::printNeighbors()
{
	PRINTF("NEIGHBORS: ");

	LCD_CLEAR();
	LCD_GOTO(0,0);
	LCD_PRINT("NEIGHBORS: ");

	if(neighborsTable.end != 0)
	{
		for(int i = 0; i<neighborsTable.end; i++)
		{
			PRINTF(neighborsTable.array[i][0], HEX);
			PRINTF(" (");
			PRINTF((neighborsTable.array[i][1] & 0x7F) >> 2);
			PRINTF(" ");
			PRINTF(neighborsTable.array[i][1] & 0x03);
			PRINTF(")");
			PRINTF((neighborsTable.array[i][1] & 0x80) ? "? " : "  ");

			LCD_GOTO(0,i+1);
			LCD_PRINT_NUM(neighborsTable.array[i][0]);
			LCD_PRINT(" (");
			LCD_PRINT_NUM(((neighborsTable.array[i][1] & 0x7F) >> 2));
			LCD_PRINT(" ");
			LCD_PRINT_NUM((neighborsTable.array[i][1] & 0x03));
			LCD_PRINT(") ");
			LCD_PRINT_CHAR((neighborsTable.array[i][1] & 0x80) ? '?' : ' ');
		}
		PRINTF("\r\n");
	}else
	{
		PRINTF("EMPTY\r\n");

		LCD_GOTO(0,1);
		LCD_PRINT("EMPTY");
	}
}

void NETWORK::printRouteTable()
{
	PRINTF("ROUTE TABLE: ");

	LCD_CLEAR();
	LCD_GOTO(0,0);

	if(routeTable.end != 0)
	{
		for(int i = 0; i<routeTable.end; i++)
		{
			PRINTF(routeTable.array[i][0], HEX);
			PRINTF(" BY ");
			PRINTF(routeTable.array[i][1], HEX);
			PRINTF(" (");
			PRINTF(routeTable.array[i][2], DEC);
			PRINTF(") ");

			LCD_GOTO(0,i);
			LCD_PRINT("R ");
			LCD_PRINT_NUM(routeTable.array[i][0]);
			LCD_PRINT(" BY ");
			LCD_PRINT_NUM(routeTable.array[i][1]);
			LCD_PRINT(" (");
			LCD_PRINT_NUM(routeTable.array[i][2]);
			LCD_PRINT(")");
		}
		PRINTF("\r\n");
	}else
	{
		PRINTF("EMPTY\r\n");

		LCD_GOTO(0,0);
		LCD_PRINT("R EMPTY");
	}
}

void NETWORK::printLookTable()
{
	PRINTF("LOOK TABLE: ");

	if(lookTable.end != 0)
	{
		for(int i = 0; i<lookTable.end; i++)
		{
			PRINTF(lookTable.array[i][0], HEX);
			PRINTF(" BY ");
			PRINTF(lookTable.array[i][1], HEX);
			PRINTF(" (");
			PRINTF(lookTable.array[i][2], DEC);
			PRINTF(", ");
			PRINTF(lookTable.array[i][3], DEC);
			PRINTF(") ");

			LCD_GOTO(0,routeTable.end + 1 + i);
			LCD_PRINT("L ");
			LCD_PRINT_NUM(lookTable.array[i][0]);
			LCD_PRINT(">");
			LCD_PRINT_NUM(lookTable.array[i][1]);
			LCD_PRINT(" ");
			LCD_PRINT_NUM(lookTable.array[i][2]);
			LCD_PRINT(",");
			LCD_PRINT_NUM(lookTable.array[i][3]);
		}
		PRINTF("\r\n");
	}else
	{
		PRINTF("EMPTY\r\n");

		LCD_GOTO(0,routeTable.end + 1);
		LCD_PRINT("L EMPTY");
	}
}

STATE* state;
FIFO_STATE* fifoState;
uint8_t auxBuffer[8];
int aux, e;

ISR(INT0_vect) {
	DIGITAL_WRITE(LED_PIN, HIGH);

	state = Radio.getState();

	if (state->rx_dr) {
		do{
			Radio.ClearIRQFlags(false,true, false);

			aux = Radio.getDynamicPayloadSize();

			Radio.read(&auxBuffer, 0, aux);

			if((auxBuffer[0] & 0xC0) != BROADCAST)//TODO:BORRAR
			{
				for(e=0;e<aux;e++)
				{
					PRINTF(auxBuffer[e], HEX);
					PRINTF(" ");
				}

				PRINTF(" -> ");
			}

			aux = (auxBuffer[0] & 0xC0);
			if(aux == DATA)
			{
				PRINTF("DATA from ");
				PRINTF(auxBuffer[3], HEX);
				PRINTF("\r\n");

				if(auxBuffer[2] == nodeAddress)//It's to me?
				{
					memcpy((uint8_t*)&dataInBuffer.buffer + dataInBuffer.writeIndex, (uint8_t*)&auxBuffer, sizeof(DATA_MSG));
					dataInBuffer.writeIndex += sizeof(DATA_MSG);
				}else
				{
					memcpy((uint8_t*)&dataOutBuffer.buffer + dataOutBuffer.writeIndex, (uint8_t*)&auxBuffer, sizeof(DATA_MSG));
					dataOutBuffer.writeIndex += sizeof(DATA_MSG);
				}
			}else if (aux == BROADCAST)
			{
				aux = auxBuffer[1];
				//PRINTF("Broadcast received from ");
				//PRINTF(aux, HEX);
				//PRINTF("\r\n");

				for(e=0; e<neighborsTable.end; e++)
				{
					if(neighborsTable.array[e][0] == aux)
					{
//						neighborsTable.array[e][1] &= 0xFC; //Reset the counter
						neighborsTable.array[e][1] &= 0x7F;//Clean flag
						if(neighborsTable.array[e][1]!=0)
							neighborsTable.array[e][1]--;

						break;
					}
				}

#if defined(UNO)
bool trollCondition = true;
#elif defined(DUEMILANOVE)
bool trollCondition = aux != 0x03;
#else
bool trollCondition = aux != 0x02;
#endif

				if(e == neighborsTable.end && trollCondition) //Not exists
				{
					neighborsTable.array[neighborsTable.end][0] = aux;
					neighborsTable.array[neighborsTable.end++][1] = 0;
				}
			}
			else
			{
				memcpy((uint8_t*)&routeBuffer.buffer + routeBuffer.writeIndex, (uint8_t*)&auxBuffer, sizeof(ROUTE_MSG));
				routeBuffer.writeIndex += sizeof(ROUTE_MSG);
				PRINTF("ROUTE from ");
				PRINTF(auxBuffer[3], HEX);
				PRINTF("\r\n");
			}
		}while(!(fifoState = Radio.getFifoState())->rx_empty);//RX FIFO NOT EMPTY
	}

	DIGITAL_WRITE(LED_PIN, LOW);
}




