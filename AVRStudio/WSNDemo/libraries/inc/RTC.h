/*
 * RTC.h
 *
 * Created: 08/10/2012 19:50:38
 *  Author: Victor
 */ 


#ifndef RTC_H_
#define RTC_H_
#include <avr/interrupt.h>

typedef struct{
	unsigned char second;   //enter the current time, date, month, and year
	unsigned char minute;
	unsigned char hour;
	//unsigned char date;
	//unsigned char month;
	//unsigned int year;
}TIME_t;

void RTC_Init();
int8_t compareTimes(TIME_t time1, TIME_t time2);

#endif /* RTC_H_ */