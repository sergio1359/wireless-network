/**************************************************************************//**
  \file blink.c

  \brief Blink application.

  \author
    Atmel Corporation: http://www.atmel.com \n
    Support email: avr@atmel.com

  Copyright (c) 2008-2012, Atmel Corporation. All rights reserved.
  Licensed under Atmel's Limited License Agreement (BitCloudTM).

  \internal
    History:
******************************************************************************/

#include <appTimer.h>
#include <zdo.h>
#include <taskManager.h>
#include <gpio.h>
#include <usart.h>

#define RX_BUFFER_SIZE 10

static HAL_AppTimer_t blinkTimer;                           // Blink timer.

static void blinkTimerFired(void);                          // blinkTimer handler.

HAL_UsartDescriptor_t usart;

static uint8_t Rx_Buffer[RX_BUFFER_SIZE];

void usartRcvd()
{
	uint8_t usartRecieveBuffer;
	uint8_t errBuffer = 'e';
	uint16_t length;
	HAL_ReadUsart(&usart,&usartRecieveBuffer,1);
	HAL_WriteUsart(&usart,&usartRecieveBuffer,1);
}

void Usart_Init()
{
	usart.tty             = USART_CHANNEL_0;
	usart.mode            = USART_MODE_ASYNC;
	usart.baudrate        = USART_BAUDRATE_19200;
	usart.dataLength      = USART_DATA8;
	usart.parity          = USART_PARITY_NONE;
	usart.stopbits        = USART_STOPBIT_1;
	usart.rxBuffer        = Rx_Buffer;
	usart.rxBufferLength  = RX_BUFFER_SIZE;
	usart.txBuffer        = NULL;
	usart.txBufferLength  = 0;
	usart.rxCallback      = usartRcvd;
	usart.txCallback      = NULL;
	usart.flowControl     = USART_FLOW_CONTROL_NONE;

	HAL_OpenUsart(&usart);
}

/*******************************************************************************
  Description: application task handler.

  Parameters: none.

  Returns: nothing.
*******************************************************************************/
void APL_TaskHandler(void)
{
  GPIO_B4_make_out();
  GPIO_B4_clr();
  
  Usart_Init();	
  HAL_WriteUsart(&usart,"Hello",strlen("Hello"));
	
  // Configure blink timer
  blinkTimer.interval = 1000;       // Timer interval
  blinkTimer.mode     = TIMER_REPEAT_MODE;        // Repeating mode (TIMER_REPEAT_MODE or TIMER_ONE_SHOT_MODE)
  blinkTimer.callback = blinkTimerFired;          // Callback function for timer fire event
  HAL_StartAppTimer(&blinkTimer);                 // Start blink timer
  
  ZDO_StartNetworkReq()
}

/*******************************************************************************
  Description: blinkying timer fire event handler.

  Parameters: none.

  Returns: nothing.
*******************************************************************************/
static void blinkTimerFired()
{
  //Toggle the led
  GPIO_B4_toggle();
}

/*******************************************************************************
  Description: just a stub.

  Parameters: are not used.

  Returns: nothing.
*******************************************************************************/
void ZDO_MgmtNwkUpdateNotf(ZDO_MgmtNwkUpdateNotf_t *nwkParams)
{
  nwkParams = nwkParams;  // Unused parameter warning prevention
}

/*******************************************************************************
  Description: just a stub.

  Parameters: none.

  Returns: nothing.
*******************************************************************************/
void ZDO_WakeUpInd(void)
{
}

#ifdef _BINDING_
/***********************************************************************************
  Stub for ZDO Binding Indication

  Parameters:
    bindInd - indication

  Return:
    none

 ***********************************************************************************/
void ZDO_BindIndication(ZDO_BindInd_t *bindInd)
{
  (void)bindInd;
}

/***********************************************************************************
  Stub for ZDO Unbinding Indication

  Parameters:
    unbindInd - indication

  Return:
    none

 ***********************************************************************************/
void ZDO_UnbindIndication(ZDO_UnbindInd_t *unbindInd)
{
  (void)unbindInd;
}
#endif //_BINDING_

void BSP_TaskHandler()
{
	
}

/**********************************************************************//**
  \brief Main - C program main start function

  \param none
  \return none
**************************************************************************/
int main(void)
{
  SYS_SysInit();
	
	
  for(;;)
  {
    SYS_RunTask();
  }
}

//eof blink.c
