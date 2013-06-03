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
		
		runningConfiguration.topConfiguration.deviceInfo.networkRetriesLimit = DEFAULT_RETRIES_LIMIT;
		
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
	uint8_t length = sizeof(OPERATION_HEADER_t) + getCommandArgsLength(&message->opCode);
	if(freeSpace(copiesMessages_Buffer.start, copiesMessages_Buffer.end, COPIES_BUFFER_SIZE) >= length)
	{
		for(int i = 0; i < length; i++)
		{
			copiesMessages_Buffer.buffer[copiesMessages_Buffer.end + i] = *((uint8_t*)message + i);
		}
		copiesMessages_Buffer.end += length;
		copiesMessages_Buffer.end &= COPIES_BUFFER_SIZE_MASK;
		
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
		failRetries++;
		
		if(failRetries == runningConfiguration.topConfiguration.deviceInfo.networkRetriesLimit)
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
	//OM_ProccessExternalOperation((OPERATION_HEADER_t*)ind->data);		
	
	OPERATION_HEADER_t* operation_header = (OPERATION_HEADER_t*)ind->data;
	
	/* TESTING REGION */
	//For testing purposes just send throw UART port
	HAL_UartPrint("OPERATION RECEIVED >> FROM:");
	HAL_UartWriteNumberHEX(operation_header->sourceAddress);
	HAL_UartPrint("\t TO:");
	HAL_UartWriteNumberHEX(operation_header->destinationAddress);
	HAL_UartPrint("\t CODE:");
	HAL_UartWriteNumberHEX(operation_header->opCode);
	HAL_UartPrint("\t ARGS:");
	
	uint8_t length = getCommandArgsLength(&operation_header->opCode);
	
	for (uint8_t i = 0; i < length; i++)
	{
		HAL_UartWriteNumberHEX(*((uint8_t*)operation_header + sizeof(OPERATION_HEADER_t) + i));
		HAL_UartWriteByte(' ');
	}
	
	HAL_UartPrint("\r\n");
	if(operation_header->opCode == EXTENSION_OPCODE)
	return;
	/* END OF TESTING REGION */
	
	//TODO: Check with internal address instead of configuration address...
	if(operation_header->destinationAddress == runningConfiguration.topConfiguration.networkConfig.deviceAddress) //MINE (EXTERNAL)
	{
		handleCommand(operation_header);
	}else
	{
		//TODO: Send or log ERROR (OPERATION_DEST_ADDR_ERROR)
	}
}