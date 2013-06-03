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
uint8_t lastChangeSlopeSign = 0; //0 -> rising  1 -> decreasing

uint8_t debounceBuffer[DEBOUNCED_PINS];
uint8_t debounce_prt = 0;

uint8_t port_tst = 0, pin_tst = 0, val_tst = 0;

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
			case 5: val = HAL_GPIO_PORTF_read(); config = runningConfiguration.topConfiguration.portConfig_PF; break;
			//case 5: val = HAL_GPIO_PORTF_read(); config.maskAD = 0; break;
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
					if ((config.changeTypeD>>(pin<<1)) != NO_EDGE) //AND ChangeType != NONE
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
							case FALLING_EDGE:	changeOcurred = (  ((lastValuesD[port]>>pin) & 0x01) & ~((val>>pin) & 0x01) ); break;
							case RISIN_EDGE:	changeOcurred = ( ~((lastValuesD[port]>>pin) & 0x01) &  ((val>>pin) & 0x01) ); break;
							case BOTH_EDGE:		changeOcurred = (  ((lastValuesD[port]>>pin) & 0x01) !=  ((val>>pin) & 0x01) ); break;
						}
						
						if(changeOcurred)
							lastValuesD[port] = val; //Previous update for incoming request in module handlers
					}else
					{
						changeOcurred = 0;
					}
				}else if(port == 5) //Analog (Port F only)
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
					
					int16_t diff = analog_val - lastValuesA[pin];
					if(diff>0) //rising
					{
						if(lastChangeSlopeSign) //SlopeChange
						{
							if(changeOcurred = diff > analog_config.threshold)
								lastChangeSlopeSign &= ~(1<<pin);
						}else
						{
							changeOcurred = diff > analog_config.increment;
						}
					}else if((diff<0)) //decreasing
					{
						if(!lastChangeSlopeSign)//SlopeChange
						{
							if(changeOcurred = -diff > analog_config.threshold)
								lastChangeSlopeSign |= (1<<pin);
						}else
						{
							changeOcurred = -diff > analog_config.increment;
						}
					}
					
					if(firstTime || changeOcurred)
					{
						lastValuesA[pin] = analog_val;
					}
				}
			}
			
			//Launch Operations if exits
			if(!firstTime && changeOcurred)
			{
				PortMonitor_LaunchOperations((port*8) + pin);
			}
		}
		
		lastValuesD[port] = val;
	}
	firstTime = 0;
	debounce_prt = 0;
}


void PortMonitor_LaunchOperations(uint8_t pinAddress)
{
	uint16_t pin_operation_list_start_addr = runningConfiguration.raw[OPERATION_INDEX_START_ADDR + (pinAddress * sizeof(uint16_t))];
	uint16_t pin_operation_list_end_addr;
	
	if(pinAddress == (NUM_PINS - 1)) //Last pin
		pin_operation_list_end_addr = OPERATION_LIST_END_ADDR;
	else
		pin_operation_list_end_addr = runningConfiguration.raw[OPERATION_INDEX_START_ADDR + (pinAddress * sizeof(uint16_t)) + sizeof(uint16_t)];
	
	if(pin_operation_list_end_addr - pin_operation_list_start_addr == 0) //Operations
		return;
	
	uint16_t res_ptr;
	OPERATION_RESTRICTION_t* restric = (OPERATION_RESTRICTION_t*)&runningConfiguration.raw[OPERATION_RESTRIC_LIST_END_ADDRESS];
	//Looks for the first temporary restriction applicable to the current list of operations. (Operation address greater or equal than the list start address to process)
	for(res_ptr = OPERATION_RESTRIC_LIST_START_ADDRESS; res_ptr < OPERATION_RESTRIC_LIST_END_ADDRESS; res_ptr += sizeof(OPERATION_RESTRICTION_t))
	{
		restric = (OPERATION_RESTRICTION_t*)&runningConfiguration.raw[res_ptr];
		
		if(restric->operationAddress >= pin_operation_list_start_addr) break;
	}
	
	//Iterate the list of operations for the current pin and checks to be satisfied temporary restrictions (if any) and that the enable flag is set
	for(uint16_t operation_ptr = pin_operation_list_start_addr; operation_ptr < pin_operation_list_end_addr;)
	{
		OPERATION_HEADER_t* operation_header = (OPERATION_HEADER_t*)&runningConfiguration.raw[operation_ptr];
		
		_Bool restriction_passed = true;
		
		while( (restric->operationAddress == operation_ptr) && (res_ptr < OPERATION_RESTRIC_LIST_END_ADDRESS) ) //Restriction for the current operation? && Restrictions operations available
		{
			restriction_passed = ( (TIME_CompareTimes(restric->start, currentTime) <= 0) && (TIME_CompareTimes(restric->end, currentTime) >= 0) ); //In time
			restriction_passed &= ((currentDate.weekDay.raw & restric->weekDays.raw) != 0); //Day of the week
			res_ptr += sizeof(OPERATION_RESTRICTION_t);
			restric = (OPERATION_RESTRICTION_t*)&runningConfiguration.raw[res_ptr]; //Next restriction
		}
		
		if( restriction_passed ) //If all restrictions are met
		{
			OM_ProccessInternalOperation(operation_header, false);
		}
		
		operation_ptr += sizeof(OPERATION_HEADER_t) + getCommandArgsLength(&operation_header->opCode);
	}
}