/************************************************************************//**
  \file zclAttributes.h

  \brief
    Header for implementation of state-independent attributes logic for ZCL.

  \author
    Atmel Corporation: http://www.atmel.com \n
    Support email: avr@atmel.com

  Copyright (c) 2008-2012, Atmel Corporation. All rights reserved.
  Licensed under Atmel's Limited License Agreement (BitCloudTM).

  \internal
    History:
    16.03.2012 - D.Loskutnikov - extracted from zcl.c/zcl.h
******************************************************************************/

#ifndef _ZCL_ATTRIBUTES_H
#define _ZCL_ATTRIBUTES_H

/******************************************************************************
                           Functions prototypes section
******************************************************************************/

/*************************************************************************//**
  \brief Finds command descriptor by cluster, direction and command id.

  \param[in] cluster   - pointer to cluster.
  \param[in] direction - command direction (client or server).
  \param[in] commandId - command unique identifier.
  \return command descriptor if found, NULL otherwise.
*****************************************************************************/
ZclCommand_t * zclGetCommandByCluster(ZCL_Cluster_t *cluster, uint8_t direction, ZCL_CommandId_t commandId);

/*************************************************************************//**
  \brief Finds command descriptor by endpoint id, cluster id and command id.

  \param[in] endpointId - endpoint number.
  \param[in] clusterId - cluster unique identifier.
  \param[in] direction - command direction (client or server).
  \param[in] commandId - command unique identifier.
  \return command descriptor if found, NULL otherwise.
*****************************************************************************/
ZclCommand_t * zclGetCommand(Endpoint_t endpointId, ClusterId_t clusterId, uint8_t direction, ZCL_CommandId_t commandId);

/*************************************************************************//**
  \brief Finds attribute descriptor by endpoint id, cluster id , cluster side
         and attribute id.

  \param[in] endpointId - endpoint unique identifier.
  \param[in] clusterId - cluster unique identifier.
  \param[in] clusterSide - cluster side (client or server).
  \param[in] attributeId - attribute unique identifier.
  \return attribute descriptor if found, NULL otherwise.
*****************************************************************************/
ZclAttribute_t * zclGetAttribute(Endpoint_t endpointId, ClusterId_t clusterId, uint8_t clusterSide, ZCL_AttributeId_t attributeId);

/*************************************************************************//**
  \brief Compares current attribute value with last reported value and if
         difference exceeds reportableChange value returns true.

  \param[in] pAttr - pointer to attribute.

  \return true if reporting is needed, false otherwise.
*****************************************************************************/
bool isOnChangeReportingNeeded(const ZclAttribute_t *pAttr);

/**************************************************************************//**
\brief Safe version of ZCL_ReadAttributeValue without caller buffer overflow

\param[in] endpointId - endpoint of attribute to be read
\param[in] clusterId - cluster of attribute to be read
\param[in] clusterSide - client or server side
\param[in] attrId - attribute identifier
\param[out] attrType - attribute type
\param[out] attrValue - buffer to read attribute value
\param[in] maxSize - maximum size of buffer
\return status of operation
******************************************************************************/
ZCL_Status_t zclReadAttributeValueSafe(Endpoint_t endpointId,
                                       ClusterId_t clusterId,
                                       uint8_t clusterSide,
                                       ZCL_AttributeId_t attrId,
                                       uint8_t *attrType,
                                       uint8_t *attrValue,
                                       uint16_t maxSize);

/**************************************************************************//**
\brief Read indexed (structured) attribute from array

\param[in,out] selector - pointer to pointer to index selector
\param[in] endpointId - endpoint of attribute to be read
\param[in] clusterId - cluster of attribute to be read
\param[in] clusterSide - client or server side
\param[in] attrId - attribute identifier
\param[out] attrType - attribute type
\param[out] attrValue - buffer to read attribute value
\param[in] maxSize - maximum size of buffer
\return status of operation
******************************************************************************/
ZCL_Status_t zclReadIndexedAttribute(const ZCL_Selector_t *selector,
                                     Endpoint_t endpointId,
                                     ClusterId_t clusterId,
                                     uint8_t clusterSide,
                                     ZCL_AttributeId_t attrId,
                                     uint8_t *attrType,
                                     uint8_t *attrValue,
                                     uint16_t maxSize);

#endif // _ZCL_ATTRIBUTES_H
//eof zclAttributes.h
