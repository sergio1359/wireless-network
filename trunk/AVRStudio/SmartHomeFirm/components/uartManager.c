/*
 * uartManager.c
 *
 * Created: 22/05/2013 21:31:07
 *  Author: Victor
 */ 

/*****************************************************************************
*****************************************************************************/
#include "uartManager.h"
#include "EEPROM.h"

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
				//TODO: PROCCESS MESAGE!!
				OM_ProccessOperation((OPERATION_HEADER_t*)rxBuffer, true);
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
				rxBuffer[index++] = byte;
			}				
			else
			{
				//TODO: Send or log ERROR (UART_RXBUFFER_FULL)
			}				
		}			
	}
}

static void sendData(uint8_t *data, uint8_t size)
{
	uint8_t cs = 0;

	HAL_UartWriteByte(sof[0]);
	HAL_UartWriteByte(sof[1]);

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

	HAL_UartWriteByte(eof[0]);
	HAL_UartWriteByte(eof[1]);
	cs += sof[0] + sof[1] + eof[0] + eof[1];

	HAL_UartWriteByte(cs);
}