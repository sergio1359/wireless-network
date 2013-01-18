/*
 * RTC.c
 *
 * Created: 08/10/2012 19:58:33
 *  Author: Victor
 */ 
#include "RTC.h"
#include "globals.h"
#include <avr/interrupt.h>

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

ISR(TIMER2_OVF_vect)  //overflow interrupt vector
{
	if (++current_Time.second==60)        //keep track of time
	{
		current_Time.second=0;
		if (++current_Time.minute==60)
		{
			current_Time.minute=0;
			if (++current_Time.hour==24)
			{
				current_Time.hour=0;
				//if (++current_Time.date==32)
				//{
					//current_Time.month++;
					//current_Time.date=1;
				//}
				//else if (current_Time.date==31)
				//{
					//if ((current_Time.month==4) || (current_Time.month==6) || (current_Time.month==9) || (current_Time.month==11))
					//{
						//current_Time.month++;
						//current_Time.date=1;
					//}
				//}
				//else if (current_Time.date==30)
				//{
					//if(current_Time.month==2)
					//{
						//current_Time.month++;
						//current_Time.date=1;
					//}
				//}
				//else if (current_Time.date==29)
				//{
					//if((current_Time.month==2) && (not_leap()))
					//{
						//current_Time.month++;
						//current_Time.date=1;
					//}
				//}
				//if (current_Time.month==13)
				//{
					//current_Time.month=1;
					//current_Time.year++;
				//}
			}
		}
	}
}