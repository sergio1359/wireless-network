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

//LIBRARIES
#include "RTC.h"
#include "EEPROM.h"
#include "ANALOG.h"
#include "DIGITAL.h"
#include "DS2401.h"
#include "DHT11.h"
#include "DISPLAY.h"

#define DECLARING_GLOBALS
#include "globals.h"

//COMPONENTS
#include "uartManager.h"

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

_Bool initialized = false;
_Bool sendFlag = false;

#if APP_ROUTER || APP_ENDDEVICE
/*****************************************************************************
*****************************************************************************/
static void appNetworkStatusTimerHandler(SYS_Timer_t *timer)
{
	sendFlag = true;
		
	ledToggle(0);
	(void)timer;
}
#endif

/*****************************************************************************
*****************************************************************************/
uint16_t adcVal;
uint8_t temp;
static void appSendData(void)
{
	
	numWrite(currentTime.hour);
	HAL_UartWriteByte(':');
	numWrite(currentTime.minute);
	HAL_UartWriteByte(':');
	numWrite(currentTime.second);
	HAL_UartPrint("\r\n");
	
	DISPLAY_Clear();
	DISPLAY_SetCursor(0,0);
	DISPLAY_WriteString("TIME: ");
	DISPLAY_SetCursor(0,1);
	DISPLAY_WriteNumber(currentTime.hour,2);
	DISPLAY_WriteByte(':');
	DISPLAY_WriteNumber(currentTime.minute,2);
	DISPLAY_WriteByte(':');
	DISPLAY_WriteNumber(currentTime.second,2);
	
	ADC_Reference(REF_DEFAULT);
	adcVal = ADC_Read(ADC4);
	//ADC_Reference(REF_INTERNAL_16);
	//adcVal = ADC_Read(INTERNAL_TEMP);
	//adcVal = 1.13 * adcVal - 272.8;
	HAL_UartPrint("ADC4: ");
	numWrite(adcVal);
	HAL_UartPrint("\r\n");
	
	temp = DHT11_ReadTemperature(PINADDRESS('D', 0));
	if(temp != DHT11_ERROR)
	{
		HAL_UartPrint("TEMP: ");
		numWrite(temp);
		HAL_UartPrint("ºC\t");
		
		temp = DHT11_ReadHumidity(PINADDRESS('D', 0));//D0
		HAL_UartPrint("HUM: ");
		numWrite(temp);
		HAL_UartPrint("%\r\n");
	}else
	{
		HAL_UartPrint("DHT11 not detected\r\n");
	}
	
	sendFlag = false;
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
	appNetworkStatusTimer.interval = 1000;//500
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
	
	initialized = true;
}

/*****************************************************************************
*****************************************************************************/
static void APP_TaskHandler(void)
{
	if(!initialized)
	{
		appInit();
	}else if(sendFlag){
		appSendData();
	}
}
#include <util/delay.h>
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
	
	//EEPROM_Init();
	
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
	
	ledsInit();
	DISPLAY_Init(PINADDRESS('D',1), PINADDRESS('D',0), PINADDRESS('B',1), PINADDRESS('B',3), PINADDRESS('B',5), PINADDRESS('B',7));
	DISPLAY_WriteString("HELLO!");

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
		//PortMonitor_TaskHandler();
	}
}
