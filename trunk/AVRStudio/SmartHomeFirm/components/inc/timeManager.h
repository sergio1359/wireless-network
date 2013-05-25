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

void TIME_Validate(TIME_t *receivedTime, DATE_t *receivedDate);
int8_t TIME_CompareTimes(TIME_t time1, TIME_t time2);
void   TIME_CheckTimeOperation();


#endif /* TIMEMANAGER_H_ */