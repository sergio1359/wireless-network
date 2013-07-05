/*
 * timeManager.h
 *
 * Created: 25/05/2013 22:21:52
 *  Author: Victor
 */ 


#ifndef TIMEMANAGER_H_
#define TIMEMANAGER_H_

#include <stdint.h>
#include "EEPROM.h"

#define VALID_DATETIME (validDate & validTime)

void TIME_ValidateTime(TIME_t *receivedTime);
void TIME_ValidateDate(DATE_t *receivedDate, WEEKDAY_t *receivedWeek);
int8_t TIME_CompareTimes(TIME_t time1, TIME_t time2);
int8_t TIME_CompareDates(DATE_t date1, DATE_t date2);
void   TIME_CheckTimeOperation();


#endif /* TIMEMANAGER_H_ */