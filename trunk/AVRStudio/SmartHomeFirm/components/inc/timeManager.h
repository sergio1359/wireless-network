/*
 * timeManager.h
 *
 * Created: 25/05/2013 22:21:52
 *  Author: Victor
 */ 


#ifndef TIMEMANAGER_H_
#define TIMEMANAGER_H_

#include <stdint.h>
#include "configManager.h"

#define VALID_DATETIME (validDate & validTime)

typedef enum _TimeSyncState_t
{
	TIME_SYNC_WAITING_DISCOVERY_SEND,
	TIME_SYNC_WAITING_DISCOVERY_CONFIRM,
	TIME_SYNC_WAITING_SYNC_SEND,
	TIME_SYNC_WAITING_SYNC_CONFIRM,
	TIME_SYNC_WAITING_SYNC_RESPONSE,
	TIME_SYNC_WAITING_NEXT_LOOP,
} TimeSyncState_t;

void TIME_Init(void);
void TIME_ValidateTime(TIME_t *receivedTime);
void TIME_ValidateDate(DATE_t *receivedDate, WEEKDAY_t *receivedWeek);
int8_t TIME_CompareTimes(TIME_t time1, TIME_t time2);
int8_t TIME_CompareDates(DATE_t date1, DATE_t date2);
void   TIME_CheckTimeOperation();
void   TIME_ProccessSyncResponse(WEEKDAY_t *receivedWeek, DATE_t *receivedDate, TIME_t *receivedTime);


#endif /* TIMEMANAGER_H_ */