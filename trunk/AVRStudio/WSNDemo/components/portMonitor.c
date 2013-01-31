/*
* portMonitor.c
*
* Created: 25/01/2013 17:27:24
*  Author: Victor
*/


#include "portMonitor.h"

_Bool firstTime = 1;
uint8_t lastValuesD[] = {0,0,0,0,0};//Ha de inicializarse con los valores al iniciar
uint8_t lastValuesA[] = {0,0,0,0,0,0,0,0};
//uint8_t lastChangeSign = 0; //0 -> positive  1 -> negative	

void PortMonitor_TaskHandler()
{
	for(uint8_t port=0; port<5; port++)
	{
		uint8_t val;
		PORT_CONFIG_t config;
		_Bool changeOcurred = 0;
	
		switch(port)
		{
			case 0: val = HAL_GPIO_PORTB_read(); config = runningConfiguration.topConfiguration.portConfig_PB; break;
			case 1: val = HAL_GPIO_PORTD_read(); config = runningConfiguration.topConfiguration.portConfig_PD; break;
			case 2: val = HAL_GPIO_PORTE_read(); config = runningConfiguration.topConfiguration.portConfig_PE; break;
			case 3: val = HAL_GPIO_PORTF_read(); config = runningConfiguration.topConfiguration.portConfig_PF; break;
			case 4: val = HAL_GPIO_PORTG_read(); config = runningConfiguration.topConfiguration.portConfig_PG; break;
		}

		for(uint8_t pin=0; pin<8; pin++)
		{
			if(((~config.maskIO)>>pin) & 0x01) //Input
			{
				if((config.maskAD>>pin) & 0x01) //Digital
				{
					//Check for valid changes
					switch((config.changeTypeD>>(pin<<1)))
					{
						case FALLING_EDGE: changeOcurred = ( ((lastValuesD[port]>>pin) & 0x01) & (~((val>>pin) & 0x01)) ); break;
						case RISIN_EDGE: changeOcurred = ( (~((lastValuesD[port]>>pin) & 0x01)) & ((val>>pin) & 0x01) ); break;
						case BOTH_EDGE: changeOcurred = ( ((lastValuesD[port]>>pin) & 0x01) != ((val>>pin) & 0x01) ); break;
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
					
					//int16_t diff = lastValuesA[pin] - analog_val;
					//if(diff>=0) //positive
					//{
						//changeOcurred = diff > analog_config.increment;
						//lastChangeSign &= ~(1<<pin);
					//}else
					//{
						//lastChangeSign |= (1<<pin);						
					//}
					
					if(firstTime || (changeOcurred = (abs(lastValuesA[pin] - analog_val) >= analog_config.increment)))
					{
						lastValuesA[pin] = analog_val;	
					}
				}
			}
			
			//Launch Events if exits
			if(!firstTime && changeOcurred)
			{
				launchEvents((port*8) + pin);
			}
		}
			
		lastValuesD[port] = val;
	}
	firstTime = 0;
}

void launchEvents(uint8_t pinAddress)
{
	//TODO: Update from uint8_t to uint16_t!!
	uint16_t pin_event_list_start_addr_rel = runningConfiguration.raw[EVENT_TABLE_ADDR + (pinAddress * 2)]; //Event address relative to the end of the event table
	uint16_t pin_event_list_end_addr_rel = runningConfiguration.raw[EVENT_TABLE_ADDR + (pinAddress * 2) + 2];
	
	uint16_t table_restriction_addr_rel = runningConfiguration.raw[EVENT_TABLE_ADDR + (NUM_PINS * 2) + (2 * 2)];//Relative
	
	uint16_t table_enable_addr_rel = runningConfiguration.raw[EVENT_TABLE_ADDR + (NUM_PINS * 2) + (3 * 2)];//Event flags address relative to the end of the event table
	uint8_t enable_pin_number_rel = runningConfiguration.raw[EVENT_TABLE_END_ADDR + table_enable_addr_rel + pinAddress];
	uint8_t enable_pin_addr_rel = (enable_pin_number_rel / 8); //Relative to the end of the enable event table

	uint16_t enable_pin_addr_absolute = (EVENT_TABLE_END_ADDR + table_enable_addr_rel + NUM_PINS + 2) + enable_pin_addr_rel;
	uint8_t pin_ptr = (enable_pin_number_rel - (enable_pin_addr_rel * 8)); //Rest of division
	
	uint16_t res_ptr;
	//Looks for the first temporary restriction applicable to the current list of events. (Address greater or equal than the first event in the list to be processed)
	for(res_ptr = table_restriction_addr_rel; res_ptr < table_enable_addr_rel; res_ptr += sizeof(EVENT_RESTRICTION_t))
	{
		EVENT_RESTRICTION_t* restric = (EVENT_RESTRICTION_t*)&runningConfiguration.raw[res_ptr + EVENT_TABLE_END_ADDR];
			
		if(restric->eventAddress >= pin_event_list_start_addr_rel) break;
	}
	
	//Iterate the list of events for the current pin and checks to be satisfied temporary restrictions (if any) and that the enable flag is set
	for(uint16_t event_ptr=pin_event_list_start_addr_rel; event_ptr < pin_event_list_end_addr_rel;)
	{
		EVENT_RESTRICTION_t* restric = (EVENT_RESTRICTION_t*)&runningConfiguration.raw[EVENT_TABLE_END_ADDR + res_ptr];
		EVENT_HEADER_t* event_header = (EVENT_HEADER_t*)&runningConfiguration.raw[EVENT_TABLE_END_ADDR + event_ptr];//DestAddress AND OP_CODE
		
		uint8_t args_length = getCommandArgsLenght(&event_header->operation);
		
		_Bool restriction_passed = !(restric->eventAddress == event_ptr);
		
		if( restric->eventAddress == event_ptr ) //Restriction for the current event?
		{
			restriction_passed |= ( (compareTimes(restric->start, currentTime) <= 0) && (compareTimes(restric->end, currentTime) >= 0) ); //In time
			res_ptr += sizeof(EVENT_RESTRICTION_t);
			restric = (EVENT_RESTRICTION_t*)&runningConfiguration.raw[EVENT_TABLE_END_ADDR + res_ptr];
		}		 
		
		
		if( ((runningConfiguration.raw[enable_pin_addr_absolute] >> (8 - pin_ptr)) & 0x01) && //Is enabled 
			restriction_passed ) //if time restriction ok
		{
			RF_Send_Event(event_header);
		}
		
		pin_ptr++;
		if(pin_ptr == 8)
		{
			pin_ptr = 0;
			enable_pin_addr_absolute++;
		}
		
		event_ptr += args_length + sizeof(EVENT_HEADER_t);
	}
}