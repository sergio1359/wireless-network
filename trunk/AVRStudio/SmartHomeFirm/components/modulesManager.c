/*
* modulesManager.c
*
* Created: 27/01/2013 19:37:23
*  Author: Victor
*/
#include "modulesManager.h"

//--------------------MODULES-----------------//
#define X(a, b, c) [a] = b,
void (*modules_Inits[]) () = {
	MODULES_TABLE
};
#undef X

#define X(a, b, c) [a] = c,
void (*modules_NotificationInd[]) (uint8_t, OPERATION_HEADER_t*) = {
	MODULES_TABLE
};
#undef X

//------------------OPERATIONS----------------//
#define X(a, b, c, d, e, f) [b] = c,
void (*command_handlers[]) (OPERATION_HEADER_t*) = {
	COMMANDS_TABLE
};
#undef X


#define  X(a, b, c, d, e, f) [b] = d,
void (*command_DataConfs[]) (OPERATION_DataConf_t *req) = {
	COMMANDS_TABLE
};
#undef X

#define X(a, b, c, d, e, f) [b] = sizeof(e),
uint8_t command_lengths[] = {
	COMMANDS_TABLE
};
#undef X

#define X(a, b, c, d, e, f) [b] = f,
bool command_is_dinamic[] = {
	COMMANDS_TABLE
};
#undef X

void MODULES_Init(void)
{
	for(uint8_t i = 0; i<(sizeof(modules_Inits) / sizeof(void*));i++)
		(*modules_Inits[i])();
}

void MODULES_Notify(uint8_t moduleId, OPERATION_HEADER_t* header)
{
	for(uint8_t i = 0; i<(sizeof(modules_NotificationInd) / sizeof(void*));i++)
	{
		if(i != moduleId)// Doesn't notify to the sender
			(*modules_NotificationInd[i])(moduleId, header);	
	}
}

uint8_t MODULES_GetCommandArgsLength(uint8_t* opcode)
{
	if(*opcode == EXTENSION_OPCODE)
		return 4;// JUST FOR TRIALS! In final version, we need to decode the next byte. *(opcode + 1)
	
	if(command_is_dinamic[*opcode])
		return command_lengths[*opcode] + *(opcode+2);
	else
		return command_lengths[*opcode];
}

void* MODULES_DataConf(uint8_t opcode)
{
	return (*command_DataConfs[opcode]);
}	

void MODULES_HandleCommand(OPERATION_HEADER_t* header)
{
	//TODO: Check if exists
	if(command_handlers[header->opCode] != 0)
		(*command_handlers[header->opCode]) (header);
}

_Bool MODULES_HandledByFirmware(uint8_t opcode)
{
	return opcode == DateTimeRead			||
		   opcode == DateTimeReadResponse	||
		   opcode == FirmwareVersionRead;
}
