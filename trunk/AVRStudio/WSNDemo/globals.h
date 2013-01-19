/*
 * globals.h
 *
 * Created: 15/10/2012 20:08:48
 *  Author: Victor
 */ 


#ifndef GLOBALS_H_
#define GLOBALS_H_

#ifdef DECLARING_GLOBALS
#define GLOBAL
#else
#define GLOBAL extern
#endif

#include <stdint.h>
#include "config.h"
#include "RTC.h"

/*	Network vars	21 bytes  */
GLOBAL volatile uint16_t device_Address;
GLOBAL volatile uint8_t current_Channel;
GLOBAL volatile uint16_t current_PanId;
GLOBAL volatile uint8_t securityKey[16];

/*	Time vars	4 bytes  */
GLOBAL volatile TIME current_Time;
GLOBAL volatile _Bool valid_Time;



#endif /* GLOBALS_H_ */