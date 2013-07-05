/*
* RTC.c
*
* Created: 08/10/2012 19:58:33
*  Author: Victor
*/
#include "RTC.h"
#include "globals.h"
#include "timeManager.h"

char not_leap(void);

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
				
				if(currentWeek.flags.Sunday)
					currentWeek.raw = 1;	//Monday
				else
					currentWeek.raw <<= 1;	//Next day
				
				if (++currentDate.day==32)
				{
					currentDate.month++;
					currentDate.day=1;
				}
				else if (currentDate.day==31)
				{
					if ((currentDate.month==4) || (currentDate.month==6) || (currentDate.month==9) || (currentDate.month==11))
					{
						currentDate.month++;
						currentDate.day=1;
					}
				}
				else if (currentDate.day==30)
				{
					if(currentDate.month==2)
					{
						currentDate.month++;
						currentDate.day=1;
					}
				}
				else if (currentDate.day==29)
				{
					if((currentDate.month==2) && (not_leap()))
					{
						currentDate.month++;
						currentDate.day=1;
					}
				}
				if (currentDate.month==13)
				{
					currentDate.month=1;
					currentDate.year++;
				}
			}
		}
	}
	
	TIME_CheckTimeOperation();
}

char not_leap(void)      //check for leap year
{
	if (!(currentDate.year%100))
		return (char)(currentDate.year%400);
	else
		return (char)(currentDate.year%4);
}