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

#include "configuration.h"
#include <appTimer.h>
#include <zdo.h>
#include <configServer.h>
#include <taskManager.h>
#include <gpio.h>
#include <usart.h>
#include <adc.h>
#include "application.h"

#define PRINT(x) HAL_WriteUsart(&usart,x,strlen(x))

#define RX_BUFFER_SIZE 10

AppState_t appState = APP_INITING_STATE;

static HAL_AppTimer_t blinkTimer;                           // Blink timer.

HAL_AdcDescriptor_t adcDescriptor;

HAL_UsartDescriptor_t usart;
static uint8_t Rx_Buffer[RX_BUFFER_SIZE];

static ZDO_StartNetworkReq_t zdoStartReq;
static APS_RegisterEndpointReq_t endpointParams;
static SimpleDescriptor_t simpleDescriptor = { 1U, 1U, 1, 1, 0, 0 , NULL, 0, NULL };

static void blinkTimerFired(void);                          // blinkTimer handler.

void usartRcvd()
{
	uint8_t usartRecieveBuffer;
	
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

uint8_t adcBuffer;

void Adc_Init()
{
	adcDescriptor.resolution = RESOLUTION_8_BIT;
	adcDescriptor.voltageReference=INTERNAL_2d56V;
	adcDescriptor.bufferPointer=&adcBuffer;
	adcDescriptor.selectionsAmount=1;	
	adcDescriptor.callback = NULL;
	
	HAL_OpenAdc(&adcDescriptor);
}

void APS_DataIndication(APS_DataInd_t* indData)
{
	//static uint8_t b=60;
	(void) indData;		/* suppress compiler warnings */
	// Data received indication
	GPIO_D6_set();
	/* usartSendByte(b++); */
	//usartSendByte(b++);
}

char buf[20];
static void appZdoStartNetworkConf(ZDO_StartNetworkConf_t *startInfo)
{
	/*
	switch (startInfo->status)
	{
		case 0:
		PRINT("appZdoStartNetworkConf 0\n");
		break;
		
		case 1:
		PRINT("appZdoStartNetworkConf 1\n");
		break;
		
		case 2:
		PRINT("appZdoStartNetworkConf 2\n");
		break;
		
		case 3:
		PRINT("appZdoStartNetworkConf 3\n");
		break;
		
		case 4:
		PRINT("appZdoStartNetworkConf 4\n");
		break;
		
		case 5:
		PRINT("appZdoStartNetworkConf 5\n");
		break;
		
		case 6:
		PRINT("appZdoStartNetworkConf 6\n");
		break;
		
		case 7:
		PRINT("appZdoStartNetworkConf 7\n");
		break;
		
		case 8:
		PRINT("appZdoStartNetworkConf 8\n");
		break;
		
		case 9:
		PRINT("appZdoStartNetworkConf 9\n");
		break;
		
		case 10:
		PRINT("appZdoStartNetworkConf 10\n");
		break;
		
		case 11:
		PRINT("appZdoStartNetworkConf 11\n");
		break;
		
		case 12:
		PRINT("appZdoStartNetworkConf 12\n");
		break;
		
		case 13:
		PRINT("appZdoStartNetworkConf 13\n");
		break;
		
		case 14:
		PRINT("appZdoStartNetworkConf 14\n");
		break;
		
		case 15:
		PRINT("appZdoStartNetworkConf 15\n");
		break;
		
		case 16:
		PRINT("appZdoStartNetworkConf 16\n");
		break;
		
		case 17:
		PRINT("appZdoStartNetworkConf 17\n");
		break;
		
		case 18:
		PRINT("appZdoStartNetworkConf 18\n");
		break;
		
		case 19:
		PRINT("appZdoStartNetworkConf 19\n");
		break;
		
		case 231:
		PRINT("appZdoStartNetworkConf 231\n");
		break;
		
		case 232:
		PRINT("appZdoStartNetworkConf 232\n");
		break;
		
		case 233:
		PRINT("appZdoStartNetworkConf 233\n");
		break;
		
		case 234:
		PRINT("appZdoStartNetworkConf 234\n");
		break;
		
		case 255:
		PRINT("appZdoStartNetworkConf 255\n");
		break;
		
		default:
		if(startInfo->status < 232)
			PRINT("<50\n");
		else
			PRINT(">50\n");
		break;
	}*/
	
	sprintf(buf,"appZdoStartNetworkConf STATUS:0x%X PANId:0x%X ADDRESS:0x%X CHANNEL:0x%X\r\n", startInfo->status, startInfo->PANId, startInfo->shortAddr, startInfo->activeChannel);
	HAL_WriteUsart(&usart,&buf,strlen(buf));
	
	if(startInfo->status == ZDO_SUCCESS_STATUS)
	{
		appState = APP_IN_NETWORK_STATE;
		
		SYS_PostTask(APL_TASK_ID);
	}
}	

void Network_Init()
{
	 DeviceType_t deviceType = APP_DEVICE_TYPE;
	 uint16_t nwkAddr;
	 ExtAddr_t extAddr;

	 //#if deviceType == DEVICE_TYPE_COORDINATOR
	//	nwkAddr = 0;
	//	extAddr = 0xAAAAAAAAAAAAAAAALL;
	 //#else
		nwkAddr = 1;
		extAddr = 0x00LL; 
	 //#endif
	 
	// Set the NWK address value to Config Server
	CS_WriteParameter(CS_NWK_ADDR_ID, &nwkAddr);
	CS_WriteParameter(CS_NWK_UNIQUE_ADDR_ID, &(bool){true});
		 
	CS_WriteParameter(CS_UID_ID, &extAddr);
		 

	// Set the deviceType value to Config Server
	CS_WriteParameter(CS_DEVICE_TYPE_ID, &deviceType);
	
	
	/*
	DeviceType_t type_device = DEVICE_TYPE_COORDINATOR;
	uint16_t panId;
	ExtAddr_t extAddr;
	CS_WriteParameter(CS_DEVICE_TYPE_ID, &type_device);
	
	CS_ReadParameter(CS_DEVICE_TYPE_ID, &type_device);
	CS_ReadParameter(CS_NWK_PANID_ID, &panId);
	CS_ReadParameter(CS_UID_ID, &extAddr);*/
	
	sprintf(buf,"DEVICE_TYPE:0x%X NwkADDRESS:0x%X ExtADDRESS:0x%X\r\n", deviceType, nwkAddr, extAddr);
	HAL_WriteUsart(&usart,&buf,strlen(buf));
	/*switch (type_device)
	{
		case 0:
		PRINT("DEVICE_TYPE COORDINATOR\r\n");
		break;
		
		case 1:
		PRINT("DEVICE_TYPE ROUTER\r\n");
		break;
		
		case 2:
		PRINT("DEVICE_TYPE END_DEVICE\r\n");
		break;
		
		default:
		PRINT("DEVICE_TYPE UNKNOWN\r\n");
		break;
	}*/
		
				// Set application endpoint properties
				endpointParams.simpleDescriptor = &simpleDescriptor;
				endpointParams.APS_DataInd = APS_DataIndication;
				// Register endpoint
				APS_RegisterEndpointReq(&endpointParams);
		
	// Start network
	zdoStartReq.ZDO_StartNetworkConf = appZdoStartNetworkConf;
	ZDO_StartNetworkReq(&zdoStartReq);	
	
	appState = APP_STARTING_NETWORK_STATE;
	SYS_PostTask(APL_TASK_ID);
}

/*******************************************************************************
  Description: application task handler.

  Parameters: none.

  Returns: nothing.
*******************************************************************************/
void APL_TaskHandler(void)
{
	if(appState == APP_INITING_STATE)
	{
		GPIO_D6_make_out();
		GPIO_D6_clr();
	
		Usart_Init();	
		//PRINT("Hello\n");
	
		Adc_Init();
	
		// Configure blink timer
		blinkTimer.interval = 1000;       // Timer interval
		blinkTimer.mode     = TIMER_REPEAT_MODE;        // Repeating mode (TIMER_REPEAT_MODE or TIMER_ONE_SHOT_MODE)
		blinkTimer.callback = blinkTimerFired;          // Callback function for timer fire event
  
		Network_Init();
	}else if(appState == APP_STARTING_NETWORK_STATE)
	{
		HAL_StartAppTimer(&blinkTimer);                 // Start blink timer
	}else if(appState == APP_IN_NETWORK_STATE)
	{
		HAL_StopAppTimer(&blinkTimer);
		GPIO_D6_set();
	}
}

uint16_t adc_meastemp (void)
{
	ADCSRC = 10<<ADSUT0; // set start-up time
	ADCSRB = 1<<MUX5; // set MUX5 first
	ADMUX = (3<<REFS0) + (9<<MUX0); // store new ADMUX, 1.6V AREF
	// switch ADC on, set prescaler, start conversion
	ADCSRA = (1<<ADEN) + (1<<ADSC) + (4<<ADPS0);
	do{
		
	} while( (ADCSRA & (1<<ADSC))); // wait for conversion end
	ADCSRA = 0; // disable the ADC
	return (ADC);
}

/*******************************************************************************
  Description: blinkying timer fire event handler.

  Parameters: none.

  Returns: nothing.
*******************************************************************************/
static void blinkTimerFired()
{
	//Toggle the led
	GPIO_D6_toggle();
	/*
	HAL_ReadAdc(&adcDescriptor, HAL_ADC_CHANNEL7);
	sprintf(buf,"ADC:%u\r\n", adcBuffer);
	//double temp = 1.13 * adc_meastemp() - 272.8;
	/*const float chipTempOffset = 272.8;
	const float chipTempCoeff = 1.13;
	float temp = (((float)adc_meastemp() * chipTempCoeff) - chipTempOffset );
	sprintf(buf,"TEMP:%d C\r\n", (int)temp);
	HAL_WriteUsart(&usart,&buf,strlen(buf));*/
}

/*******************************************************************************
  Description: just a stub.

  Parameters: are not used.

  Returns: nothing.
*******************************************************************************/
void ZDO_MgmtNwkUpdateNotf(ZDO_MgmtNwkUpdateNotf_t *nwkParams)
{
  nwkParams = nwkParams;  // Unused parameter warning prevention
  PRINT("ZDO_MgmtNwkUpdateNotf\r\n");
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
	
	GPIO_D6_make_out();	
	GPIO_D6_clr();
	
  for(;;)
  {
    //SYS_RunTask();
  }
}

//eof blink.c
