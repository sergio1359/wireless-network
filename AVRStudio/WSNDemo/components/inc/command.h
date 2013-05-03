/*
 * command.h
 *
 * Created: 27/01/2013 19:37:33
 *  Author: Victor
 */ 


#ifndef COMMAND_H_
#define COMMAND_H_

#include <stdint.h>
#include "RTC.h"

typedef enum COMMAND_OPCODES
{
	Reset = 0x00,
	RouteTableRead = 0x01,
	RouteTableReadResponse = 0x02,

	DigitalWrite = 0x05,
	DigitalSwitch = 0x06,
	DigitalRead = 0x07,
	DigitalReadResponse = 0x08,

	AnalogWrite = 0x10,
	AnalogRead = 0x11,
	AnalogReadResponse = 0x12,

	TimeWrite = 0x20,
	TimeRead = 0x21,
	TimeReadResponse = 0x22,

	Warning = 0x30,
	Error = 0x31,

	ConfigWrite = 0x40,
	ConfigRead = 0x41,
	ConfigReadResponse = 0x42,
	ConfigChecksum = 0x43,
	ConfigChecksumResponse = 0x44,

	ColorWrite = 0x50,
	ColorWriteRandom = 0x51,
	ColorSecuenceWrite = 0x52,
	ColorSortedSecuenceWrite = 0x53,
	ColorRead = 0x54,
	ColorReadResponse = 0x55,

	PresenceRead = 0x57,
	PresenceReadResponse = 0x58,

	TemperatureRead = 0x5A,
	TemperatureReadResponse = 0x5B,
	HumidityRead = 0x5C,
	HumidityReadResponse = 0x5D,

	PowerRead = 0x60,
	PowerReadResponse = 0x61,

	LuminosityRead = 0x63,
	LuminosityReadResponse = 0x64,

	Extension = 0xFF,
}COMMAND_OPCODES;

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





uint8_t getCommandArgsLenght(uint8_t* opcode);


#endif /* COMMAND_H_ */