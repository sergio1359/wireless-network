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
	unsigned char hour;
	unsigned char minute;
	unsigned char second; 
}TIME_t;

typedef struct{
	unsigned char date;
	unsigned char month;
	unsigned int year;
}DATE_t;

void RTC_Init();
void RTC_ValidateTime(TIME_t *receivedTime);
int8_t compareTimes(TIME_t time1, TIME_t time2);

#endif /* RTC_H_ */