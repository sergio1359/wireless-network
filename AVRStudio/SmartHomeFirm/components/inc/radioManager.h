/*
 * radioManager.h
 *
 * Created: 28/01/2013 14:27:58
 *  Author: Victor
 */ 


#ifndef RADIO_H_
#define RADIO_H_

#include <stdbool.h>

#include "nwk.h"
#include "configManager.h"

#include "DS2401.h"

typedef enum RadioState_t
{
	RF_STATE_INITIAL,
	RF_STATE_READY_TO_SEND,
	RF_STATE_WAIT_CONF,
	RF_STATE_PREPARE_TO_SLEEP,
	RF_STATE_SLEEP,
	RF_STATE_WAKEUP,
} RadioState_t;

typedef enum NetworkJoinState_t
{
	JOIN_STATE_INITIAL,
	JOIN_STATE_AES_GENERATION,
	JOIN_STATE_ADDRESS_GENERATION,
	JOIN_STATE_ABORTED,
	JOIN_STATE_JOINED,
	JOIN_STATE_SEND_REQUEST,
	JOIN_STATE_WAIT_REQUEST_CONF,
	JOIN_STATE_WAIT_REQUEST_RESP,
	JOIN_STATE_SEND_ABORT,
	JOIN_STATE_WAIT_ABORT_CONF,
	JOIN_STATE_SEND_ACCEPT,
	JOIN_STATE_WAIT_ACCEPT_CONF,
	JOIN_STATE_WAIT_ACCEPT_RESP,
} NetworkJoinState_t;

void  RADIO_Init(void);
void  RADIO_SendWakeup(void* callback);
void  RADIO_SendDiscovery(void* callback);
void  RADIO_StartNetworkJoin(void);

_Bool RADIO_AddMessageByCopy(OPERATION_HEADER_t* message, void* callback);
_Bool RADIO_AddMessageWithBodyByCopy(OPERATION_HEADER_t* message, uint8_t* body, uint8_t bodySize, void* callback);
_Bool RADIO_AddMessageByReference(OPERATION_HEADER_t* message, void* callback);

#endif /* RADIO_H_ */