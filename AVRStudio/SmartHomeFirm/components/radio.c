/*
 * radio.c
 *
 * Created: 28/01/2013 14:28:08
 *  Author: Victor
 */ 

#include "globals.h"
#include "nwk.h"
#include "sysTimer.h"

#include "radio.h"
#include "modules.h"

#define REFERENCES_BUFFER_SIZE 64
#define COPIES_BUFFER_SIZE 256

#define RETRIES_LIMIT 5
#define TIME_BW_RETRIES 500 //milliseconds
 
/* Circular buffer object */
CREATE_CIRCULARBUFFER(OPERATION_HEADER_t*, REFERENCES_BUFFER_SIZE)	referencesMessages_Buffer;
CREATE_CIRCULARBUFFER(uint8_t, COPIES_BUFFER_SIZE)		copiesMessages_Buffer;

BufferType_t currentBuffer = RF_BUFFER_NONE;
RadioState_t currentState = RF_STATE_INITIAL;
NWK_DataReq_t nwkDataReq;
uint8_t failRetries;
SYS_Timer_t retriesTimer;

void sendNextMessage(void);
static void rfDataConf(NWK_DataReq_t *req);
static void retriesTimerHandler(SYS_Timer_t *timer);

void Radio_Init()
{
	failRetries = 0;
	
	retriesTimer.interval = TIME_BW_RETRIES; 
	retriesTimer.mode = SYS_TIMER_INTERVAL_MODE;
	retriesTimer.handler = retriesTimerHandler;
	
	//TODO: Initialize network
	//...
	
	currentState = RF_STATE_READY_TO_SEND;
}

_Bool Radio_AddMessageByCopy(OPERATION_HEADER_t* message)
{
	uint8_t length = sizeof(OPERATION_HEADER_t) + getCommandArgsLength(&message->opCode);
	if(copiesMessages_Buffer.end - copiesMessages_Buffer.start >= length)
	{
		for(int i = 0; i < length; i++)
		{
			copiesMessages_Buffer.buffer[copiesMessages_Buffer.end + i] = *((uint8_t*)message + i);
		}
		copiesMessages_Buffer.end += length;
		
		sendNextMessage();
	}else
	{
		//TODO: Send or Log ERROR (COPIES_BUFFER_FULL)
		return false;
	}
	return true;
}

_Bool Radio_AddMessageByReference(OPERATION_HEADER_t* message)
{
	if(referencesMessages_Buffer.end - referencesMessages_Buffer.start >= 1)
	{
		referencesMessages_Buffer.buffer[referencesMessages_Buffer.end++] = message;
		
		sendNextMessage();
		
		return true;
	}else
	{
		//TODO: Send or Log ERROR (REFERENCES_BUFFER_FULL)
		return false;
	}
}

/*****************************************************************************
*****************************************************************************/
void sendNextMessage()
{
	if(currentState != RF_STATE_READY_TO_SEND)
		return;
	
	OPERATION_HEADER_t* currentOP;
	if(referencesMessages_Buffer.end - referencesMessages_Buffer.start > 0)
	{
		currentOP = referencesMessages_Buffer.buffer[referencesMessages_Buffer.start];
		currentBuffer = RF_BUFFER_REFERENCES;
	}else if(copiesMessages_Buffer.end - copiesMessages_Buffer.start > 0)
	{
		currentOP = (OPERATION_HEADER_t*)copiesMessages_Buffer.buffer[copiesMessages_Buffer.start];
		currentBuffer = RF_BUFFER_COPIES;
	}else
	{
		currentBuffer = RF_BUFFER_NONE;
		return; //Nothing to send
	}		
			
	nwkDataReq.dstAddr = currentOP->destinationAddress;
	nwkDataReq.dstEndpoint = APP_ENDPOINT;
	nwkDataReq.srcEndpoint = APP_ENDPOINT;
	nwkDataReq.options = NWK_OPT_ACK_REQUEST | NWK_OPT_ENABLE_SECURITY;
	nwkDataReq.data = (uint8_t *)currentOP;
	nwkDataReq.size = sizeof(OPERATION_HEADER_t) + getCommandArgsLength(&currentOP->opCode);
	nwkDataReq.confirm = rfDataConf;

	currentState = RF_STATE_WAIT_CONF;
		
	NWK_DataReq(&nwkDataReq);
}

/*****************************************************************************
*****************************************************************************/
static void rfDataConf(NWK_DataReq_t *req)
{
//	ledOff(LED_DATA);

	if (NWK_SUCCESS_STATUS == req->status)
	{
		failRetries = 0;
		currentState = RF_STATE_READY_TO_SEND;
		
		sendNextMessage();
	}
	else //NETWORK_PROBLEM
	{
		failRetries++;
		
		//Retry
		SYS_TimerStart(&retriesTimer);
		
		if(failRetries == RETRIES_LIMIT)
		{
			failRetries = 0;
			
			//Discard message
			if(currentBuffer == RF_BUFFER_REFERENCES)
				referencesMessages_Buffer.start++;
			else if(currentBuffer == RF_BUFFER_COPIES)
			{
				OPERATION_HEADER_t* currentOP = (OPERATION_HEADER_t*)copiesMessages_Buffer.buffer[copiesMessages_Buffer.start];
				copiesMessages_Buffer.start+= sizeof(OPERATION_HEADER_t) + getCommandArgsLength(&currentOP->opCode);
			}				
			
			
			currentState = RF_STATE_READY_TO_SEND;
			
			//TODO: Send or log ERROR (LIMIT_EXCEDED)
		}
		
	}//TODO: Check and log for non expected network status
}

/*****************************************************************************
*****************************************************************************/
static void retriesTimerHandler(SYS_Timer_t *timer)
{
	sendNextMessage();

	(void)timer;
}