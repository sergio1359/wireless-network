/*
 * timeManager.c
 *
 * Created: 25/05/2013 22:21:34
 *  Author: Victor
 */ 

#include "globals.h"
#include "timeManager.h"
#include "modulesManager.h"
#include "operationsManager.h"

#include "sysTimer.h"

static TIME_OPERATION_HEADER_t* time_operation_header;

static struct
{
	OPERATION_HEADER_t header;
	TIME_READ_MESSAGE_t request;
}timeSyncRequest;

static uint8_t timeSyncCounter;
static SYS_Timer_t timeSyncTimer;
static uint16_t currentNeighborAddress;
static TimeSyncState_t timeSyncState;

static void searchFirstTimeOperation();
static void timeSyncTimerHandler(SYS_Timer_t *timer);
static void timeSync_DataConf(OPERATION_DataConf_t *req);

void TIME_Init()
{
	RTC_Init();
	
	timeSyncState = TIME_SYNC_WAITING_NEXT_LOOP;
	timeSyncCounter = 2;
	currentNeighborAddress = 0;
	timeSyncRequest.header.opCode = DateTimeRead;
	
	//Configure Timer
	timeSyncTimer.interval = 1000;
	timeSyncTimer.mode = SYS_TIMER_PERIODIC_MODE;
	timeSyncTimer.handler = timeSyncTimerHandler;
	SYS_TimerStart(&timeSyncTimer);
}

void TIME_ValidateTime(TIME_t *receivedTime)
{
	memcpy((uint8_t*)&currentTime,(uint8_t*)receivedTime, sizeof(TIME_t));
	validTime = 1;
	
	if(validDate)
		searchFirstTimeOperation();
}

