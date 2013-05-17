/**
 * \file otaInterface.c
 *
 * \brief OTA callback functions handlers
 *
 * Copyright (C) 2012 Atmel Corporation. All rights reserved.
 *
 * \asf_license_start
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met:
 *
 * 1. Redistributions of source code must retain the above copyright notice,
 *    this list of conditions and the following disclaimer.
 *
 * 2. Redistributions in binary form must reproduce the above copyright notice,
 *    this list of conditions and the following disclaimer in the documentation
 *    and/or other materials provided with the distribution.
 *
 * 3. The name of Atmel may not be used to endorse or promote products derived
 *    from this software without specific prior written permission.
 *
 * 4. This software may only be redistributed and used in connection with an
 *    Atmel microcontroller product.
 *
 * THIS SOFTWARE IS PROVIDED BY ATMEL "AS IS" AND ANY EXPRESS OR IMPLIED
 * WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF
 * MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NON-INFRINGEMENT ARE
 * EXPRESSLY AND SPECIFICALLY DISCLAIMED. IN NO EVENT SHALL ATMEL BE LIABLE FOR
 * ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
 * DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS
 * OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION)
 * HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT,
 * STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN
 * ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
 * POSSIBILITY OF SUCH DAMAGE.
 *
 * \asf_license_stop
 *
 * $Id: otaInterface.c 5223 2012-09-10 16:47:17Z ataradov $
 *
 */

#include "sysTypes.h"
#include "otaClient.h"
#include "otaServer.h"


#if defined(APP_ENABLE_OTA_SERVER) || defined(APP_ENABLE_OTA_CLIENT)
/*****************************************************************************
*****************************************************************************/
static bool appUpgradeInProgress = false;
static uint16_t appOtaSize;
#endif 


#ifdef APP_ENABLE_OTA_CLIENT

/*****************************************************************************
*****************************************************************************/
#define STARTING_ADDRESS        (SYS_DEVICE_SIZE / 2)
#define IAP_SWITCH_HANDLER      (SYS_DEVICE_SIZE - 4)
#define IAP_WRITE_PAGE_HANDLER  (SYS_DEVICE_SIZE - 2)

/*****************************************************************************
*****************************************************************************/
typedef void (*IapWritePage_t)(uint32_t addr, uint16_t *buf);
typedef void (*IapSwitch_t)(void);

/*****************************************************************************
*****************************************************************************/
static uint32_t appOtaAddr;
static uint8_t appOtaPage[SYS_PAGE_SIZE];

/*****************************************************************************
*****************************************************************************/
static void *appGetIapHandler(uint32_t addr)
{
  void *handler;

#if defined(__ICCAVR__)
  memcpy_P((void *)&handler, (void const __farflash *)addr, sizeof(void *));
#else
  handler = (void *)pgm_read_word_far(addr);
#endif

  return handler;
}

/*****************************************************************************
*****************************************************************************/
static void appWritePage(void)
{
  IapWritePage_t iapWritePage = (IapWritePage_t)appGetIapHandler(IAP_WRITE_PAGE_HANDLER);

  if (0 == appOtaSize)
    return;

  for (uint16_t i = appOtaSize; i < sizeof(appOtaPage); i++)
    appOtaPage[i] = 0xff;

  ATOMIC_SECTION_ENTER
    iapWritePage(appOtaAddr, (uint16_t *)appOtaPage);
  ATOMIC_SECTION_LEAVE
}

/*****************************************************************************
*****************************************************************************/
static void appSwitch(void)
{
  IapSwitch_t iapSwitch = (IapSwitch_t)appGetIapHandler(IAP_SWITCH_HANDLER);

  ATOMIC_SECTION_ENTER
    iapSwitch();
  ATOMIC_SECTION_LEAVE
}

/*****************************************************************************
*****************************************************************************/
void OTA_ClientNotification(OTA_Status_t status)
{
  if (OTA_UPGRADE_STARTED_STATUS == status)
  {
    if (appUpgradeInProgress)
      return; // Should not happen at all

    appUpgradeInProgress = true;
    appOtaAddr = STARTING_ADDRESS;
    appOtaSize = 0;
  }
  else if (OTA_UPGRADE_COMPLETED_STATUS == status)
  {
    appWritePage();
    appSwitch();
    appUpgradeInProgress = false;
  }
  else
  {
    // Some error happened, upgrade will be stopped.
    appUpgradeInProgress = false;
  }
}

/*****************************************************************************
*****************************************************************************/
void OTA_ClientBlockIndication(uint8_t size, uint8_t *data)
{
  if (appUpgradeInProgress)
  {
    while (size > 0)
    {
      for (; (size > 0) && (appOtaSize < sizeof(appOtaPage)); appOtaSize++, size--)
        appOtaPage[appOtaSize] = *data++;

      if (sizeof(appOtaPage) == appOtaSize)
      {
        appWritePage();
        appOtaAddr += sizeof(appOtaPage);
        appOtaSize = 0;
      }
    }
    OTA_ClientBlockConfirm(OTA_SUCCESS_STATUS);
  }
  else
  {
    OTA_ClientBlockConfirm(OTA_SW_FAIL_STATUS);
  }
}

#endif // APP_ENABLE_OTA_CLIENT


#ifdef APP_ENABLE_OTA_SERVER
static uint8_t* data_ptr;
static uint8_t progressPercent;
static uint8_t progressDiv;

void OTA_Server_Start(uint16_t deviceAddr, uint8_t* dataBuffer, uint32_t size)
{
	data_ptr = dataBuffer;
	progressPercent = 0;
	appOtaSize = size;
	progressDiv = (appOtaSize / SYS_PAGE_SIZE) * 100;
	
	OTA_ServerStartUpdrade(deviceAddr, size);
}

/*****************************************************************************
*****************************************************************************/
void OTA_ServerNotification(OTA_Status_t status)
{
	if (OTA_CLIENT_READY_STATUS == status) //Ready to send
	{
		if (appUpgradeInProgress)//Continue
		{
			data_ptr += SYS_PAGE_SIZE;
			appOtaSize -= SYS_PAGE_SIZE;
			progressPercent += progressDiv;
		}
		
		appUpgradeInProgress = true;

		OTA_ServerSendBlock(data_ptr);
	}
	else if (OTA_UPGRADE_COMPLETED_STATUS == status)//FINISH
	{
		if(appOtaSize == 0) //Upgrade success!
		{
			//Show Success message
			progressPercent = 100;
		}
		
		appUpgradeInProgress = false;
	}else //OTA_NO_RESPONSE_STATUS || OTA_CRC_ERROR_STATUS || OTA_SW_FAIL_STATUS
	{
		//Show Error message
		
		// Some error happened, upgrade will be stopped.
		appUpgradeInProgress = false;
	}
}

#endif // APP_ENABLE_OTA_SERVER

