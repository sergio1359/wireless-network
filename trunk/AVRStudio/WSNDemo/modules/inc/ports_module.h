/*
 * native_module.h
 *
 * Created: 12/05/2013 13:00:15
 *  Author: Victor
 */ 


#ifndef PORT_MODULE_H_
#define PORT_MODULE_H_

#include <stdint.h>
#include <stdbool.h>
#include "RTC.h"
#include "EEPROM.h"

#define COMMANDS_TABLE_PORTS \
X(DigitalWrite,			0x05, digitalPort_Handler, DIGITAL_WRITE_MESSAGE_t,			false)	\
X(DigitalSwitch,		0x06, digitalPort_Handler, DIGITAL_SWITCH_MESSAGE_t,		false)	\
X(DigitalRead,			0x07, digitalPort_Handler, DIGITAL_READ_MESSAGE_t,			false)	\
X(DigitalReadResponse,	0x08, digitalPort_Handler, DIGITAL_READ_RESPONSE_MESSAGE_t,	false)	\
X(AnalogWrite,			0x10, analogPort_Handler,  ANALOG_WRITE_MESSAGE_t,			false)	\
X(AnalogRead,			0x11, analogPort_Handler,  ANALOG_READ_MESSAGE_t,			false)	\
X(AnalogReadResponse,	0x12, analogPort_Handler,  ANALOG_READ_RESPONSE_MESSAGE_t,	false)	\

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

void portModule_Init(void);
void portModule_TaskHandler(void);
void digitalPort_Handler(OPERATION_HEADER_t* operation_header, uint16_t sourceAddress);
void analogPort_Handler(OPERATION_HEADER_t* operation_header, uint16_t sourceAddress);

#endif /* PORTS_MODULE_H_ */