/*
 * uartManager.c
 *
 * Created: 22/05/2013 21:31:07
 *  Author: Victor
 */ 

/*****************************************************************************
*****************************************************************************/
#include "uartManager.h"

/*****************************************************************************
                              Definitions section
******************************************************************************/
// Magic symbol to start SOF end EOF sequences with. Should be duplicated if
// occured inside the message.
#define APP_MAGIC_SYMBOL        0x10
#define APP_SOF_SEQUENCE        {APP_MAGIC_SYMBOL, 0x02}
#define APP_EOF_SEQUENCE        {APP_MAGIC_SYMBOL, 0x03}
#define APP_DUPLICATED_SYMBOL   {APP_MAGIC_SYMBOL, APP_MAGIC_SYMBOL}

#define RX_BUFFER_SIZE 256

const uint8_t sof[] = APP_SOF_SEQUENCE;
const uint8_t eof[] = APP_EOF_SEQUENCE;

UARTReceiverState_t uartState = USART_RECEIVER_IDLE_RX_STATE;

uint8_t* rxBuffer[RX_BUFFER_SIZE];
uint8_t index;
uint8_t checkSum;

inline void sendMagicPackage(INPUT_UART_HEADER_t* input_header, uint8_t* data, uint8_t size, uint8_t* body, uint8_t bodySize);

inline uint8_t sendMagicSegment(uint8_t* data, uint8_t size);


void HAL_UartBytesReceived(uint16_t bytes)
{
	uint8_t byte;

	while (bytes)
	{
		_Bool acceptByte = false;
		
		byte = HAL_UartReadByte();

		bytes--;

		switch (uartState)
		{
			case USART_RECEIVER_IDLE_RX_STATE:
			if (APP_MAGIC_SYMBOL == byte)
			{
				uartState = USART_RECEIVER_SOF_RX_STATE;
				index = 0;
				checkSum = 0;
			}
			break;

			case USART_RECEIVER_SOF_RX_STATE:
			if (sof[1] == byte)
			{
				uartState = USART_RECEIVER_DATA_RX_STATE;
			}
			else
			{
				uartState = USART_RECEIVER_IDLE_RX_STATE;	
			}
			break;

			case USART_RECEIVER_DATA_RX_STATE:
			if (APP_MAGIC_SYMBOL == byte)
			{
				uartState = USART_RECEIVER_MAGIC_RX_STATE;	
			}
			else
			{
				acceptByte = true;	
			}
			break;

			case USART_RECEIVER_MAGIC_RX_STATE:
			if (APP_MAGIC_SYMBOL == byte)
			{
				uartState = USART_RECEIVER_DATA_RX_STATE;
				acceptByte = true;
			}
			else if (eof[1] == byte)
			{
				uartState = USART_RECEIVER_EOF_RX_STATE;
			}				
			else
			{
				uartState = USART_RECEIVER_ERROR_RX_STATE; 
				//TODO: Send or log ERROR (UART_RECEIVER_ERROR)
			}				
			break;
			
			case USART_RECEIVER_EOF_RX_STATE:
			if (checkSum == byte)
			{
				uartState = USART_RECEIVER_IDLE_RX_STATE;
				//TODO: PROCCESS UART HEADER!!
				OM_ProccessUARTOperation((OUTPUT_UART_HEADER_t*)rxBuffer, (OPERATION_HEADER_t*)((uint8_t*)rxBuffer + sizeof(OUTPUT_UART_HEADER_t)));
			}
			else
			{
				uartState = USART_RECEIVER_ERROR_RX_STATE;
				//TODO: Send or log ERROR (UART_CHECKSUM_ERROR)
			}
			break;
			
			default:
			break;
		}

		checkSum += byte;

		if (acceptByte)
		{
			if(index < RX_BUFFER_SIZE)
			{
				*((uint8_t*)rxBuffer+index) = byte;
				index++;
			}				
			else
			{
				//TODO: Send or log ERROR (UART_RXBUFFER_FULL)
			}				
		}			
	}
}

void USART_SendOperation(INPUT_UART_HEADER_t* input_header, OPERATION_HEADER_t* operation_header)
{
	sendMagicPackage(input_header, (uint8_t*) operation_header, sizeof(OPERATION_HEADER_t) + MODULES_GetCommandArgsLength(&operation_header->opCode), 0, 0);
}

void USART_SendOperationWithBody(INPUT_UART_HEADER_t* input_header, OPERATION_HEADER_t* operation_header, uint8_t* bodyPtr, uint8_t bodySize)
{
	sendMagicPackage(input_header, (uint8_t*) operation_header, sizeof(OPERATION_HEADER_t) + MODULES_GetCommandArgsLength(&operation_header->opCode) - bodySize, bodyPtr, bodySize);
}

uint8_t sendMagicSegment(uint8_t* data, uint8_t size)
{
	uint8_t cs = 0;
	
	for (uint8_t i = 0; i < size; i++)
	{
		if (data[i] == APP_MAGIC_SYMBOL)
		{
			HAL_UartWriteByte(APP_MAGIC_SYMBOL);
			cs += APP_MAGIC_SYMBOL;
		}
		HAL_UartWriteByte(data[i]);
		cs += data[i];
	}
	
	return cs;
}

void sendMagicPackage(INPUT_UART_HEADER_t* input_header, uint8_t* operation, uint8_t operationSize, uint8_t* body, uint8_t bodySize)
{
	uint8_t cs = 0;

	HAL_UartWriteByte(sof[0]);
	HAL_UartWriteByte(sof[1]);
	
	cs += sendMagicSegment((uint8_t*) input_header, sizeof(INPUT_UART_HEADER_t));
	
	cs += sendMagicSegment(operation, operationSize);
	
	cs += sendMagicSegment(body, bodySize);

	HAL_UartWriteByte(eof[0]);
	HAL_UartWriteByte(eof[1]);
	cs += sof[0] + sof[1] + eof[0] + eof[1];

	HAL_UartWriteByte(cs);
}