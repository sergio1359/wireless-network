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

uint8_t port_tst = 0, pin_tst = 0;

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
					/*
					if( ~((lastValuesD[port]>>pin) & 0x01) & ((val>>pin) & 0x01) )
					{
						HAL_UartPrint("BUTTON PIN P");
						switch(port)
						{
							case 0: HAL_UartWriteByte('A');break;
							case 1: HAL_UartWriteByte('B');break;
							case 2: HAL_UartWriteByte('C');break;
							case 3: HAL_UartWriteByte('D');break;
							case 4: HAL_UartWriteByte('E');break;
							case 5: HAL_UartWriteByte('F');break;
							case 6: HAL_UartWriteByte('G');break;
						}
						numWrite(pin);
						HAL_UartPrint(" PRESSED\r\n");
					}*/
					
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
			
			//Launch Events if exits
			if(!firstTime && changeOcurred)
			{
				//launchEvents((port*8) + pin);
			}
		}
			
		lastValuesD[port] = val;
	}
	firstTime = 0;
	debounce_prt = 0;
}

void launchEvents(uint8_t pinAddress)
{
	uint16_t pin_event_list_start_addr_rel = runningConfiguration.raw[EVENT_TABLE_ADDR + (pinAddress * 2)]; //Event address relative to the end of the event table
	uint16_t pin_event_list_end_addr_rel = runningConfiguration.raw[EVENT_TABLE_ADDR + (pinAddress * 2) + 2];
	
	if(pin_event_list_start_addr_rel - pin_event_list_end_addr_rel == 0) //Events
		return;
	
	uint16_t res_ptr;
	//Looks for the first temporary restriction applicable to the current list of events. (Event address greater or equal than the list start address to process)
	for(res_ptr = EVENT_RESTRIC_LIST_START_ADDRESS; res_ptr < EVENT_RESTRIC_LIST_END_ADDRESS; res_ptr += sizeof(EVENT_RESTRICTION_t))
	{
		EVENT_RESTRICTION_t* restric = (EVENT_RESTRICTION_t*)&runningConfiguration.raw[EVENT_TABLE_END_ADDR + res_ptr];
		
		if(restric->eventAddress >= pin_event_list_start_addr_rel) break;
	}			
	
	//Iterate the list of events for the current pin and checks to be satisfied temporary restrictions (if any) and that the enable flag is set
	for(uint16_t event_ptr=pin_event_list_start_addr_rel; event_ptr < pin_event_list_end_addr_rel;)
	{
		EVENT_RESTRICTION_t* restric = (EVENT_RESTRICTION_t*)&runningConfiguration.raw[EVENT_TABLE_END_ADDR + res_ptr];
		EVENT_HEADER_t* event_header = (EVENT_HEADER_t*)&runningConfiguration.raw[EVENT_TABLE_END_ADDR + event_ptr];//DestAddress AND OP_CODE
		
		uint8_t args_length = getCommandArgsLenght(&event_header->operation);
		
		_Bool restriction_passed = (restric->eventAddress != event_ptr);
		
		while( (restric->eventAddress == event_ptr) && (event_ptr < EVENT_RESTRIC_LIST_END_ADDRESS) ) //Restriction for the current event? && Restrictions events available
		{
			restriction_passed |= ( (compareTimes(restric->start, currentTime) <= 0) && (compareTimes(restric->end, currentTime) >= 0) ); //In time
			res_ptr += sizeof(EVENT_RESTRICTION_t);
			restric = (EVENT_RESTRICTION_t*)&runningConfiguration.raw[EVENT_TABLE_END_ADDR + res_ptr]; //Next restriction
		}		 
		
		
		if( restriction_passed ) //If all restrictions are met
		{
			RF_Send_Event(event_header);
		}
		
		event_ptr += args_length + sizeof(EVENT_HEADER_t);
	}
}