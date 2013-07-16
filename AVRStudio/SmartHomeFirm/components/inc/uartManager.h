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

#include "configManager.h"
	
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

typedef struct
{
	unsigned int endPoint : 6;
	unsigned int routing  : 1;
	unsigned int security : 1;
	uint16_t nextHop;
	uint8_t rssi;
}INPUT_UART_HEADER_t;

typedef struct
{
	unsigned int endPoint : 6;
	unsigned int routing  : 1;
	unsigned int security : 1;
	uint8_t retries;
}OUTPUT_UART_HEADER_t;

void USART_SendOperation(INPUT_UART_HEADER_t* input_header, OPERATION_HEADER_t* operation_header);

void USART_SendOperationWithBody(INPUT_UART_HEADER_t* input_header, OPERATION_HEADER_t* operation_header, uint8_t* bodyPtr, uint8_t bodySize);

#endif /* UARTMANAGER_H_ */