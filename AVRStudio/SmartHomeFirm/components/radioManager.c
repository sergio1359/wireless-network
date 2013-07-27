/*
 * radioManager.c
 *
 * Created: 28/01/2013 14:28:08
 *  Author: Victor
 */ 

#include "globals.h"
#include "sysTimer.h"

#include "radioManager.h"
#include "modulesManager.h"
#include "operationsManager.h"

#include "leds.h"

#define COPIES_BUFFER_SIZE 256
#define PENDING_BUFFER_SIZE 64

#define COPIES_BUFFER_SIZE_MASK COPIES_BUFFER_SIZE-1
#define PENDING_BUFFER_SIZE_MASK PENDING_BUFFER_SIZE-1

#define DEFAULT_RETRIES_LIMIT 6
#define TIME_BW_RETRIES 500 //milliseconds

typedef struct
{
	OPERATION_HEADER_t* operationPtr;
	void         (*confirm)(struct OPERATION_DataConf_t *req);
}RADIO_ELEM_t;	

/* Circular buffer object */
CREATE_CIRCULARBUFFER(uint8_t, COPIES_BUFFER_SIZE)				copiesMessages_Buffer;
CREATE_CIRCULARBUFFER(RADIO_ELEM_t, PENDING_BUFFER_SIZE)		pendingMessages_Buffer;

static RadioState_t currentState = RF_STATE_INITIAL;
static NWK_DataReq_t nwkDataReq;
static uint8_t failRetries;
static SYS_Timer_t retriesTimer;

static INPUT_UART_HEADER_t uartRadioHeader;

static OPERATION_DataConf_t radioDataConf;

static struct
{
	OPERATION_HEADER_t header;
}discoveryFirmwareRead;

inline _Bool addMessageByCopy(OPERATION_HEADER_t* message, uint8_t size, uint8_t* body, uint8_t bodySize, void* callback);
inline unsigned int freeSpace(unsigned int start, unsigned int end, unsigned int size);
inline _Bool isInCopiesBuffer(uint8_t* message);
void resetRadioState(NWK_DataReq_t *req);
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

void Radio_SendDiscovery(void* callback)
{
	discoveryFirmwareRead.header.opCode				= FirmwareVersionRead;
	discoveryFirmwareRead.header.sourceAddress		= runningConfiguration.topConfiguration.networkConfig.deviceAddress;
	discoveryFirmwareRead.header.destinationAddress = BROADCAST_ADDRESS;
	
	Radio_AddMessageByReference(&discoveryFirmwareRead.header, callback);
}

_Bool Radio_AddMessageByReference(OPERATION_HEADER_t* message, void* callback)
{
	if(freeSpace(pendingMessages_Buffer.start, pendingMessages_Buffer.end, PENDING_BUFFER_SIZE) >= 1)
	{
		pendingMessages_Buffer.buffer[pendingMessages_Buffer.end].operationPtr = message;
		pendingMessages_Buffer.buffer[pendingMessages_Buffer.end].confirm = callback;
		pendingMessages_Buffer.end++;
		pendingMessages_Buffer.end &= PENDING_BUFFER_SIZE_MASK;
		
		sendNextMessage();
		
		return true;
	}else
	{
		//TODO: Send or Log ERROR (REFERENCES_BUFFER_FULL)
		return false;
	}
}

_Bool Radio_AddMessageByCopy(OPERATION_HEADER_t* message, void* callback)
{
	return addMessageByCopy(message, sizeof(OPERATION_HEADER_t) + MODULES_GetCommandArgsLength(&message->opCode), 0, 0, callback);
}


_Bool Radio_AddMessageWithBodyByCopy(OPERATION_HEADER_t* message, uint8_t* body, uint8_t bodySize, void* callback)
{
	return addMessageByCopy(message, sizeof(OPERATION_HEADER_t) + MODULES_GetCommandArgsLength(&message->opCode) - bodySize, body, bodySize, callback);
}



/************************************************************************/
/*                       INTERNAL METHODS                               */
/************************************************************************/


_Bool addMessageByCopy(OPERATION_HEADER_t* message, uint8_t size, uint8_t* body, uint8_t bodySize, void* callback)
{
	if(freeSpace(copiesMessages_Buffer.start, copiesMessages_Buffer.end, COPIES_BUFFER_SIZE) >= (size + bodySize))
	{	
		OPERATION_HEADER_t* initialPtr = (OPERATION_HEADER_t*)&copiesMessages_Buffer.buffer[copiesMessages_Buffer.end];
		
		for(int i = 0; i < size; i++)
		{
			copiesMessages_Buffer.buffer[copiesMessages_Buffer.end] = *((uint8_t*)message + i);
			copiesMessages_Buffer.end++;
			copiesMessages_Buffer.end &= COPIES_BUFFER_SIZE_MASK;
		}
		
		for(int i = 0; i < bodySize; i++)
		{
			copiesMessages_Buffer.buffer[copiesMessages_Buffer.end] = body[i];
			copiesMessages_Buffer.end++;
			copiesMessages_Buffer.end &= COPIES_BUFFER_SIZE_MASK;
		}

		return Radio_AddMessageByReference(initialPtr, callback);
	}else
	{
		//TODO: Send or Log ERROR (COPIES_BUFFER_FULL)
		return false;
	}
}


