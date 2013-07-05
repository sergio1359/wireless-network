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
#include "modulesManager.h"
#include "operationsManager.h"


#include "leds.h"

#define REFERENCES_BUFFER_SIZE 64
#define COPIES_BUFFER_SIZE 256

#define REFERENCES_BUFFER_SIZE_MASK REFERENCES_BUFFER_SIZE-1
#define COPIES_BUFFER_SIZE_MASK COPIES_BUFFER_SIZE-1

#define DEFAULT_RETRIES_LIMIT 6
#define TIME_BW_RETRIES 500 //milliseconds
 
/* Circular buffer object */
CREATE_CIRCULARBUFFER(OPERATION_HEADER_t*, REFERENCES_BUFFER_SIZE)	referencesMessages_Buffer;
CREATE_CIRCULARBUFFER(uint8_t, COPIES_BUFFER_SIZE)					copiesMessages_Buffer;

RadioState_t currentState = RF_STATE_INITIAL;
NWK_DataReq_t nwkDataReq;
uint8_t failRetries;
SYS_Timer_t retriesTimer;

inline _Bool addMessageByCopy(OPERATION_HEADER_t* message, uint8_t size, uint8_t* body, uint8_t bodySize);
inline unsigned int freeSpace(unsigned int start, unsigned int end, unsigned int size);
void sendNextMessage(void);
static void rfDataConf(NWK_DataReq_t *req);
static void rfDataInd(NWK_DataInd_t *ind);
static void retriesTimerHandler(SYS_Timer_t *timer);

void Radio_Init()
{
	failRetries = 0;
	
	retriesTimer.interval = TIME_BW_RETRIES; 
	retriesTimer.mode = SYS_TIMER_INTERVAL_MODE;
	retriesTimer.handler = retriesTimerHandler;
	
	if(validConfiguration)
	{
		NWK_SetAddr(runningConfiguration.topConfiguration.networkConfig.deviceAddress);
		NWK_SetPanId(runningConfiguration.topConfiguration.networkConfig.panId);
		PHY_SetChannel(runningConfiguration.topConfiguration.networkConfig.channel);
		
		#ifdef NWK_ENABLE_SECURITY
		NWK_SetSecurityKey(runningConfiguration.topConfiguration.networkConfig.securityKey);
		#endif
	}else
	{
		NWK_SetAddr(APP_ADDR);
		NWK_SetPanId(APP_PANID);
		PHY_SetChannel(APP_CHANNEL);
		
		runningConfiguration.topConfiguration.networkConfig.networkRetries = DEFAULT_RETRIES_LIMIT;
		
		#ifdef NWK_ENABLE_SECURITY
		NWK_SetSecurityKey((uint8_t *)APP_SECURITY_KEY);
		#endif
	}
	
	PHY_SetRxState(true);

	NWK_OpenEndpoint(APP_ENDPOINT, rfDataInd);
	
	currentState = RF_STATE_READY_TO_SEND;
}

_Bool Radio_AddMessageByCopy(OPERATION_HEADER_t* message)
{
	return addMessageByCopy(message, sizeof(OPERATION_HEADER_t) + getCommandArgsLength(&message->opCode), 0, 0);
}


_Bool Radio_AddMessageWithBodyByCopy(OPERATION_HEADER_t* message, uint8_t* body, uint8_t bodySize)
{
	return addMessageByCopy(message, sizeof(OPERATION_HEADER_t) + getCommandArgsLength(&message->opCode) - bodySize, body, bodySize);
}

inline _Bool addMessageByCopy(OPERATION_HEADER_t* message, uint8_t size, uint8_t* body, uint8_t bodySize)
{
	if(freeSpace(copiesMessages_Buffer.start, copiesMessages_Buffer.end, COPIES_BUFFER_SIZE) >= (size + bodySize))
	{
		for(int i = 0; i < size; i++)
		{
			copiesMessages_Buffer.buffer[copiesMessages_Buffer.end + i] = *((uint8_t*)message + i);
		}
		copiesMessages_Buffer.end += size;
		copiesMessages_Buffer.end &= COPIES_BUFFER_SIZE_MASK;
		
		for(int i = 0; i < bodySize; i++)
		{
			copiesMessages_Buffer.buffer[copiesMessages_Buffer.end + i] = body[i];
		}
		copiesMessages_Buffer.end += bodySize;		
		copiesMessages_Buffer.end &= COPIES_BUFFER_SIZE_MASK;
		
		sendNextMessage();
		
		return true;
	}else
	{
		//TODO: Send or Log ERROR (COPIES_BUFFER_FULL)
		return false;
	}
}

