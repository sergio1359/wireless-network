/************************************************************************//**
  \file zclMemoryManager.h

  \brief
    The header file describes the ZCL Memory Manager interface

  \author
    Atmel Corporation: http://www.atmel.com \n
    Support email: avr@atmel.com

  Copyright (c) 2008-2012, Atmel Corporation. All rights reserved.
  Licensed under Atmel's Limited License Agreement (BitCloudTM).

  \internal
    History:
    27.11.08 A. Potashov - Created.
******************************************************************************/

#ifndef _ZCLMEMORYMANAGER_H
#define _ZCLMEMORYMANAGER_H

/******************************************************************************
                   Includes section
******************************************************************************/
#include <types.h>
#include <aps.h>
#include <appFramework.h>
#include <macAddr.h>
#include <zcl.h>
#include <dbg.h>

/******************************************************************************
                   Types section
******************************************************************************/
typedef union
{
  APS_DataReq_t apsDataReq;
  APS_DataInd_t apsDataInd;
} ZclMmPrimitive_t;

typedef enum
{
  ZCL_MM_BUFFER_FREE  = 0x00,
  ZCL_MM_BUFFER_BUSY  = 0x01
} ZclMmBufferBusy_t;

typedef struct
{
  ZclMmPrimitive_t  primitive;
  uint8_t *frame;
} ZclMmBuffer_t;

typedef struct
{
  ZclMmBufferBusy_t          busy;
  ZclMmBuffer_t              buf;
} ZclMmBufferDescriptor_t;

/******************************************************************************
                   Prototypes section
******************************************************************************/
/*************************************************************************//**
\brief Looks for and return free zcl memory buffer.

\return pointer to memory buffer or NULL if there is no free buffer.
*****************************************************************************/
ZclMmBuffer_t *zclMmGetMem(void);

/*************************************************************************//**
\brief Free zcl buffer.

\param[in] mem - pointer to used zcl buffer.
*****************************************************************************/
void zclMmFreeMem(ZclMmBuffer_t *mem);

#endif  //#ifndef _ZCLMEMORYMANAGER_H


//eof zclMemoryManager.h