unsigned int freeSpace(unsigned int start, unsigned int end, unsigned int size)
{
	if(start <= end)
		return size - (end - start);
	else
		return start - end;
}

_Bool isInCopiesBuffer(uint8_t* message)
{
	return ((uint16_t)message >= (uint16_t)&copiesMessages_Buffer.buffer && (uint16_t)message <= ((uint16_t)(&copiesMessages_Buffer.buffer) + COPIES_BUFFER_SIZE));
}

void resetRadioState(NWK_DataReq_t *req)
{
	if( pendingMessages_Buffer.buffer[pendingMessages_Buffer.start].confirm != 0)
	{
		radioDataConf.header = pendingMessages_Buffer.buffer[pendingMessages_Buffer.start].operationPtr;
		radioDataConf.retries = failRetries;
		radioDataConf.sendOk = (NWK_SUCCESS_STATUS == req->status);
		(*pendingMessages_Buffer.buffer[pendingMessages_Buffer.start].confirm)(&radioDataConf);
	}		
	
	pendingMessages_Buffer.start++;
	pendingMessages_Buffer.start &= PENDING_BUFFER_SIZE_MASK;
	
	if(isInCopiesBuffer(nwkDataReq.data))
	{
		copiesMessages_Buffer.start += nwkDataReq.size;
		copiesMessages_Buffer.start &= COPIES_BUFFER_SIZE_MASK;
	}
	
	failRetries = 0;
	currentState = RF_STATE_READY_TO_SEND;
	sendNextMessage();
}

void sendNextMessage()
{
	//TODO: Avoid this situation
	if(currentState != RF_STATE_READY_TO_SEND)
		return;
	
	OPERATION_HEADER_t* currentOP;
	uint8_t length;
	
	if(pendingMessages_Buffer.start != pendingMessages_Buffer.end)//Something to send
	{
		currentOP = pendingMessages_Buffer.buffer[pendingMessages_Buffer.start].operationPtr;
		length = sizeof(OPERATION_HEADER_t) + MODULES_GetCommandArgsLength(&currentOP->opCode);
			
		nwkDataReq.dstAddr = currentOP->destinationAddress;
		nwkDataReq.dstEndpoint = APP_ENDPOINT;
		nwkDataReq.srcEndpoint = APP_ENDPOINT;
		nwkDataReq.data = (uint8_t *)currentOP;
		nwkDataReq.size = length;
		nwkDataReq.confirm = rfDataConf;
		
		if(nwkDataReq.dstAddr == BROADCAST_ADDRESS)
			nwkDataReq.options = NWK_OPT_LINK_LOCAL  | NWK_OPT_ENABLE_SECURITY;
		else
			nwkDataReq.options = NWK_OPT_ACK_REQUEST | NWK_OPT_ENABLE_SECURITY;

		currentState = RF_STATE_WAIT_CONF;
		
		NWK_DataReq(&nwkDataReq);
	}	
}


static void rfDataConf(NWK_DataReq_t *req)
{
	if (NWK_SUCCESS_STATUS == req->status)
	{
		resetRadioState(req);
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
			//Discard message
			resetRadioState(req);
			
			//TODO: Send or log ERROR (LIMIT_EXCEDED)
		}else
		{
			//Retry
			SYS_TimerStart(&retriesTimer);
		}
		
	}//TODO:  Send or log ERROR (UNEXPECTED_NETWORK_STATUS)	
}

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
	
	uartRadioHeader.endPoint = ind->dstEndpoint;
	uartRadioHeader.nextHop = NWK_RouteNextHop(ind->srcAddr,0);
	uartRadioHeader.routing = (ind->options & NWK_IND_OPT_LOCAL) == 0;
	uartRadioHeader.rssi = ind->rssi;
	uartRadioHeader.security = (ind->options & NWK_IND_OPT_SECURED) != 0;
	
	OM_ProccessExternalOperation(&uartRadioHeader, (OPERATION_HEADER_t*)ind->data);		
}


static void retriesTimerHandler(SYS_Timer_t *timer)
{
	NWK_DataReq(&nwkDataReq);

	(void)timer;
}