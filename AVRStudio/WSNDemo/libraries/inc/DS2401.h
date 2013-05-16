/*
 * DS2401.h
 *
 * Created: 10/04/2013 15:14:20
 *  Author: Victor
 */ 


#ifndef DS2401_H_
#define DS2401_H_

#include "sysTypes.h"

#define SERIAL_NUMBER_SIZE    6

//
// Dallas Semiconductor DS2401 Silicon Serial Number
//

extern uint8_t serialNumber[SERIAL_NUMBER_SIZE];
extern _Bool serialNumber_OK;

_Bool DS2401_Init(void);

#endif /* DS2401_H_ */