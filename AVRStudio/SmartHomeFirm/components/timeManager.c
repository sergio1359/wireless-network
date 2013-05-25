/*
 * timeManager.c
 *
 * Created: 25/05/2013 22:21:34
 *  Author: Victor
 */ 

#include "timeManager.h"
#include "globals.h"
#include "modules.h"
#include "operationsManager.h"

TIME_OPERATION_HEADER_t* time_operation_header;

void searchFirstTimeOperation();

void TIME_Validate(TIME_t *receivedTime, DATE_t *receivedDate)
{
	memcpy((uint8_t*)receivedTime,(uint8_t*)&currentTime, sizeof(TIME_t));
	memcpy((uint8_t*)receivedDate,(uint8_t*)&currentDate, sizeof(DATE_t));
	
	searchFirstTimeOperation();
	validDateTime = 1;
}

int8_t TIME_CompareTimes(TIME_t time1, TIME_t time2)
{
	if (time1.hour > time2.hour) return 1;
	if (time1.hour < time2.hour) return -1;
	if (time1.minute > time2.minute) return 1;
	if (time1.minute < time2.minute) return -1;
	if (time1.second > time2.second) return 1;
	if (time1.second < time2.second) return -1;
	return 0;
}

void TIME_CheckTimeOperation()
{
	if(validDateTime)
	{
		while(TIME_CompareTimes(time_operation_header->activationTime, currentTime) == 0)
		{
			if(currentDate.weekDay.raw & time_operation_header->weekDays.raw != 0)
				OM_ProccessOperation(&time_operation_header->operationHeader, false);
			
			time_operation_header += sizeof(TIME_OPERATION_HEADER_t) + getCommandArgsLength(&time_operation_header->operationHeader.opCode);
			
			if(time_operation_header >= ((uint16_t)&runningConfiguration) + TIME_OPERATION_LIST_END_ADDRESS)
			{
				time_operation_header = (TIME_OPERATION_HEADER_t*)&runningConfiguration.raw[TIME_OPERATION_LIST_START_ADDRESS];
				break;
			}
		}
	}
}


void searchFirstTimeOperation()
{
	uint16_t operation_ptr;
	for(operation_ptr = TIME_OPERATION_LIST_START_ADDRESS; operation_ptr < TIME_OPERATION_LIST_END_ADDRESS;)
	{
		time_operation_header = (TIME_OPERATION_HEADER_t*)&runningConfiguration.raw[operation_ptr];
		
		if(TIME_CompareTimes(time_operation_header->activationTime, currentTime) >= 0)
		break;
		
		operation_ptr += sizeof(TIME_OPERATION_HEADER_t) + getCommandArgsLength(&time_operation_header->operationHeader.opCode);
	}
	
	if(operation_ptr >= TIME_OPERATION_LIST_END_ADDRESS)
	{
		time_operation_header = (TIME_OPERATION_HEADER_t*)&runningConfiguration.raw[TIME_OPERATION_LIST_START_ADDRESS];
	}
}