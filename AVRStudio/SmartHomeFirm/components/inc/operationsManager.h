/*
 * operationsManager.h
 *
 * Created: 11/05/2013 22:39:38
 *  Author: Victor
 */ 


#ifndef OPERATIONSMANAGER_H_
#define OPERATIONSMANAGER_H_

#include "configManager.h"

void OM_ProccessInternalOperation(OPERATION_HEADER_t* operation_header, _Bool byCopy);
void OM_ProccessExternalOperation(OPERATION_HEADER_t* operation_header);
void OM_ProccessResponseOperation(OPERATION_HEADER_t* operation_header);
void OM_ProccessResponseWithBodyOperation(OPERATION_HEADER_t* operation_header, uint8_t* bodyPtr, uint8_t bodyLength);

#endif /* OPERATIONSMANAGER_H_ */