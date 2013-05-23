/*
 * operationsManager.h
 *
 * Created: 11/05/2013 22:39:38
 *  Author: Victor
 */ 


#ifndef OPERATIONSMANAGER_H_
#define OPERATIONSMANAGER_H_

#include "EEPROM.h"

void OM_ProccessOperation(OPERATION_HEADER_t* operation_header, _Bool byCopy);


#endif /* OPERATIONSMANAGER_H_ */