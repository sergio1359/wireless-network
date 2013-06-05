/*
 * time_module.c
 *
 * Created: 01/06/2013 14:11:49
 *  Author: Victor
 */ 

#include "modulesManager.h"
#include "globals.h"

struct
{
	OPERATION_HEADER_t header;
	TIME_READ_RESPONSE_MESSAGE_t response;
}timeResponse;

struct
{
	OPERATION_HEADER_t header;
	DATE_READ_RESPONSE_MESSAGE_t response;
}dateResponse;


void timeModule_Init(void)
{
	timeResponse.header.opCode = TimeReadResponse;
	dateResponse.header.opCode = DateReadResponse;
}

void timeModule_NotificationInd(uint8_t sender, OPERATION_HEADER_t* notification)
{
	
}

void time_Handler(OPERATION_HEADER_t* operation_header)
{
	if(operation_header->opCode == TimeWrite)
	{
		TIME_WRITE_MESSAGE_t* msg = (TIME_WRITE_MESSAGE_t*)(operation_header + 1);
		TIME_ValidateTime(&msg->time);
		
	}else if(operation_header->opCode == TimeRead)
	{
		timeResponse.header.destinationAddress = operation_header->sourceAddress;
		timeResponse.header.sourceAddress = runningConfiguration.topConfiguration.networkConfig.deviceAddress;
		timeResponse.response.time = currentTime;
		
		OM_ProccessResponseOperation(&timeResponse.header);
	}else if(operation_header->opCode == TimeReadResponse)
	{
		//TODO: NOTIFICATION
	}
}

void date_Handler(OPERATION_HEADER_t* operation_header)
{
	if(operation_header->opCode == DateWrite)
	{
		DATE_WRITE_MESSAGE_t* msg = (DATE_WRITE_MESSAGE_t*)(operation_header + 1);
		TIME_ValidateDate(&msg->date);
	}else if(operation_header->opCode == DateRead)
	{
		dateResponse.header.destinationAddress = operation_header->sourceAddress;
		dateResponse.header.sourceAddress = runningConfiguration.topConfiguration.networkConfig.deviceAddress;
		dateResponse.response.date = currentDate;
		
		OM_ProccessResponseOperation(&dateResponse.header);
	}else if(operation_header->opCode == DateReadResponse)
	{
		//TODO: NOTIFICATION
	}
}