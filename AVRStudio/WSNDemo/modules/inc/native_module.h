/*
 * native_module.h
 *
 * Created: 12/05/2013 13:00:15
 *  Author: Victor
 */ 


#ifndef NATIVE_MODULE_H_
#define NATIVE_MODULE_H_

#include <stdint.h>
#include <stdbool.h>
#include "RTC.h"

//DIGITAL PORT MESSAGES
typedef struct
{
	uint8_t dir;
	uint8_t mask;
	uint8_t value;
	TIME_t time;
}DIGITAL_WRITE_MESSAGE_t;

typedef struct
{
	uint8_t dir;
	uint8_t mask;
	TIME_t time;
}DIGITAL_SWITCH_MESSAGE_t;

typedef struct
{
	uint8_t dir;
}DIGITAL_READ_MESSAGE_t;

typedef struct
{
	uint8_t dir;
	uint8_t value;
}DIGITAL_READ_RESPONSE_MESSAGE_t;



//ANALOG PORT MESSAGES
typedef struct
{
	uint8_t dir;
	uint8_t value;
	TIME_t time;
}ANALOG_WRITE_MESSAGE_t;

typedef struct
{
	uint8_t dir;
}ANALOG_READ_MESSAGE_t;

typedef struct
{
	uint8_t dir;
	uint8_t value;
}ANALOG_READ_RESPONSE_MESSAGE_t;



// CONTROL
typedef struct
{
}RESET_MESSAGE_t;

typedef struct
{
}ROUTE_TABLE_READ_t;

typedef struct
{
	uint16_t length;
}ROUTE_TABLE_READ_RESPONSE_HEADER_t;



// CONFIGURATION
typedef struct
{
	uint16_t length;
}CONFIG_WRITE_HEADER_MESSAGE_t;

typedef struct
{
}CONFIG_READ_MESSAGE_t;

typedef struct
{
	uint16_t length;
}CONFIG_READ_RESPONSE_HEADER_MESSAGE_t;

typedef struct
{
}CONFIG_CHECKSUM_MESSAGE_t;

typedef struct
{
	uint16_t checksum;
}CONFIG_CHECKSUM_RESPONSE_MESSAGE_t;



//TIEMPO
typedef struct
{
	TIME_t time;
}TIME_WRITE_MESSAGE_t;

typedef struct
{
}TIME_READ_MESSAGE_t;

typedef struct
{
	TIME_t time;
}TIME_READ_RESPONSE_MESSAGE_t;


void nativeHandler(uint8_t* opcode);

#endif /* NATIVE_MODULE_H_ */