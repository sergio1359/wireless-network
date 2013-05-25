/*
 * uartManager.h
 *
 * Created: 23/05/2013 11:29:09
 *  Author: Victor
 */ 


#ifndef UARTMANAGER_H_
#define UARTMANAGER_H_

#include <stdint.h>
#include <stdbool.h>
#include "halUart.h"
	
/*****************************************************************************
                              Types section
******************************************************************************/
typedef enum _USARTReceiverState_t
{
  USART_RECEIVER_IDLE_RX_STATE,
  USART_RECEIVER_MAGIC_RX_STATE,
  USART_RECEIVER_SOF_RX_STATE,
  USART_RECEIVER_DATA_RX_STATE,
  USART_RECEIVER_EOF_RX_STATE,
  USART_RECEIVER_ERROR_RX_STATE
} UARTReceiverState_t;

#endif /* UARTMANAGER_H_ */