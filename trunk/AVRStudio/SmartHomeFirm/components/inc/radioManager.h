/*
 * radioManager.h
 *
 * Created: 28/01/2013 14:27:58
 *  Author: Victor
 */ 


#ifndef RADIO_H_
#define RADIO_H_

#include "configManager.h"
#include <stdbool.h>

typedef enum RadioState_t
{
	RF_STATE_INITIAL,
	RF_STATE_READY_TO_SEND,
	RF_STATE_WAIT_CONF,
	RF_STATE_PREPARE_TO_SLEEP,
	RF_STATE_SLEEP,
	RF_STATE_WAKEUP,
} RadioState_t;

_Bool Radio_AddMessageByCopy(OPERATION_HEADER_t* message, void* callback);
_Bool Radio_AddMessageWithBodyByCopy(OPERATION_HEADER_t* message, uint8_t* body, uint8_t bodySize, void* callback);
_Bool Radio_AddMessageByReference(OPERATION_HEADER_t* message, void* callback);

void Radio_Init(void);

#endif /* RADIO_H_ */