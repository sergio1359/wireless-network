/*
 * operationsManager.h
 *
 * Created: 11/05/2013 22:39:38
 *  Author: Victor
 */ 


#ifndef OPERATIONSMANAGER_H_
#define OPERATIONSMANAGER_H_

#include "configManager.h"
#include "uartManager.h"

void OM_Init(void);
void OM_ProccessInternalOperation(OPERATION_HEADER_t* operation_header);
void OM_ProccessExternalOperation(INPUT_UART_HEADER_t* input_header, OPERATION_HEADER_t* operation_header);
uint8_t OM_ProccessResponseOperation(OPERATION_HEADER_t* operation_header);
uint8_t OM_ProccessResponseWithBodyOperation(OPERATION_HEADER_t* operation_header, uint8_t* bodyPtr, uint8_t bodyLength);
void OM_ProccessUARTOperation(OUTPUT_UART_HEADER_t* output_header, OPERATION_HEADER_t* operation_header);

#endif /* OPERATIONSMANAGER_H_ */