/*
 * global.h
 *
 *  Created on: 22/05/2012
 *      Author: Victor
 */

#ifndef GLOBAL_H_
#define GLOBAL_H_

typedef enum {
	DATA = 0x80, BROADCAST = 0xC0, ROUTE_ANSWER = 0x40, ROUTE_RESPONSE = 0x00
} MSG_TYPE;

typedef union DATA_MSG {
	struct {
		struct {
			unsigned id :6;
			unsigned reserved :1; //must be 0
			unsigned type :1; //must be 1
		} header;
		uint8_t from;
		uint8_t to;
		uint8_t parent; //neighbor who has sent the message
		uint8_t data[4];
	};
	uint8_t raw_bytes[8];
};

typedef union ROUTE_MSG {
	struct {
		struct {
			unsigned distance :5;
			unsigned ok :1; //for response only
			unsigned restype :1; // 0 -> response ; 1 -> answer
			unsigned type :1; //must be 0
		} header;
		uint8_t from;
		uint8_t to;
		uint8_t parent; //neighbor who has sent the message
		uint8_t reference; //searched node
	};
	uint8_t raw_bytes[5];
};

#define LIST_SIZE 20

struct list
{
	uint8_t array[LIST_SIZE];
	uint8_t end;
};

struct dictionary
{
	uint8_t array[LIST_SIZE][2];
	uint8_t end;
};

struct dictionary2
{
	uint8_t array[LIST_SIZE][3];
	uint8_t end;
};

struct dictionary3
{
	uint8_t array[LIST_SIZE][4];
	uint8_t end;
};

extern uint32_t NetworkAddress;
extern uint8_t nodeAddress;
extern uint8_t masterAddress;

#define FULL_ADDRESS(x) ({(uint64_t)NetworkAddress <<8 | x;})
#define BROADCAST_ADDRESS 0xFF
#define GATEWAY_ADDRESS 0x01

#define LED_PIN 8

#endif /* GLOBAL_H_ */
