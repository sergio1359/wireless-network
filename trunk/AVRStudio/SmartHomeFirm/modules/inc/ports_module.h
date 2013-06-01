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

#define PORTS_MODULE_DEFINITION  X(PortModule, portModule_Init, portModule_NotificationInd)

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
	uint8_t seconds;
}DIGITAL_WRITE_MESSAGE_t;

typedef struct
{
	uint8_t dir;
	uint8_t mask;
	uint8_t seconds;
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
	uint8_t seconds;
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


//TODO: MOVE!
// CONTROL
typedef struct
{
}RESET_MESSAGE_t;

void portModule_Init(void);
void portModule_NotificationInd(uint8_t sender, OPERATION_HEADER_t* notification);

void digitalPort_Handler(OPERATION_HEADER_t* operation_header);
void analogPort_Handler(OPERATION_HEADER_t* operation_header);

#endif /* PORTS_MODULE_H_ */