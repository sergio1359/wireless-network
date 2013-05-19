/*
* native_module.c
*
* Created: 12/05/2013 13:00:25
*  Author: Victor
*/
#include "modules.h"
#include "globals.h"
#include "portMonitor.h"

DIGITAL_READ_RESPONSE_MESSAGE_t digitalResponse;
ANALOG_READ_RESPONSE_MESSAGE_t analogResponse;

bool validatePortAction(PORT_CONFIG_t config, uint8_t mask, bool digital, bool read);
bool proccessDigitalPortAction(uint8_t dir, uint8_t mask, bool read, uint8_t value, uint16_t sourceAddress);

void portModule_Init(void)
{
	//TODO: Read and set configuration
}

void portModule_TaskHandler(void)
{
	//TODO: Check the programed off pins
}

void digitalPort_Handler(OPERATION_HEADER_t* operation_header, uint16_t sourceAddress)
{
	if(operation_header->opCode == DigitalWrite)
	{
		DIGITAL_WRITE_MESSAGE_t* msg = (DIGITAL_WRITE_MESSAGE_t*)(operation_header->opCode + 1);
		//TODO: Handle the time param
		if(!proccessDigitalPortAction(msg->dir, msg->mask, false,  msg->value, sourceAddress))
		{
			//TODO:SEND ERROR MESSAGE (INVALID OPERATION)
		}
	}else if(operation_header->opCode == DigitalSwitch)
	{
		DIGITAL_SWITCH_MESSAGE_t* msg = (DIGITAL_SWITCH_MESSAGE_t*)(operation_header->opCode + 1);
		//TODO: Handle the time param
		if(!proccessDigitalPortAction(msg->dir, msg->mask, false,  ~lastValuesD[msg->dir], sourceAddress))
		{
			//TODO:SEND ERROR MESSAGE (INVALID OPERATION)
		}
	}else if(operation_header->opCode == DigitalRead)
	{
		DIGITAL_READ_MESSAGE_t* msg = (DIGITAL_READ_MESSAGE_t*)(operation_header->opCode + 1);
		if(!proccessDigitalPortAction(msg->dir, 0, true,  0, sourceAddress))
		{
			//TODO:SEND ERROR MESSAGE (INVALID OPERATION)
		}
	}else if(operation_header->opCode == DigitalReadResponse) //As action
	{
		DIGITAL_READ_MESSAGE_t* msg = (DIGITAL_READ_MESSAGE_t*)(operation_header->opCode + 1);
		if(!proccessDigitalPortAction(msg->dir, 0, true,  0, sourceAddress))
		{
			//TODO:SEND ERROR MESSAGE (INVALID OPERATION)
		}
	}
}

void analogPort_Handler(OPERATION_HEADER_t* operation_header, uint16_t sourceAddress)
{
	if(operation_header->opCode == AnalogWrite)
	{
		DIGITAL_WRITE_MESSAGE_t* msg = (DIGITAL_WRITE_MESSAGE_t*)(operation_header->opCode + 1);
		//TODO: To consider the time param
		if(!proccessDigitalPortAction(msg->dir, msg->mask, false,  msg->value, sourceAddress))
		{
			//TODO:SEND ERROR MESSAGE
		}
	}else if(operation_header->opCode == AnalogRead)
	{
		DIGITAL_SWITCH_MESSAGE_t* msg = (DIGITAL_SWITCH_MESSAGE_t*)(operation_header->opCode + 1);
		//TODO: To consider the time param
		if(!proccessDigitalPortAction(msg->dir, msg->mask, false, ~lastValuesD[msg->dir], sourceAddress))
		{
			//TODO:SEND ERROR MESSAGE
		}
	}else if(operation_header->opCode == AnalogReadResponse)
	{
		DIGITAL_READ_MESSAGE_t* msg = (DIGITAL_READ_MESSAGE_t*)(operation_header->opCode + 1);
		if(!proccessDigitalPortAction(msg->dir, 0, true,  0, sourceAddress))
		{
			//TODO:SEND ERROR MESSAGE
		}
	}
}	

_Bool validatePortAction(PORT_CONFIG_t config, uint8_t mask, _Bool digital, _Bool read)
{
	_Bool result;
		
	if(digital)
		result = (config.maskAD & mask) == mask;
	else
		result = ((~config.maskAD) & mask) == mask;
		
	if(read)
		result &= ((~config.maskIO) & mask) == mask;
	else
		result &= (config.maskIO & mask) == mask;
		
	return result;
}

_Bool proccessDigitalPortAction(uint8_t port, uint8_t mask, _Bool read, uint8_t value, uint16_t sourceAddress)
{
	switch(port)
	{
		case 0: //PORTA
			if(!validatePortAction(runningConfiguration.topConfiguration.portConfig_PA, mask, true, read))
				return false;

			if(!read)
			{			
				HAL_GPIO_PORTA_set(value & mask);
				HAL_GPIO_PORTA_clr(~value & mask);	
			}
		break;
		
		case 1: //PORTB
			if(!validatePortAction(runningConfiguration.topConfiguration.portConfig_PB, mask, true, read))
				return false;
		
			if(!read)
			{
				HAL_GPIO_PORTB_set(value & mask);
				HAL_GPIO_PORTB_clr(~value & mask);
			}
		break;
		
		case 2: //PORTC
			if(!validatePortAction(runningConfiguration.topConfiguration.portConfig_PC, mask, true, read))
				return false;
		
			if(!read)
			{
				HAL_GPIO_PORTC_set(value & mask);
				HAL_GPIO_PORTC_clr(~value & mask);
			}
		break;
		
		case 3: //PORTD
			if(!validatePortAction(runningConfiguration.topConfiguration.portConfig_PD, mask, true, read))
				return false;
			
			if(!read)
			{
				HAL_GPIO_PORTD_set(value & mask);
				HAL_GPIO_PORTD_clr(~value & mask);
			}
		break;
		
		case 4: //PORTE
			if(!validatePortAction(runningConfiguration.topConfiguration.portConfig_PE, mask, true, read))
				return false;
			
			if(!read)
			{
				HAL_GPIO_PORTE_set(value & mask);
				HAL_GPIO_PORTE_clr(~value & mask);
			}
		break;
		
		case 5: //PORTF
			if(!validatePortAction(runningConfiguration.topConfiguration.portConfig_PF, mask, true, read))
				return false;
			
			if(!read)
			{
				HAL_GPIO_PORTF_set(value & mask);
				HAL_GPIO_PORTF_clr(~value & mask);
			}
		break;
		
		case 6: //PORTG
			if(!validatePortAction(runningConfiguration.topConfiguration.portConfig_PG, mask, true, read))
				return false;
			
			if(!read)
			{
				HAL_GPIO_PORTG_set(value & mask);
				HAL_GPIO_PORTG_clr(~value & mask);
			}
		break;
		
		default:
			return false;
		break;
	}
	
	if(read)
	{
		digitalResponse.value = lastValuesD[port];
		digitalResponse.dir = sourceAddress;
		//TODO: Send a DIGITAL_READ_RESPONSE_MESSAGE_t
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