void TIME_ValidateDate(DATE_t *receivedDate, WEEKDAY_t *receivedWeek)
{
	memcpy((uint8_t*)&currentDate,(uint8_t*)receivedDate, sizeof(DATE_t));
	memcpy((uint8_t*)&currentWeek,(uint8_t*)receivedWeek, sizeof(WEEKDAY_t));
	validDate = 1;
	
	if(validTime)
		searchFirstTimeOperation();
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

int8_t TIME_CompareDates(DATE_t date1, DATE_t date2)
{
	if (date1.year > date2.year) return 1;
	if (date1.year < date2.year) return -1;
	if (date1.month > date2.month) return 1;
	if (date1.month < date2.month) return -1;
	if (date1.day > date2.day) return 1;
	if (date1.day < date2.day) return -1;
	return 0;
}

void TIME_CheckTimeOperation()
{
	if(VALID_DATETIME && (TIME_OPERATION_LIST_START_ADDRESS != TIME_OPERATION_LIST_END_ADDRESS))
	{
		while(TIME_CompareTimes(time_operation_header->activationTime, currentTime) == 0)
		{
			//TODO: USE OPERATION MANAGER!
			OM_ProccessInternalOperation(&time_operation_header->operationHeader);
			
			time_operation_header += sizeof(TIME_OPERATION_HEADER_t) + MODULES_GetCommandArgsLength(&time_operation_header->operationHeader.opCode);
			
			if(time_operation_header >= ((uint16_t)&runningConfiguration) + TIME_OPERATION_LIST_END_ADDRESS)
			{
				time_operation_header = (TIME_OPERATION_HEADER_t*)&runningConfiguration.raw[TIME_OPERATION_LIST_START_ADDRESS];
				break;
			}
		}
	}
}

void TIME_ProccessSyncResponse(WEEKDAY_t *receivedWeek, DATE_t *receivedDate, TIME_t *receivedTime)
{
	if(timeSyncState == TIME_SYNC_WAITING_SYNC_RESPONSE)
	{
		//Check if the received DATETIME is valid.
		if(receivedWeek->raw != 0xFF)
		{
			TIME_ValidateTime(receivedTime);
			TIME_ValidateDate(receivedDate, receivedWeek);	
			//TODO: Turn off timer?
			//SYS_TimerStop(&timeSyncTimer);
		}else
		{
			timeSyncState = TIME_SYNC_WAITING_SYNC_SEND;
			timeSyncCounter = 0;	
		}
	}
}


/************************************************************************/
/*                       INTERNAL METHODS                               */
/************************************************************************/


static void searchFirstTimeOperation()
{
	uint16_t operation_ptr;
	for(operation_ptr = TIME_OPERATION_LIST_START_ADDRESS; operation_ptr < TIME_OPERATION_LIST_END_ADDRESS;)
	{
		time_operation_header = (TIME_OPERATION_HEADER_t*)&runningConfiguration.raw[operation_ptr];
		
		if(TIME_CompareTimes(time_operation_header->activationTime, currentTime) >= 0)
			break;
		
		operation_ptr += sizeof(TIME_OPERATION_HEADER_t) + MODULES_GetCommandArgsLength(&time_operation_header->operationHeader.opCode);
	}
	
	if(operation_ptr >= TIME_OPERATION_LIST_END_ADDRESS)
	{
		time_operation_header = (TIME_OPERATION_HEADER_t*)&runningConfiguration.raw[TIME_OPERATION_LIST_START_ADDRESS];
	}
}

static void timeSyncTimerHandler(SYS_Timer_t *timer)
{
	if(!VALID_DATETIME)
	{
		if(timeSyncCounter > 0)
		{
			timeSyncCounter--;	
		}
		else if(timeSyncState == TIME_SYNC_WAITING_NEXT_LOOP)
		{
			//Refresh the table in 10 seconds..
			timeSyncState = TIME_SYNC_WAITING_DISCOVERY_SEND;
		}
		else if(timeSyncState == TIME_SYNC_WAITING_DISCOVERY_SEND)
		{
			//Send a discovery operation to refresh the neighbors table
			RADIO_SendDiscovery(timeSync_DataConf);
			timeSyncState = TIME_SYNC_WAITING_DISCOVERY_CONFIRM;
		}
		else if(timeSyncState == TIME_SYNC_WAITING_SYNC_RESPONSE)//TimeOut
		{
			//Skip this node at this iteration
			timeSyncState = TIME_SYNC_WAITING_SYNC_SEND;
		}				
		else if(timeSyncState == TIME_SYNC_WAITING_SYNC_SEND)
		{
			//Get the next neighbor address
			currentNeighborAddress = NWK_GetNextNeighborAddress(currentNeighborAddress);
			
			if(currentNeighborAddress == 0xFFFF)//Last neighbor
			{
				//Try again in 10 seconds..
				timeSyncState = TIME_SYNC_WAITING_NEXT_LOOP;
				timeSyncCounter = 10;	
			}else
			{
				//Send TimeSyncRequest
				timeSyncRequest.header.destinationAddress = currentNeighborAddress;
				timeSyncRequest.header.sourceAddress = runningConfiguration.topConfiguration.networkConfig.deviceAddress;
				
				timeSyncState = TIME_SYNC_WAITING_SYNC_CONFIRM;
				RADIO_AddMessageByReference(&timeSyncRequest.header, timeSync_DataConf);	
			}
		}
		
		DISPLAY_Clear();
		DISPLAY_WriteString("SENT TO: 0x");
		DISPLAY_WriteNumberHEX(currentNeighborAddress >> 8);
		DISPLAY_WriteNumberHEX(currentNeighborAddress & 0xFF);
		
		DISPLAY_SetCursor(0,1);
		switch (timeSyncState)
		{
			case TIME_SYNC_WAITING_DISCOVERY_SEND:
			DISPLAY_WriteString("DISC SEND");
			break;
			case TIME_SYNC_WAITING_DISCOVERY_CONFIRM:
			DISPLAY_WriteString("DISC CONF");
			break;
			case TIME_SYNC_WAITING_NEXT_LOOP:
			DISPLAY_WriteString("NEXT LOOP ");
			DISPLAY_WriteNumberDEC(timeSyncCounter, 2);
			break;
			case TIME_SYNC_WAITING_SYNC_SEND:
			DISPLAY_WriteString("SYNC SEND ");
			DISPLAY_WriteNumberDEC(timeSyncCounter, 2);
			break;
			case TIME_SYNC_WAITING_SYNC_CONFIRM:
			DISPLAY_WriteString("SYNC CONFIRM");
			break;
			case TIME_SYNC_WAITING_SYNC_RESPONSE:
			DISPLAY_WriteString("SYNC RESPONSE");
			break;
		}		
	}
	
	(void)timer;
}

static void timeSync_DataConf(OPERATION_DataConf_t *req)
{
	if(timeSyncState == TIME_SYNC_WAITING_DISCOVERY_CONFIRM)
	{
		if (req->sendOk)
		{
			timeSyncCounter = 5;//Waiting for responses before sending
			timeSyncState = TIME_SYNC_WAITING_SYNC_SEND;
		}else
		{
			//Try again
			timeSyncState = TIME_SYNC_WAITING_DISCOVERY_SEND;
		}
	}else if(timeSyncState == TIME_SYNC_WAITING_SYNC_CONFIRM)
	{
		if (req->sendOk)
		{
			timeSyncCounter = 5;//TimeOut
			timeSyncState = TIME_SYNC_WAITING_SYNC_RESPONSE;
		}else
		{
			//Skip this node at this iteration
			timeSyncState = TIME_SYNC_WAITING_DISCOVERY_SEND;
		}	
	}else
	{
		//TODO:  Send or log ERROR (UNEXPECTED_TIME_SYNC_STATUS)
	}
}