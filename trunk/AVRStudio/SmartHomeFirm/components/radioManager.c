/*
 * radioManager.c
 *
 * Created: 28/01/2013 14:28:08
 *  Author: Victor
 */ 
#include <util/delay.h>

#include "globals.h"
#include "sysTimer.h"

#include "radioManager.h"
#include "modulesManager.h"
#include "operationsManager.h"

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
static _Bool usingSecurity = false;
static NWK_DataReq_t nwkDataReq;
static uint8_t failRetries;
static SYS_Timer_t retriesTimer;

static INPUT_UART_HEADER_t uartRadioHeader;

static OPERATION_DataConf_t radioDataConf;

static NetworkJoinState_t joinState = JOIN_STATE_INITIAL;
static uint8_t responsesCount;

static struct
{
	OPERATION_HEADER_t header;
	union
	{
		JOIN_ABORT_MESSAGE_t joinAbortBody;
		JOIN_ACCEPT_MESSAGE_t joinAcceptBody;
	};
}radioMessage;

inline _Bool addMessageByCopy(OPERATION_HEADER_t* message, uint8_t size, uint8_t* body, uint8_t bodySize, void* callback);
inline unsigned int freeSpace(unsigned int start, unsigned int end, unsigned int size);
inline _Bool isInCopiesBuffer(uint8_t* message);
static void resetRadioState(NWK_DataReq_t *req);
static void sendNextMessage(void);
inline static void sendDataRequest(uint16_t destinationAddress, uint8_t* data, uint8_t size);
inline static void handleJoinConf(OPERATION_DataConf_t *req);
static void rfDataConf(NWK_DataReq_t *req);
static void rfDataInd(NWK_DataInd_t *ind);
static void retriesTimerHandler(SYS_Timer_t *timer);

void RADIO_Init()
{
	failRetries = 0;
	
	retriesTimer.interval = TIME_BW_RETRIES; 
	retriesTimer.mode = SYS_TIMER_INTERVAL_MODE;
	retriesTimer.handler = retriesTimerHandler;
	
	usingSecurity = true;
	
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

void RADIO_SendDiscovery(void* callback)
{
	radioMessage.header.opCode				= FirmwareVersionRead;
	radioMessage.header.sourceAddress		= runningConfiguration.topConfiguration.networkConfig.deviceAddress;
	radioMessage.header.destinationAddress  = BROADCAST_ADDRESS;
	
	RADIO_AddMessageByReference(&radioMessage.header, callback);
}

void RADIO_StartNetworkJoin()
{
	uint8_t joinCounter = 0;
	while(1)
	{
		switch(joinState)
		{
			case JOIN_STATE_INITIAL:
				//Use default radio settings forcing validConfiguration to false value
				validConfiguration = false;
				RADIO_Init();
				
				responsesCount = 0;
				//TODO: Generate Random AES Key
				
				joinState = JOIN_STATE_SEND_REQUEST;
				break;
				
			case JOIN_STATE_SEND_REQUEST:
				//Send Join Request
				radioMessage.header.opCode				= JoinRequest;
				radioMessage.header.sourceAddress		= APP_ADDR;
				radioMessage.header.destinationAddress  = COORDINATOR_ADDRESS;
				usingSecurity = false;
				
				RADIO_AddMessageByReference(&radioMessage.header, handleJoinConf);
				
				joinState = JOIN_STATE_WAIT_REQUEST_CONF;
				break;
				
			case JOIN_STATE_WAIT_REQUEST_RESP:
				//TODO: Wait until joinCounter reaches a defined value. Then check the number of responses. If "numberResponses == 1" SEND ACCEPT. Otherwise SEND ABORT.
				/*if(joinCounter++ > 5 * (20)) // 1s / 50ms = 20
				{
					if(responsesCount == 0)
					{
						
					}else if(responsesCount == 1)
					{
						joinState = JOIN_STATE_SEND_ACCEPT;
					}else
					{
						joinState = JOIN_STATE_SEND_ABORT;
					}																		
				}*/
				break;
				
			case JOIN_STATE_SEND_ABORT:
				//Send Join Abort
				radioMessage.header.opCode				= JoinAbort;
				radioMessage.header.sourceAddress		= APP_ADDR;
				radioMessage.header.destinationAddress  = COORDINATOR_ADDRESS;
				radioMessage.joinAbortBody.NumberOfResponses = responsesCount;
				usingSecurity = false;
			
				RADIO_AddMessageByReference(&radioMessage.header, handleJoinConf);
			
				joinState = JOIN_STATE_WAIT_ABORT_CONF;
				break;
			
			case JOIN_STATE_SEND_ACCEPT:
				//Send Join Accept
				radioMessage.header.opCode				= JoinAccept;
				radioMessage.header.sourceAddress		= APP_ADDR;
				radioMessage.header.destinationAddress  = COORDINATOR_ADDRESS;
				//memcpy((uint8_t*)&radioMessage.joinAcceptBody.AES_Key,(uint8_t*)&serialNumber, 16); 	//TODO: Fill this field correctly
				memcpy((uint8_t*)&radioMessage.joinAcceptBody.MacAddress,(uint8_t*)&serialNumber, SERIAL_NUMBER_SIZE);
				usingSecurity = false;
				
				RADIO_AddMessageByReference(&radioMessage.header, handleJoinConf);
				
				joinState = JOIN_STATE_WAIT_ACCEPT_CONF;
				break;
				
			case JOIN_STATE_WAIT_ACCEPT_RESP:
				break;
		}
		
		if(joinState == JOIN_STATE_ABORTED) //|| joinState == JOIN_STATE_JOINED
			break;
		
		BASE_LedToggle();
		_delay_ms(50);
		
		//Call Handlers
		SYS_TaskHandler();
		HAL_UartTaskHandler();	
	}
}

_Bool RADIO_AddMessageByReference(OPERATION_HEADER_t* message, void* callback)
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

_Bool RADIO_AddMessageByCopy(OPERATION_HEADER_t* message, void* callback)
{
	return addMessageByCopy(message, sizeof(OPERATION_HEADER_t) + MODULES_GetCommandArgsLength(&message->opCode), 0, 0, callback);
}


_Bool RADIO_AddMessageWithBodyByCopy(OPERATION_HEADER_t* message, uint8_t* body, uint8_t bodySize, void* callback)
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

		return RADIO_AddMessageByReference(initialPtr, callback);
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
	uint8_t size;
	
	if(pendingMessages_Buffer.start != pendingMessages_Buffer.end)//Something to send
	{
		currentOP = pendingMessages_Buffer.buffer[pendingMessages_Buffer.start].operationPtr;
		size = sizeof(OPERATION_HEADER_t) + MODULES_GetCommandArgsLength(&currentOP->opCode);
		
		sendDataRequest(currentOP->destinationAddress, (uint8_t *)currentOP, size);
	}	
}

