/*
* portMonitor.c
*
* Created: 25/01/2013 17:27:24
*  Author: Victor
*/


#include "portMonitor.h"

_Bool firstTime = 1; //Avoid launch events on the first iteration
uint8_t lastValuesD[NUM_PORTS];
uint8_t lastValuesA[ANALOG_PINS];
//uint8_t lastChangeSign = 0; //0 -> positive  1 -> negative

uint8_t debounceBuffer[DEBOUNCED_PINS];
uint8_t debounce_prt = 0;

uint8_t port_tst = 0, pin_tst = 0, val_tst = 0;

/* WRITE PORTS*/
void PortManager_TaskHandler()
{
	
}

/* READ PORTS*/
void PortMonitor_TaskHandler()
{
	for(uint8_t port=0; port<NUM_PORTS; port++)
	{
		uint8_t val;
		PORT_CONFIG_t config;
		_Bool changeOcurred = 0;
		
		switch(port)
		{
			case 0: val = HAL_GPIO_PORTA_read(); config = runningConfiguration.topConfiguration.portConfig_PA; break;
			case 1: val = HAL_GPIO_PORTB_read(); config = runningConfiguration.topConfiguration.portConfig_PB; break;
			case 2: val = HAL_GPIO_PORTC_read(); config = runningConfiguration.topConfiguration.portConfig_PC; break;
			case 3: val = HAL_GPIO_PORTD_read(); config = runningConfiguration.topConfiguration.portConfig_PD; break;
			case 4: val = HAL_GPIO_PORTE_read(); config = runningConfiguration.topConfiguration.portConfig_PE; break;
			//case 5: val = HAL_GPIO_PORTF_read(); config = runningConfiguration.topConfiguration.portConfig_PF; break;
			case 5: val = HAL_GPIO_PORTF_read(); config.maskAD = 0; break;
			case 6: val = HAL_GPIO_PORTG_read(); config = runningConfiguration.topConfiguration.portConfig_PG; break;
		}

		for(uint8_t pin=0; pin<8; pin++)
		{
			port_tst = port;
			pin_tst = pin;
			val_tst = val;
			
			if(((~config.maskIO)>>pin) & 0x01) //Input
			{
				if((config.maskAD>>pin) & 0x01) //Digital
				{
					//Debouncer
					debounceBuffer[debounce_prt] = debounceBuffer[debounce_prt]<<1 | ((val>>pin) & 0x01);
					if(debounceBuffer[debounce_prt] == 0xFF)
					{
						val |=  (1 << pin);	//Set pin value
					}else
					{
						val &= ~(1 << pin);	//Clear pin value
					}
					debounce_prt++;
					
					if( ((lastValuesD[port]>>pin) & 0x01) & ~((val>>pin) & 0x01) & (port == 3 & pin == 7))
						HAL_UartPrint("BUTTON PRESSED\r\n");
					
					//Check for valid changes
					switch((config.changeTypeD>>(pin<<1)))
					{
						case FALLING_EDGE:	changeOcurred = (  ((lastValuesD[port]>>pin) & 0x01) && ~((val>>pin) & 0x01) ); break;
						case RISIN_EDGE:	changeOcurred = ( ~((lastValuesD[port]>>pin) & 0x01) &&  ((val>>pin) & 0x01) ); break;
						case BOTH_EDGE:		changeOcurred = (  ((lastValuesD[port]>>pin) & 0x01) !=  ((val>>pin) & 0x01) ); break;
						case NO_EDGE :		changeOcurred = 0; break;
					}
				}else //Analog
				{
					uint8_t analog_val = ADC_Read(pin);
					ANALOG_EVENT_CONFIG_t analog_config;
					
					switch(pin)
					{
						case ADC0: analog_config = runningConfiguration.topConfiguration.analogConfig_ADC0; break;
						case ADC1: analog_config = runningConfiguration.topConfiguration.analogConfig_ADC1; break;
						case ADC2: analog_config = runningConfiguration.topConfiguration.analogConfig_ADC2; break;
						case ADC3: analog_config = runningConfiguration.topConfiguration.analogConfig_ADC3; break;
						case ADC4: analog_config = runningConfiguration.topConfiguration.analogConfig_ADC4; break;
						case ADC5: analog_config = runningConfiguration.topConfiguration.analogConfig_ADC5; break;
						case ADC6: analog_config = runningConfiguration.topConfiguration.analogConfig_ADC6; break;
						case ADC7: analog_config = runningConfiguration.topConfiguration.analogConfig_ADC7; break;
					}
					
					/*int16_t diff = lastValuesA[pin] - analog_val;
					if(diff>=0) //positive
					{
						changeOcurred = diff > analog_config.increment;
						lastChangeSign &= ~(1<<pin);
					}else
					{
						lastChangeSign |= (1<<pin);
					}*/
					
					if(firstTime || (changeOcurred = (abs(lastValuesA[pin] - analog_val) >= analog_config.increment)))
					{
						lastValuesA[pin] = analog_val;
					}
				}
			}
			
			//Launch Operations if exits
			if(!firstTime && changeOcurred)
			{
				//launchOperations((port*8) + pin);
			}
		}
		
		lastValuesD[port] = val;
	}
	firstTime = 0;
	debounce_prt = 0;
}

