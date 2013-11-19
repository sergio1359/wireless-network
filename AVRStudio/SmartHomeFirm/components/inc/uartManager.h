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

typedef enum INPUTConfirmationTypes_t
{
	CONFIRMATION_NONE = 0,
	CONFIRMATION_OK,
	CONFIRMATION_ERROR,
} INPUTConfirmationTypes_t;

typedef struct
{
	uint8_t messageId; //Message internal ID. TODO: Use this ID to avoid incorrect confirmations confusion
	unsigned int sendOk : 1;	//LSB
	unsigned int retries : 7;	//MSB
	OPERATION_HEADER_t* header;
} OPERATION_DataConf_t;

typedef struct
{
	uint8_t messageId; //Message internal ID. TODO: Use this ID to avoid incorrect confirmations confusion
	unsigned int endPoint		: 4;
	unsigned int confirmation	: 2;
	unsigned int routing		: 1;
	unsigned int security		: 1;
	uint16_t nextHop;
	int8_t rssi;
}INPUT_UART_HEADER_t; //To the PC

typedef struct
{
	uint8_t messageId; //Message internal ID. TODO: Use this ID to avoid incorrect confirmations confusion
	unsigned int endPoint : 4;
	unsigned int reserved : 2;
	unsigned int routing  : 1;
	unsigned int security : 1;
	uint8_t retries;
}OUTPUT_UART_HEADER_t; //From the PC

OPERATION_DataConf_t uartDataConf;

void USART_Init(void);

void USART_DataConf(OPERATION_DataConf_t *req);

void USART_SendOperation(INPUT_UART_HEADER_t* input_header, OPERATION_HEADER_t* operation_header, void* callback);

void USART_SendOperationWithBody(INPUT_UART_HEADER_t* input_header, OPERATION_HEADER_t* operation_header, uint8_t* bodyPtr, uint8_t bodySize, void* callback);

#endif /* UARTMANAGER_H_ */