static void sendDataRequest(uint16_t destinationAddress, uint8_t* data, uint8_t size)
{
	nwkDataReq.dstAddr = destinationAddress;
	nwkDataReq.dstEndpoint = APP_ENDPOINT;
	nwkDataReq.srcEndpoint = APP_ENDPOINT;
	nwkDataReq.data = data;
	nwkDataReq.size = size;
	nwkDataReq.confirm = rfDataConf;
	
	if(destinationAddress == BROADCAST_ADDRESS)
		nwkDataReq.options = NWK_OPT_LINK_LOCAL;
	else
		nwkDataReq.options = NWK_OPT_ACK_REQUEST;
	
	if(usingSecurity)
		nwkDataReq.options |= NWK_OPT_ENABLE_SECURITY;

	currentState = RF_STATE_WAIT_CONF;
	
	NWK_DataReq(&nwkDataReq);
}

static void handleJoinConf(OPERATION_DataConf_t *req)
{
	switch(joinState)
	{
		case JOIN_STATE_WAIT_REQUEST_CONF:
			if (req->sendOk)
				joinState = JOIN_STATE_WAIT_REQUEST_RESP;
			else
				joinState = JOIN_STATE_SEND_REQUEST;
			break;
		
		case JOIN_STATE_WAIT_ABORT_CONF:
			if (req->sendOk)
				joinState = JOIN_STATE_ABORTED;
			else
				joinState = JOIN_STATE_SEND_ABORT;
			break;
		
		case JOIN_STATE_WAIT_ACCEPT_CONF:
			if (req->sendOk)
				joinState = JOIN_STATE_WAIT_ACCEPT_RESP;
			else
				joinState = JOIN_STATE_SEND_ACCEPT;
			break;
		
		default:
			//TODO: Send or log ERROR (UNEXPECTED_NETWORK_JOIN_STATUS)
			break;
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
	
	if(joinState == JOIN_STATE_INITIAL || joinState == JOIN_STATE_JOINED)
	{
		uartRadioHeader.endPoint = ind->dstEndpoint;
		uartRadioHeader.nextHop = NWK_RouteNextHop(ind->srcAddr,0);
		uartRadioHeader.routing = (ind->options & NWK_IND_OPT_LOCAL) == 0;
		uartRadioHeader.rssi = ind->rssi;
		uartRadioHeader.security = (ind->options & NWK_IND_OPT_SECURED) != 0;
		
		OM_ProccessExternalOperation(&uartRadioHeader, (OPERATION_HEADER_t*)ind->data);	
		
	}else if(joinState == JOIN_STATE_WAIT_REQUEST_RESP)
	{
		//TODO: Count Responses
		//responsesCount++;
	}else if(joinState == JOIN_STATE_WAIT_ACCEPT_RESP)
	{		
		
	}else
	{
		//TODO: Send or log ERROR (UNEXPECTED_NETWORK_JOIN_STATUS)
	}		
}


static void retriesTimerHandler(SYS_Timer_t *timer)
{
	NWK_DataReq(&nwkDataReq);

	(void)timer;
}