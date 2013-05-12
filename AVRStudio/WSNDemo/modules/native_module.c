/*
* native_module.c
*
* Created: 12/05/2013 13:00:25
*  Author: Victor
*/
#include "native_module.h"
#include "command.h"
#include "EEPROM.h"
#include "globals.h"

bool validatePortAction(uint8_t dir, uint8_t mask, bool read, bool digital);
bool doPortAction(uint8_t dir, uint8_t mask, bool read, bool digital, uint8_t value);

void nativeHandler(uint8_t* opcode)
{
	if(*opcode  == DigitalWrite)
	{
		DIGITAL_WRITE_MESSAGE_t* msg = (opcode + 1);
		doPortAction(msg->dir, msg->mask, false, true, msg->value);
	}
}

bool validatePortAction(uint8_t port, uint8_t mask, bool read, bool digital)
{
	PORT_CONFIG_t config;
	bool result;
	
	switch(port)
	{
		case 0: config = runningConfiguration.topConfiguration.portConfig_PA; break;
		case 1: config = runningConfiguration.topConfiguration.portConfig_PB; break;
		case 2: config = runningConfiguration.topConfiguration.portConfig_PC; break;
		case 3: config = runningConfiguration.topConfiguration.portConfig_PD; break;
		case 4: config = runningConfiguration.topConfiguration.portConfig_PE; break;
		case 5: config = runningConfiguration.topConfiguration.portConfig_PF; break;
		case 6: config = runningConfiguration.topConfiguration.portConfig_PG; break;
	}
	
	if(read)
		result = ((~config.maskIO) & mask) == mask;
	else
		result = (config.maskIO & mask) == mask;
		
	if(digital)
		result &= (config.maskAD & mask) == mask;
	else
		result &= ((~config.maskAD) & mask) == mask;
		
	return result;
}

bool doPortAction(uint8_t dir, uint8_t mask, bool read, bool digital, uint8_t value)
{
	uint8_t valueRead = 0;
	uint8_t port = dir > 7 ? dir : 0;
	while(port > 7)
	{
		port = port % 8;
	};
	
	if(!validatePortAction(port, mask, read, digital))
		return false;
	
	switch(port)
	{
		case 0: //PORTA
			if(read)
			{
				valueRead = HAL_GPIO_PORTA_read();
				//TODO: Send a DIGITAL_READ_RESPONSE_MESSAGE_t
			}
			else
			{				
				HAL_GPIO_PORTA_set(value & mask);
				HAL_GPIO_PORTA_clr(~value & mask);	
			}
		break;
		
		case 1: //PORTB
		break;
		
		case 2: //PORTC
		break;
		
		case 3: //PORTD
		break;
		
		case 4: //PORTE
		break;
		
		case 5: //PORTF
		break;
		
		case 6: //PORTG
		break;
	}
	
	return true;
}


/*
uint8_t getCommandArgsLength(uint8_t* opcode)
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
		return sizeof(ROUTE_TABLE_READ_RESPONSE_HEADER_t) + *(opcode+1); //CHECK!!!!!!!!! LENGTH READ
		
		
		
		case ConfigWrite:
		return sizeof(CONFIG_WRITE_HEADER_MESSAGE_t) + *(opcode+1); //CHECK!!!!!!!!! LENGTH READ
		
		case ConfigRead:
		return sizeof(CONFIG_READ_MESSAGE_t);
		
		case ConfigReadResponse:
		return sizeof(CONFIG_READ_RESPONSE_HEADER_MESSAGE_t) + *(opcode+1); //CHECK!!!!!!!!! LENGTH READ
		
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
		//return getCommandArgsLength(opcode+1);
		
		default:
		return 0xFF;
	}
}*/