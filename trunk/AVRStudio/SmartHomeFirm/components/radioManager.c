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

#define TIMER_JOIN_INTERVAL 50 //milliseconds
#define RESET_BUTTON_TIME 2500 //milliseconds

typedef struct
{
	OPERATION_HEADER_t* operationPtr;
	void         (*confirm)(struct OPERATION_DataConf_t *req);
}RADIO_ELEM_t;	

/* Circular buffer object */
CREATE_CIRCULARBUFFER(uint8_t, COPIES_BUFFER_SIZE)				copiesMessages_Buffer;
CREATE_CIRCULARBUFFER(RADIO_ELEM_t, PENDING_BUFFER_SIZE)		pendingMessages_Buffer;

//Radio Send vars
static RadioState_t currentState = RF_STATE_INITIAL;
static _Bool usingSecurity = false;
static NWK_DataReq_t nwkDataReq;
static uint8_t failRetries;
static uint16_t initialAddress = APP_ADDR;
static SYS_Timer_t retriesTimer;

static INPUT_UART_HEADER_t uartRadioHeader;

static OPERATION_DataConf_t radioDataConf;

//Join vars
static NetworkJoinState_t joinState = JOIN_STATE_INITIAL;
static uint8_t joinCounter;
static uint8_t resetButtonCounter;
static _Bool buttonReleased;
static uint8_t responsesCount;
static uint8_t joinAESKey[16];
static uint8_t AESIndex;

//WakeUp, Discovery and Join shared message struct
static struct
{
	OPERATION_HEADER_t header;
	union
	{
		JOIN_ABORT_MESSAGE_t joinAbortBody;
		JOIN_ACCEPT_MESSAGE_t joinAcceptBody;
	};
}radioMessage;

//Radio Send functions
inline _Bool addMessageByCopy(OPERATION_HEADER_t* message, uint8_t size, uint8_t* body, uint8_t bodySize, void* callback);
inline unsigned int freeSpace(unsigned int start, unsigned int end, unsigned int size);
inline _Bool isInCopiesBuffer(uint8_t* message);
static void resetRadioState(NWK_DataReq_t *req);
static void sendNextMessage(void);
inline static void sendDataRequest(uint16_t destinationAddress, uint8_t* data, uint8_t size);
static void rfDataConf(NWK_DataReq_t *req);
static void rfDataInd(NWK_DataInd_t *ind);
static void retriesTimerHandler(SYS_Timer_t *timer);

//Join functions
inline static void joinStateMachine(void);
static void handleJoinConf(OPERATION_DataConf_t *req);
static void joinRandomRequest(uint16_t rnd);
static void joinTimerHandler(SYS_Timer_t *timer);



void RADIO_Init(_Bool forceDefaultParams)
{
	//Keep Join network parameters
	if(joinState == JOIN_STATE_JOINED)
		return;
	
	failRetries = 0;
	
	retriesTimer.interval = TIME_BW_RETRIES; 
	retriesTimer.mode = SYS_TIMER_INTERVAL_MODE;
	retriesTimer.handler = retriesTimerHandler;
	
	usingSecurity = true;
	
	if(!forceDefaultParams && (validConfiguration || IS_TEMPORAL_CONFIG))
	{
		NWK_SetAddr(runningConfiguration.topConfiguration.networkConfig.deviceAddress);
		NWK_SetPanId(runningConfiguration.topConfiguration.networkConfig.panId);
		PHY_SetChannel(runningConfiguration.topConfiguration.networkConfig.channel);
		
		#ifdef NWK_ENABLE_SECURITY
		NWK_SetSecurityKey(runningConfiguration.topConfiguration.networkConfig.securityKey);
		#endif
	}else
	{
		NWK_SetAddr(initialAddress);
		NWK_SetPanId(APP_PANID);
		PHY_SetChannel(APP_CHANNEL);
		
		runningConfiguration.topConfiguration.networkConfig.networkRetries = DEFAULT_RETRIES_LIMIT;
		
		#ifdef NWK_ENABLE_SECURITY
		NWK_SetSecurityKey((uint8_t *)APP_SECURITY_KEY);
		#endif
	}
	
	PHY_SetTxPower(0x00);//Maximum
	
	PHY_SetRxState(true);
	
	NWK_OpenEndpoint(APP_ENDPOINT, rfDataInd);
	
	currentState = RF_STATE_READY_TO_SEND;
}

void RADIO_SendWakeup(void* callback)
{
	radioMessage.header.opCode				= WakeUp;
	radioMessage.header.sourceAddress		= runningConfiguration.topConfiguration.networkConfig.deviceAddress;
	radioMessage.header.destinationAddress  = BROADCAST_ADDRESS;
	
	RADIO_AddMessageByReference(&radioMessage.header, callback);
}

void RADIO_SendDiscovery(void* callback)
{
	radioMessage.header.opCode				= PingRequest;
	radioMessage.header.sourceAddress		= runningConfiguration.topConfiguration.networkConfig.deviceAddress;
	radioMessage.header.destinationAddress  = BROADCAST_ADDRESS;
	
	RADIO_AddMessageByReference(&radioMessage.header, callback);
}

