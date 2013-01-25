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
#include "EEPROM.h"

GLOBAL RUNNING_CONFIGURATION_t runningConfiguration;

/*	Time vars	4 bytes  */
GLOBAL volatile TIME_t currentTime;
GLOBAL volatile _Bool validTime;



#endif /* GLOBALS_H_ */