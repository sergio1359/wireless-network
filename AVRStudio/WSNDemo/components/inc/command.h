/*
 * command.h
 *
 * Created: 27/01/2013 19:37:33
 *  Author: Victor
 */ 


#ifndef COMMAND_H_
#define COMMAND_H_

#include <stdint.h>

#define DIGITAL_READ_OPCODE 0x01
#define DIGITAL_READ_LENGHT 1

#define DIGITAL_WRITE_OPCODE 0x02
#define DIGITAL_WRITE_LENGHT 3

#define DIGITAL_SWITCH_OPCODE 0x03
#define DIGITAL_SWITCH_LENGHT 2

uint8_t getCommandArgsLenght(uint8_t* opcode);


#endif /* COMMAND_H_ */