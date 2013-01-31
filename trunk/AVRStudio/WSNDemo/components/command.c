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
		case DIGITAL_READ_OPCODE:
			return DIGITAL_READ_LENGHT;
			break;
		
		case DIGITAL_WRITE_OPCODE:
			return DIGITAL_WRITE_LENGHT;
			break;
		
		case DIGITAL_SWITCH_OPCODE:
			return DIGITAL_SWITCH_LENGHT;
			break;
		
		default:
			return 0xFF;
			break;
	}
}