void RADIO_StartNetworkJoin()
{
	SYS_Timer_t joinTimer;
	joinTimer.interval = TIMER_JOIN_INTERVAL;
	joinTimer.mode = SYS_TIMER_PERIODIC_MODE;
	joinTimer.handler = joinTimerHandler;
	SYS_TimerStart(&joinTimer);
	
	while(1)
	{
		joinStateMachine();
		
		if((joinState == JOIN_STATE_ABORTED || joinState == JOIN_STATE_JOINED) && joinCounter >= 1 * (1000 / TIMER_JOIN_INTERVAL))
			break;			
		
		//Call Handlers
		SYS_TaskHandler();
		HAL_UartTaskHandler();	
	}
	
	SYS_TimerStop(&joinTimer);
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
				case NWK_NO_ROUTE_STATUS:
				DISPLAY_WriteString("NWK_NO_ROUTE_STATUS"); break;
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
	OPERATION_HEADER_t* operation_header = (OPERATION_HEADER_t*)ind->data;
	
	if(joinState == JOIN_STATE_INITIAL || joinState == JOIN_STATE_ABORTED || joinState == JOIN_STATE_JOINED)
	{
		uartRadioHeader.endPoint = ind->dstEndpoint;
		uartRadioHeader.nextHop = NWK_RouteNextHop(ind->srcAddr,0);
		uartRadioHeader.routing = (ind->options & NWK_IND_OPT_LOCAL) == 0;
		uartRadioHeader.rssi = ind->rssi;
		uartRadioHeader.security = (ind->options & NWK_IND_OPT_SECURED) != 0;
		
		OM_ProccessExternalOperation(&uartRadioHeader, operation_header);	
		
	}else if(joinState == JOIN_STATE_WAIT_REQUEST_RESP)
	{
		if(operation_header->opCode == JoinRequestResponse &&
		   operation_header->sourceAddress == COORDINATOR_ADDRESS)
		   {
			   JOIN_REQUEST_RESPONSE_MESSAGE_t* msg = (JOIN_REQUEST_RESPONSE_MESSAGE_t*)(operation_header + 1);   
			   //TODO: Copy RSA Key
			   responsesCount++;
		   }else
		   {
			   //TODO: Send or log ERROR (UNEXPECTED_NETWORK_JOIN_RESPONSE)
		   }
	}else if(joinState == JOIN_STATE_WAIT_ACCEPT_RESP)
	{		
		if(operation_header->opCode == JoinAcceptResponse &&
		operation_header->sourceAddress == COORDINATOR_ADDRESS)
		{
			JOIN_ACCEPT_RESPONSE_MESSAGE_t* msg = (JOIN_ACCEPT_RESPONSE_MESSAGE_t*)(operation_header + 1);
			
			//Update network parameters
			runningConfiguration.topConfiguration.networkConfig.deviceAddress = msg->Address;
			runningConfiguration.topConfiguration.networkConfig.channel = msg->Channel;
			runningConfiguration.topConfiguration.networkConfig.panId = msg->PanId;
			memcpy((uint8_t *)runningConfiguration.topConfiguration.networkConfig.securityKey, msg->Network_AES_Key, 16);
			
			CONFIG_SaveTemporalConfig();
			
			NWK_SetAddr(msg->Address);
			//NWK_SetPanId(msg->PanId);
			//PHY_SetChannel(msg->Channel);
#ifdef NWK_ENABLE_SECURITY
			NWK_SetSecurityKey(msg->Network_AES_Key);
#endif
			
			joinState = JOIN_STATE_JOINED;
		}if(operation_header->opCode == JoinAbort &&
		operation_header->sourceAddress == COORDINATOR_ADDRESS)
		{
			//Coordinator reject access
			softReset();
		}		
		else
		{
			//TODO: Send or log ERROR (UNEXPECTED_NETWORK_JOIN_ACCEPT_RESPONSE)
		}
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



/************************************************************************/
/*                       JOIN INTERNAL METHODS                          */
/************************************************************************/

static void joinStateMachine(void)
{
	switch(joinState)
	{
		case JOIN_STATE_INITIAL:
			if(initialAddress == APP_ADDR)
			{
				RAND_Next(joinRandomRequest);
				joinState = JOIN_STATE_ADDRESS_GENERATION;
				break;
			}
		
			//Use default radio settings
			RADIO_Init(true);
		
			responsesCount = 0;
			joinCounter = 0;
		
			//Generate Random AES Key
			AESIndex = 0;
			RAND_Next(joinRandomRequest);
		
			joinState = JOIN_STATE_AES_GENERATION;
			break;
		
		case JOIN_STATE_SEND_REQUEST:
			//Send Join Request
			radioMessage.header.opCode				= JoinRequest;
			radioMessage.header.sourceAddress		= initialAddress;
			radioMessage.header.destinationAddress  = COORDINATOR_ADDRESS;
			usingSecurity = false;
		
			RADIO_AddMessageByReference(&radioMessage.header, handleJoinConf);
		
			joinState = JOIN_STATE_WAIT_REQUEST_CONF;
			break;
		
		case JOIN_STATE_WAIT_REQUEST_RESP:
			//Wait until joinCounter reaches a defined value (8 seconds). Then check the number of responses. If "numberResponses == 1" SEND ACCEPT. Otherwise SEND ABORT.
			if(joinCounter > 8 * (1000 / TIMER_JOIN_INTERVAL))
			{
				if(responsesCount == 1)
				{
					joinState = JOIN_STATE_SEND_ACCEPT;
				}else
				{
					joinState = JOIN_STATE_SEND_ABORT;
				}
			
				joinCounter = 0;
			}
			break;
		
		case JOIN_STATE_SEND_ABORT:
			//Send Join Abort
			radioMessage.header.opCode				= JoinAbort;
			radioMessage.header.sourceAddress		= initialAddress;
			radioMessage.header.destinationAddress  = COORDINATOR_ADDRESS;
			radioMessage.joinAbortBody.NumberOfResponses = responsesCount;
			usingSecurity = false;
		
			RADIO_AddMessageByReference(&radioMessage.header, handleJoinConf);
		
			joinState = JOIN_STATE_WAIT_ABORT_CONF;
			break;
		
		case JOIN_STATE_SEND_ACCEPT:
			//Send Join Accept
			radioMessage.header.opCode				= JoinAccept;
			radioMessage.header.sourceAddress		= initialAddress;
			radioMessage.header.destinationAddress  = COORDINATOR_ADDRESS;
			radioMessage.joinAcceptBody.BaseModel	= baseModel;
			radioMessage.joinAcceptBody.ShieldModel	= shieldModel;
			memcpy((uint8_t*)&radioMessage.joinAcceptBody.AES_Key,(uint8_t*)&joinAESKey, 16); 	//TODO: Fill this field correctly
			memcpy((uint8_t*)&radioMessage.joinAcceptBody.MacAddress,(uint8_t*)&serialNumber, SERIAL_NUMBER_SIZE);
			usingSecurity = false;
		
			RADIO_AddMessageByReference(&radioMessage.header, handleJoinConf);
		
			joinState = JOIN_STATE_WAIT_ACCEPT_CONF;
			break;
	}
}


static void handleJoinConf(OPERATION_DataConf_t *req)
{
	switch(joinState)
	{
		case JOIN_STATE_WAIT_REQUEST_CONF:
			if (req->sendOk)
				joinState = JOIN_STATE_WAIT_REQUEST_RESP;
			else
				joinState = JOIN_STATE_SEND_REQUEST;//TODO: Timeout?
			break;
		
		case JOIN_STATE_WAIT_ABORT_CONF:
			if (req->sendOk)
				joinState = JOIN_STATE_ABORTED;
			else
				joinState = JOIN_STATE_SEND_ABORT;//TODO: Timeout?
			break;
		
		case JOIN_STATE_WAIT_ACCEPT_CONF:
			if (req->sendOk)
				joinState = JOIN_STATE_WAIT_ACCEPT_RESP;
			else
				joinState = JOIN_STATE_SEND_ACCEPT;//TODO: Timeout?
			break;
		
		default:
			//TODO: Send or log ERROR (UNEXPECTED_NETWORK_JOIN_STATUS)
			break;
	}
}

static void joinTimerHandler(SYS_Timer_t *timer)
{
	BASE_LedToggle();
	
	if(!BASE_ButtonPressed())
		buttonReleased = true;
	
	if(joinState == JOIN_STATE_WAIT_REQUEST_RESP || joinState == JOIN_STATE_ABORTED || joinState == JOIN_STATE_JOINED)
	{
		if(joinCounter == 0)
		{
			DISPLAY_Clear();
			if(joinState == JOIN_STATE_ABORTED)
				DISPLAY_WriteString("JOIN_STATE_ABORTED");
			else if(joinState == JOIN_STATE_JOINED)
				DISPLAY_WriteString("JOIN_STATE_JOINED");
			else
				DISPLAY_WriteString("JOIN_STATE_WAIT_REQUEST_RESP");
		}
		
		joinCounter++;
	}else if(joinState == JOIN_STATE_WAIT_ACCEPT_RESP)
	{
		if(BASE_ButtonPressed())
		{
			if(buttonReleased && resetButtonCounter++ == (RESET_BUTTON_TIME / TIMER_JOIN_INTERVAL))
				softReset();
		}else
		{
			resetButtonCounter = 0;
		}
	}
}

static void joinRandomRequest(uint16_t rnd)
{
	if(joinState == JOIN_STATE_ADDRESS_GENERATION)
	{
		initialAddress = rnd;
		joinState = JOIN_STATE_INITIAL;
	}else
	{
		joinAESKey[AESIndex++] = (rnd >> 8) & 0xFF;
		joinAESKey[AESIndex++] = rnd & 0xFF;
		
		if(AESIndex == 16)
		{
			joinState = JOIN_STATE_SEND_REQUEST;
		}else
		{
			RAND_Next(joinRandomRequest);
		}
	}
}
