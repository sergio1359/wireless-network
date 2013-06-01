/*
 * modules.h
 *
 * Created: 27/01/2013 19:37:33
 *  Author: Victor
 */ 


#ifndef MODULES_H_
#define MODULES_H_

#include <stdint.h>
#include <stdbool.h>
#include "EEPROM.h"

#include "ports_module.h"
#include "network_module.h"
#include "config_module.h"

#define EXTENSION_OPCODE 0xFF

#define MODULES_TABLE	  \
PORTS_MODULE_DEFINITION   \
NETWORK_MODULE_DEFINITION \
CONFIG_MODULE_DEFINITION  \

#define X(a, b, c) a,
typedef enum MODULES_ID {
	MODULES_TABLE
};
#undef X


#define COMMANDS_TABLE  \
COMMANDS_TABLE_PORTS    \
COMMANDS_TABLE_NETWORK  \
COMMANDS_TABLE_CONFIG   \

#define X(a, b, c, d, e) a = b,
typedef enum COMMAND_OPCODES {
	COMMANDS_TABLE
};
#undef X

void modules_Init(void);
uint8_t getCommandArgsLength(uint8_t* opcode);
extern inline void handleCommand(OPERATION_HEADER_t* header);

/*
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
}COMMAND_OPCODES;*/


#endif /* MODULES_H_ */