/**************************************************************************//**
  \file zclOTAUCluster.h

  \brief Declaration of the private OTAU interface

  \author
    Atmel Corporation: http://www.atmel.com \n
    Support email: avr@atmel.com

  Copyright (c) 2008-2012, Atmel Corporation. All rights reserved.
  Licensed under Atmel's Limited License Agreement (BitCloudTM).

  \internal
  History:
    13.05.10 A. Khromykh - Created.
  Last change:
    $Id: zclOtauDiscovery.h 20614 2012-03-23 09:37:37Z arazinkov $
*******************************************************************************/
#ifndef _ZCLOTAUDISCOVERY_H
#define _ZCLOTAUDISCOVERY_H

/******************************************************************************
                        Includes section
******************************************************************************/
#include <zcl.h>
#include <clusters.h>
#include <zdo.h>

/******************************************************************************
                           Types section
******************************************************************************/
typedef union
{
  ZDO_ZdpReq_t          zdpOtauReq;
#ifdef _ZCL_SECURITY_
  APS_RequestKeyReq_t   apsKeyReq;
#endif // _ZCL_SECURITY_
  ZCL_Request_t         zclCommandReq;
} OtauReqMemory_t;

typedef union
{
  ZCL_OtauQueryNextImageReq_t   uQueryNextImageReq;
  ZCL_OtauImageBlockReq_t       uImageBlockReq;
  ZCL_OtauImagePageReq_t        uImagePageReq;
  ZCL_OtauUpgradeEndReq_t       uUpgradeEndReq;
} OtauZclReqMemory_t;

/* This structure always should consist actual values
   to continue broken uploading. */
typedef struct
{
/* Size of internal subimage within main image (for example size of eeprom area) */
  uint32_t  imageInternalLength;
/* Amount bytes were received from internal information structute about the internal subimage.
   Should be 8 bytes. 4 byte mcu start address of the subimage. 4 byte size of the subimage. */
  uint8_t   internalAddressStatus;
/* Requested offset from the current file. */
  uint32_t  currentFileOffset;
/* Size of requested data. */
  uint8_t   currentDataSize;
/* Reminder of the not received data. */
  uint32_t  imageRemainder;
/* Address of the page begin. Address form main image. */
  uint32_t  imagePageOffset;
} OtauImageAuxVar_t;

/***************************************************************************//**
  \brief
  ZCL OTAU Cluster parametres.
*******************************************************************************/
typedef struct
{
  uint32_t        imageSize;
  uint8_t         imageBlockData[OFD_BLOCK_SIZE];
  uint8_t        *imagePageData;
} ZclOtauClientImageBuffer_t;

/******************************************************************************
                   Prototypes section
******************************************************************************/
/***************************************************************************//**
\brief Restart poll timer

\param[in] period - timer fired time
\param[in] cb     - pointer to fired method
******************************************************************************/
void otauStartGenericTimer(uint32_t period, void (* cb)(void));

/***************************************************************************//**
\brief Restart discovery service with context gap
******************************************************************************/
void otauStartPollDiscovery(void);

/***************************************************************************//**
\brief Entry point to client process
******************************************************************************/
void otauClientEntryPoint(void);

/***************************************************************************//**
\brief Start discovery of upgrade server
******************************************************************************/
void ZCL_OtauUpgradeServerDiscovery(void);

/***************************************************************************//**
\brief Server IEEE addresses request
******************************************************************************/
void otauServerExtAddrReq(void);

#endif /* _ZCLOTAUDISCOVERY_H */