void launchOperations(uint8_t pinAddress)
{
	uint16_t pin_operation_list_start_addr_rel = runningConfiguration.raw[OPERATION_TABLE_ADDR + (pinAddress * 2)]; //Operation address relative to the end of the operation table
	uint16_t pin_operation_list_end_addr_rel = runningConfiguration.raw[OPERATION_TABLE_ADDR + (pinAddress * 2) + 2];
	
	if(pin_operation_list_start_addr_rel - pin_operation_list_end_addr_rel == 0) //Operations
	return;
	
	uint16_t res_ptr;
	//Looks for the first temporary restriction applicable to the current list of operations. (Operation address greater or equal than the list start address to process)
	for(res_ptr = OPERATION_RESTRIC_LIST_START_ADDRESS; res_ptr < OPERATION_RESTRIC_LIST_END_ADDRESS; res_ptr += sizeof(OPERATION_RESTRICTION_t))
	{
		OPERATION_RESTRICTION_t* restric = (OPERATION_RESTRICTION_t*)&runningConfiguration.raw[OPERATION_TABLE_END_ADDR + res_ptr];
		
		if(restric->operationAddress >= pin_operation_list_start_addr_rel) break;
	}
	
	//Iterate the list of operations for the current pin and checks to be satisfied temporary restrictions (if any) and that the enable flag is set
	for(uint16_t operation_ptr=pin_operation_list_start_addr_rel; operation_ptr < pin_operation_list_end_addr_rel;)
	{
		OPERATION_RESTRICTION_t* restric = (OPERATION_RESTRICTION_t*)&runningConfiguration.raw[OPERATION_TABLE_END_ADDR + res_ptr];
		OPERATION_HEADER_t* operation_header = (OPERATION_HEADER_t*)&runningConfiguration.raw[OPERATION_TABLE_END_ADDR + operation_ptr];//DestAddress AND OP_CODE
		
		uint8_t args_length = getCommandArgsLenght(&operation_header->opCode);
		
		_Bool restriction_passed = (restric->operationAddress != operation_ptr);
		
		while( (restric->operationAddress == operation_ptr) && (operation_ptr < OPERATION_RESTRIC_LIST_END_ADDRESS) ) //Restriction for the current operation? && Restrictions operations available
		{
			restriction_passed |= ( (compareTimes(restric->start, currentTime) <= 0) && (compareTimes(restric->end, currentTime) >= 0) ); //In time
			res_ptr += sizeof(OPERATION_RESTRICTION_t);
			restric = (OPERATION_RESTRICTION_t*)&runningConfiguration.raw[OPERATION_TABLE_END_ADDR + res_ptr]; //Next restriction
		}
		
		
		if( restriction_passed ) //If all restrictions are met
		{
			OM_ProccessOperation(operation_header);
		}
		
		operation_ptr += args_length + sizeof(OPERATION_HEADER_t);
	}
}