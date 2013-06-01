/*
 * config_module.c
 *
 * Created: 31/05/2013 19:41:10
 *  Author: Victor
 */ 
#include "modules.h"
#include "globals.h"

_Bool receivingState;

RUNNING_CONFIGURATION_t configBuffer;
uint16_t index;
uint8_t currentFragment;
uint8_t totalExpected;

void configModule_Init(void)
{
	receivingState = false;
	index = 0;
}

void configModule_NotificationInd(uint8_t sender, OPERATION_HEADER_t* notification)
{
	
}

CONFIG_WRITE_HEADER_MESSAGE_t* msg;
void configWrite_Handler(OPERATION_HEADER_t* operation_header)
{
	if(operation_header->opCode == ConfigWrite)
	{
		_Bool acceptFragment = false;
		msg = (CONFIG_WRITE_HEADER_MESSAGE_t*)(operation_header + 1);
		if(receivingState)
		{
			if(msg->fragmentTotal != totalExpected)
			{
				receivingState = false;
				//TODO:SEND ERROR MESSAGE (FRAGMENT TOTAL NOT EXPECTED)
			}else if(msg->fragment == (currentFragment+1))
			{
				currentFragment++;
				acceptFragment= true;
			}else
			{
				receivingState = false;
				//TODO:SEND ERROR MESSAGE (ERROR FRAGMENT ORDER)
			}
		}else if(msg->fragment == 0) //FirstFrame
		{
			index = 0;
			receivingState = true;
			totalExpected = msg->fragmentTotal;
			acceptFragment = true;
		}else
		{
			receivingState = false;
			//TODO:SEND ERROR MESSAGE (ERROR WAITING FIRST FRAGMENT)
		}
		
		if(acceptFragment)
		{
			memcpy((uint8_t*)configBuffer.raw, (uint8_t*)(msg + 1), 3);
			index += msg->length;
			
			if(currentFragment == msg->fragmentTotal)//ALL RECEIVED
			{
				totalExpected = 0;
			}
		}
	}
}

void configRead_Handler(OPERATION_HEADER_t* operation_header)
{
	
}

void configChecksum_Handler(OPERATION_HEADER_t* operation_header)
{
	
}