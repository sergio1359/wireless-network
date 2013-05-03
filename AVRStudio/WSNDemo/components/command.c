/*
* command.c
*
* Created: 27/01/2013 19:37:23
*  Author: Victor
*/
#include "command.h"

uint8_t getCommandArgsLenght(uint8_t* opcode)
{
	switch (*opcode)
	{
		case DigitalWrite:
		return sizeof(DIGITAL_WRITE_MESSAGE_t);
		
		case DigitalSwitch:
		return sizeof(DIGITAL_SWITCH_MESSAGE_t);
		
		case  DigitalReadResponse:
		return sizeof(DIGITAL_READ_RESPONSE_MESSAGE_t);
		
		case DigitalRead:
		return sizeof(DIGITAL_READ_MESSAGE_t);
		
		
		
		case AnalogWrite:
		return sizeof(ANALOG_WRITE_MESSAGE_t);

		case AnalogRead:
		return sizeof(ANALOG_READ_MESSAGE_t);
		
		case AnalogReadResponse:
		return sizeof(ANALOG_READ_RESPONSE_MESSAGE_t);
		
		
		
		case Reset:
		return sizeof(RESET_MESSAGE_t);
		
		case RouteTableRead:
		return sizeof(ROUTE_TABLE_READ_t);
		
		case RouteTableReadResponse:
		return sizeof(ROUTE_TABLE_READ_RESPONSE_HEADER_t) + *(opcode+1); //CHECK!!!!!!!!! LENGHT READ
		
		
		
		case ConfigWrite:
		return sizeof(CONFIG_WRITE_HEADER_MESSAGE_t) + *(opcode+1); //CHECK!!!!!!!!! LENGHT READ
		
		case ConfigRead:
		return sizeof(CONFIG_READ_MESSAGE_t);
		
		case ConfigReadResponse:
		return sizeof(CONFIG_READ_RESPONSE_HEADER_MESSAGE_t) + *(opcode+1); //CHECK!!!!!!!!! LENGHT READ
		
		case ConfigChecksum:
		return sizeof(CONFIG_CHECKSUM_MESSAGE_t);
		
		case ConfigChecksumResponse:
		return sizeof(CONFIG_CHECKSUM_RESPONSE_MESSAGE_t);
		
		
		
		case TimeWrite:
		return sizeof(TIME_WRITE_MESSAGE_t);
		
		case TimeRead:
		return sizeof(TIME_READ_MESSAGE_t);
		
		case TimeReadResponse:
		return sizeof(TIME_READ_RESPONSE_MESSAGE_t);
		
		
		
		case  0xFF:
		return 4;// JUST FOR FUN!
		
		//case Extension:
		//return getCommandArgsLenght(opcode+1);
		
		default:
		return 0xFF;
	}
}