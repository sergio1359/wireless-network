#include <stdlib.h>
#include <stdio.h>
#include <string.h>
#include "config.h"
#include "hal.h"
#include "phy.h"
#include "sys.h"
#include "nwk.h"
#include "halUart.h"
#include "halSleep.h"
#include "sysTimer.h"
#include "leds.h"

#include "RTC.h"
#include "EEPROM.h"
#include "ANALOG.h"
#include "DIGITAL.h"
#include "DS2401.h"

#define DECLARING_GLOBALS
#include "globals.h"

#ifdef APP_ENABLE_OTA_SERVER
#include "otaServer.h"
#endif

#ifdef APP_ENABLE_OTA_CLIENT
#include "otaClient.h"
#endif

/*****************************************************************************
*****************************************************************************/
#if APP_ADDR == 0
#define APP_CAPTION     "Coordinator"
#define APP_NODE_TYPE   0
#define APP_COORDINATOR 1
#elif APP_ADDR < 0x8000
#define APP_CAPTION     "Router"
#define APP_NODE_TYPE   1
#define APP_ROUTER      1
#else
#define APP_CAPTION     "End Device"
#define APP_NODE_TYPE   2
#define APP_ENDDEVICE   1
#endif

#define APP_CAPTION_SIZE  (sizeof(APP_CAPTION) - 1)

static SYS_Timer_t appNetworkStatusTimer;

/*****************************************************************************
*****************************************************************************/
//TODO: MOVE TO THE RIGHT FILE (UART_MANAGER COMPONENT)
void HAL_UartBytesReceived(uint16_t bytes)
{
	for (uint16_t i = 0; i < bytes; i++)
	{
		HAL_UartWriteByte(HAL_UartReadByte());
	}
	ledToggle(2);
}

#if APP_ROUTER || APP_ENDDEVICE
/*****************************************************************************
*****************************************************************************/
static void appNetworkStatusTimerHandler(SYS_Timer_t *timer)
{
	ledToggle(0);
	(void)timer;
}
#endif

/*****************************************************************************
*****************************************************************************/
unsigned int adcVal;
static void appSendData(void)
{
	numWrite(currentTime.hour);
	HAL_UartWriteByte(':');
	numWrite(currentTime.minute);
	HAL_UartWriteByte(':');
	numWrite(currentTime.second);
	HAL_UartPrint("\r\n");
	
	//ADC_Reference(REF_DEFAULT);
	//adcVal = ADC_Read(ADC4);
	ADC_Reference(REF_INTERNAL_16);
	adcVal = ADC_Read(INTERNAL_TEMP);
	adcVal = 1.13 * adcVal - 272.8;
	HAL_UartPrint("TEMP: ");
	numWrite(adcVal);
	//itoa(adcVal, buf, 10);
	//HAL_UartPrint(buf);
	HAL_UartPrint("ºC\r\n");
}

#ifdef PHY_ENABLE_RANDOM_NUMBER_GENERATOR
/*****************************************************************************
*****************************************************************************/
void PHY_RandomConf(uint16_t rnd)
{
	srand(rnd);
}
#endif

/*****************************************************************************
*****************************************************************************/
static void appInit(void)
{
	ledsInit();

	#if APP_ROUTER || APP_ENDDEVICE
	appNetworkStatusTimer.interval = 500;
	appNetworkStatusTimer.mode = SYS_TIMER_PERIODIC_MODE;
	appNetworkStatusTimer.handler = appNetworkStatusTimerHandler;
	SYS_TimerStart(&appNetworkStatusTimer);
	
	HAL_UartPrint("ROUTER\r\n");
	#else
	HAL_UartPrint("COORDINATOR\r\n");
	ledOn(LED_NETWORK);
	#endif

	#ifdef PHY_ENABLE_RANDOM_NUMBER_GENERATOR
	PHY_RandomReq();
	#endif
}

/*****************************************************************************
*****************************************************************************/
#define ITERATIONS_BW_SEND 1
_Bool counter = ITERATIONS_BW_SEND + 1;
static void APP_TaskHandler(void)
{
	if(counter>ITERATIONS_BW_SEND)
	{
		appInit();
	}else if(counter==ITERATIONS_BW_SEND){
		appSendData();
	}
}

/*****************************************************************************
*****************************************************************************/
int main(void)
{
	SYS_Init();
	
	HAL_UartInit(38400);
	
	RTC_Init();
	currentTime.hour = 11;
	currentTime.minute = 11;
	currentTime.second = 00;
	
	EEPROM_Init();
	
	RTC_ValidateTime(&currentTime);
	
	if(DS2401_Init())
	{
		HAL_UartPrint("SERIAL NUMBER: ");
		for(int i = 0; i < SERIAL_NUMBER_SIZE - 1; i++)
		{
			numWriteHEX(serialNumber[i]);
			HAL_UartWriteByte('-');
		}
		numWriteHEX(serialNumber[SERIAL_NUMBER_SIZE - 1]);
		HAL_UartPrint("\r\n\r\n");
	}else
	{
		HAL_UartPrint("SERIAL NUMBER: NOT DETECTED!\r\n");
	}
	
	#ifdef APP_ENABLE_OTA_SERVER
	OTA_ServerInit();
	#endif
	#ifdef APP_ENABLE_OTA_CLIENT
	OTA_ClientInit();
	#endif
	
	Radio_Init();
	
	initializeLCD();

	while (1)
	{
		SYS_TaskHandler();
		HAL_UartTaskHandler();
		#ifdef APP_ENABLE_OTA_SERVER
		OTA_ServerTaskHandler();
		#endif
		#ifdef APP_ENABLE_OTA_CLIENT
		OTA_ClientTaskHandler();
		#endif
		APP_TaskHandler();
		PortMonitor_TaskHandler();
	}
}
