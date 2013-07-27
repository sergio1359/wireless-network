/*
 * time_module.c
 *
 * Created: 01/06/2013 14:11:49
 *  Author: Victor
 */ 

#include "globals.h"

#include "modulesManager.h"
#include "timeManager.h"

struct
{
	OPERATION_HEADER_t header;
	TIME_READ_RESPONSE_MESSAGE_t response;
}timeResponse;


void timeModule_Init(void)
{
	//Set responses opCodes
	timeResponse.header.opCode = DateTimeReadResponse;
}

void timeModule_NotificationInd(uint8_t sender, OPERATION_HEADER_t* notification)
{
	
}

void timeModule_DataConf(OPERATION_DataConf_t *req)
{
	
}

void time_Handler(OPERATION_HEADER_t* operation_header)
{
	if(operation_header->opCode == DateTimeWrite)
	{
		TIME_WRITE_MESSAGE_t* msg = (TIME_WRITE_MESSAGE_t*)(operation_header + 1);
		TIME_ValidateTime(&msg->time);
		TIME_ValidateDate(&msg->date, &msg->week);
		
	}else if(operation_header->opCode == DateTimeRead)
	{
		timeResponse.header.destinationAddress = operation_header->sourceAddress;
		timeResponse.header.sourceAddress = runningConfiguration.topConfiguration.networkConfig.deviceAddress;
		
		if(VALID_DATETIME)
		{
			timeResponse.response.week = currentWeek;
			timeResponse.response.date = currentDate;
			timeResponse.response.time = currentTime;
		}else
		{
			timeResponse.response.week.raw = 0xFF; // Datetime not valid
		}		
		
		OM_ProccessResponseOperation(&timeResponse.header);
	}else if(operation_header->opCode == DateTimeReadResponse)
	{
		//TODO: NOTIFICATION
		
		TIME_READ_RESPONSE_MESSAGE_t* msg = (TIME_READ_RESPONSE_MESSAGE_t*)(operation_header + 1);
		TIME_ProccessSyncResponse(&msg->week, &msg->date, &msg->time);
	}
}