_Bool Radio_AddMessageByReference(OPERATION_HEADER_t* message)
{
	if(freeSpace(referencesMessages_Buffer.start, referencesMessages_Buffer.end, REFERENCES_BUFFER_SIZE) >= 1)
	{
		referencesMessages_Buffer.buffer[referencesMessages_Buffer.end++] = message;
		referencesMessages_Buffer.end &= REFERENCES_BUFFER_SIZE_MASK;
		
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
inline unsigned int freeSpace(unsigned int start, unsigned int end, unsigned int size)
{
	if(start <= end)
		return size - (end - start);
	else
		return start - end;
}	

/*****************************************************************************
*****************************************************************************/
void sendNextMessage()
{
	//TODO: Avoid this situation
	if(currentState != RF_STATE_READY_TO_SEND)
		return;
	
	OPERATION_HEADER_t* currentOP;
	uint8_t length;
	if(referencesMessages_Buffer.start != referencesMessages_Buffer.end)
	{
		currentOP = referencesMessages_Buffer.buffer[referencesMessages_Buffer.start];
		length = sizeof(OPERATION_HEADER_t) + getCommandArgsLength(&currentOP->opCode);
		
		referencesMessages_Buffer.start++;
		referencesMessages_Buffer.start &= REFERENCES_BUFFER_SIZE_MASK;
	}else if(copiesMessages_Buffer.start != copiesMessages_Buffer.end)
	{
		currentOP = (OPERATION_HEADER_t*)&copiesMessages_Buffer.buffer[copiesMessages_Buffer.start];
		length = sizeof(OPERATION_HEADER_t) + getCommandArgsLength(&currentOP->opCode);
		
		copiesMessages_Buffer.start += length;
		copiesMessages_Buffer.start &= COPIES_BUFFER_SIZE_MASK;
	}else
	{
		return; //Nothing to send
	}		
			
	nwkDataReq.dstAddr = currentOP->destinationAddress;
	nwkDataReq.dstEndpoint = APP_ENDPOINT;
	nwkDataReq.srcEndpoint = APP_ENDPOINT;
	nwkDataReq.options = NWK_OPT_ACK_REQUEST | NWK_OPT_ENABLE_SECURITY;
	nwkDataReq.data = (uint8_t *)currentOP;
	nwkDataReq.size = length;
	nwkDataReq.confirm = rfDataConf;

	currentState = RF_STATE_WAIT_CONF;
		
	NWK_DataReq(&nwkDataReq);
}

/*****************************************************************************
*****************************************************************************/
static void rfDataConf(NWK_DataReq_t *req)
{
	//ledOff(LED_DATA);

	if (NWK_SUCCESS_STATUS == req->status)
	{
		failRetries = 0;
		currentState = RF_STATE_READY_TO_SEND;
		sendNextMessage();
	}
	else //NETWORK_PROBLEM
	{
		if(IS_COORDINATOR)
		{
			DISPLAY_Clear();
			switch(req->status)
			{
				case NWK_ERROR_STATUS:
					DISPLAY_WriteString("NWK_ERROR_STATUS"); break;
				case NWK_OUT_OF_MEMORY_STATUS:
					DISPLAY_WriteString("NWK_OUT_OF_MEMORY_STATUS"); break;
				case NWK_NO_ACK_STATUS:
					DISPLAY_WriteString("NWK_NO_ACK_STATUS"); break;//COMMON
				case NWK_PHY_CHANNEL_ACCESS_FAILURE_STATUS://COMMON
					DISPLAY_WriteString("NWK_PHY_CHANNEL_ACCESS_FAILURE_STATUS"); break;
				case NWK_PHY_NO_ACK_STATUS:
					DISPLAY_WriteString("NWK_PHY_NO_ACK_STATUS"); break;//COMMON
			}
		}
		
		failRetries++;
		
		if(failRetries == runningConfiguration.topConfiguration.networkConfig.networkRetries)
		{
			failRetries = 0;
			
			//Discard message
			currentState = RF_STATE_READY_TO_SEND;
			sendNextMessage();
			
			//TODO: Send or log ERROR (LIMIT_EXCEDED)
		}else
		{
			//Retry
			SYS_TimerStart(&retriesTimer);
		}
		
	}//TODO:  Send or log ERROR (UNEXPECTED_NETWORK_STATUS)
		
}

/*****************************************************************************
*****************************************************************************/
static void retriesTimerHandler(SYS_Timer_t *timer)
{
	NWK_DataReq(&nwkDataReq);

	(void)timer;
}

/*****************************************************************************
*****************************************************************************/
static void rfDataInd(NWK_DataInd_t *ind)
{
	/*TODO: Check this:
	 NWK_IND_OPT_ACK_REQUESTED Acknowledgment was requested
	>NWK_IND_OPT_SECURED Frame was encrypted
	>NWK_IND_OPT_BROADCAST Frame was sent to a broadcast address (0xffff)
	>NWK_IND_OPT_LOCAL Frame was received from a directly accessible node
	>NWK_IND_OPT_BROADCAST_PAN_ID Frame was sent to a broadcast PAN ID (0xffff)
	NWK_IND_OPT_LINK_LOCAL Frame was sent with a Link Local field set to 1
	NWK_IND_OPT_MULTICAST Frame was sent to a group address
	*/
	OM_ProccessExternalOperation((OPERATION_HEADER_t*)ind->data);		
}