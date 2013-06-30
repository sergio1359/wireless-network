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
	unsigned Sunday : 1; //LSB
	unsigned Saturday : 1;
	unsigned Friday : 1;
	unsigned Thursday : 1;
	unsigned Wednesday : 1;
	unsigned Tuesday : 1;
	unsigned Monday : 1;
	unsigned Reserved : 1; //MSB
}WEEKDAYS_FLAG_t;

typedef union{
	WEEKDAYS_FLAG_t flags;
	uint8_t raw;
}WEEKDAY_t;	

typedef struct{
	unsigned char hour;
	unsigned char minute;
	unsigned char second; 
}TIME_t;

typedef struct{
	//WEEKDAY_t weekDay;
	unsigned char day;
	unsigned char month;
	unsigned int year;
}DATE_t;

void RTC_Init();

#endif /* RTC_H_ */