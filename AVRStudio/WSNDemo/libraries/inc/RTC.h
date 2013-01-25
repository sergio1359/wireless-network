/*
 * RTC.h
 *
 * Created: 08/10/2012 19:50:38
 *  Author: Victor
 */ 


#ifndef RTC_H_
#define RTC_H_

typedef struct{
	unsigned char second;   //enter the current time, date, month, and year
	unsigned char minute;
	unsigned char hour;
	//unsigned char date;
	//unsigned char month;
	//unsigned int year;
}TIME_t;

extern volatile TIME_t currentTime;

void RTC_Init();
void numWrite(unsigned int num);

#endif /* RTC_H_ */