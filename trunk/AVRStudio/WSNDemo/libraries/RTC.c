/*
 * RTC.c
 *
 * Created: 08/10/2012 19:58:33
 *  Author: Victor
 */ 
#include "RTC.h"
#include "globals.h"
#include "command.h"

#define TIME_EVENT_LIST_START_ADDRESS runningConfiguration.raw[EVENT_TABLE_ADDR + NUM_PINS + 1] //Time event list address relative to the end of the event table
#define TIME_EVENT_LIST_END_ADDRESS runningConfiguration.raw[EVENT_TABLE_ADDR + NUM_PINS + 2]
#define ENABLE_EVENT_TABLE_START_ADDRESS runningConfiguration.raw[EVENT_TABLE_ADDR + NUM_PINS + 3]

TIME_EVENT_HEADER_t* time_event_header;
uint8_t enable_flag_prt = 0;

void RTC_Init()
{
	TIMSK2 &=~((1<<TOIE2)|(1<<OCIE2B));     //Disable TC2 interrupt
	ASSR  &=~((1<<EXCLKAMR)|(1<<EXCLK));    // set clock source to external crystal AS2=1, EXCLK=0, EXCKLAMR=0
	ASSR |= (1<<AS2);              // alternate clock source
	
	TCNT2 = 0x00;
	TCCR2B = 0x05;
	
	while(ASSR&0x07);           //Wait until TC2 is updated
	TIMSK2 |= (1<<TOIE2);        //set 8-bit Timer/Counter2 Overflow Interrupt Enable
	sei();                     //set the Global Interrupt Enable Bit
	
	validTime = 0;
}

void Validate_Time(TIME_t *receivedTime)
{
	memcpy((uint8_t*)receivedTime,(uint8_t*)&currentTime, sizeof(TIME_t));
	
	searchFirstTimeEvent();
	validTime = 1;
}

void searchFirstTimeEvent()
{
	uint8_t event_ptr;
	
	for(event_ptr = TIME_EVENT_LIST_START_ADDRESS; event_ptr < TIME_EVENT_LIST_END_ADDRESS;)
	{
		time_event_header = (TIME_EVENT_HEADER_t*)&runningConfiguration.raw[event_ptr + EVENT_TABLE_END_ADDR];
		uint8_t args_length = getCommandArgsLenght(time_event_header->eventHeader.operation);
		
		if(compareTimes(time_event_header->activationTime, currentTime) <= 0) break;
		event_ptr += args_length + sizeof(TIME_EVENT_HEADER_t);
	}
	
	if(event_ptr >= TIME_EVENT_LIST_END_ADDRESS)
	{
		time_event_header = (TIME_EVENT_HEADER_t*)&runningConfiguration.raw[TIME_EVENT_LIST_START_ADDRESS + EVENT_TABLE_END_ADDR];
	}
}

void numWrite(unsigned int num)
{
	int aux;
	if(num > 9999)
	{
		aux = num/10000;
		HAL_UartWriteByte(aux+'0');
	}
	if(num > 999)
	{
		aux = num/1000;
		HAL_UartWriteByte(aux+'0');
	}
	if(num > 99)
	{
		aux = num/100;
		HAL_UartWriteByte(aux+'0');
	}
	if(num > 9)
	{
		aux = num/10;
		HAL_UartWriteByte(aux+'0');
	}	 
	HAL_UartWriteByte((num%10)+'0');
}

int8_t compareTimes(TIME_t time1, TIME_t time2)
{
		if (time1.hour > time2.hour) return 1;
		if (time1.hour < time2.hour) return -1;
		if (time1.minute > time2.minute) return 1;
		if (time1.minute < time2.minute) return -1;
		if (time1.second > time2.second) return 1;
		if (time1.second < time2.second) return -1;
		return 0;	
}

ISR(TIMER2_OVF_vect)  //overflow interrupt vector
{
	if (++currentTime.second==60)        //keep track of time
	{
		currentTime.second=0;
		if (++currentTime.minute==60)
		{
			currentTime.minute=0;
			if (++currentTime.hour==24)
			{
				currentTime.hour=0;
				/*if (++current_Time.date==32)
				{
					current_Time.month++;
					current_Time.date=1;
				}
				else if (current_Time.date==31)
				{
					if ((current_Time.month==4) || (current_Time.month==6) || (current_Time.month==9) || (current_Time.month==11))
					{
						current_Time.month++;
						current_Time.date=1;
					}
				}
				else if (current_Time.date==30)
				{
					if(current_Time.month==2)
					{
						current_Time.month++;
						current_Time.date=1;
					}
				}
				else if (current_Time.date==29)
				{
					if((current_Time.month==2) && (not_leap()))
					{
						current_Time.month++;
						current_Time.date=1;
					}
				}
				if (current_Time.month==13)
				{
					current_Time.month=1;
					current_Time.year++;
				}*/
			}
		}
	}
	
	//Check Time Events
	if(validTime)
	{
		while(compareTimes(time_event_header->activationTime, currentTime) == 0)
		{
			uint8_t enable_flag_number = runningConfiguration.raw[EVENT_TABLE_END_ADDR + ENABLE_EVENT_TABLE_START_ADDRESS + NUM_PINS + 1] + enable_flag_prt;
			
			uint8_t enable_flag_addr_relative = (enable_flag_number / 8); //Relative to the end of the enable event table
			uint8_t bit_ptr = (enable_flag_number - enable_flag_addr_relative);
			
			uint16_t enable_pin_addr_absolute = (EVENT_TABLE_END_ADDR + ENABLE_EVENT_TABLE_START_ADDRESS + NUM_PINS + 2) + enable_flag_addr_relative;
			
			//launch
			if( ((runningConfiguration.raw[enable_pin_addr_absolute] >> (8 - enable_flag_prt)) & 0x01) ) //Is enabled
			{
				RF_Send_Event(time_event_header - EVENT_TABLE_END_ADDR);//Relative address
			}
			
			time_event_header += getCommandArgsLenght(time_event_header->eventHeader.operation) + sizeof(TIME_EVENT_HEADER_t);
			enable_flag_prt++;
			
			if( (time_event_header - EVENT_TABLE_END_ADDR) >= TIME_EVENT_LIST_END_ADDRESS)
			{
				time_event_header = (TIME_EVENT_HEADER_t*)&runningConfiguration.raw[TIME_EVENT_LIST_START_ADDRESS + EVENT_TABLE_END_ADDR];
				enable_flag_prt = 0;
			}				
		}
	}
}