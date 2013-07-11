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
#include <avr/wdt.h>
#include "config.h"
#include "RTC.h"
#include "configManager.h"

#define FIRMWARE_VERSION 1

#define softReset()        \
do                          \
{                           \
	wdt_enable(WDTO_15MS);  \
	for(;;)                 \
	{                       \
	}                       \
} while(0)

#define MIN(x,y) ((x) < (y) ? (x) : (y))
#define MAX(x,y) ((x) > (y) ? (x) : (y))
#define CEILING(x,y) (((x) + (y) - 1) / (y))

#define CREATE_CIRCULARBUFFER(elemsType, bufferSize)										\
struct {																					\
	unsigned int	start;					   /* index of oldest element              */	\
	unsigned int	end;					   /* index at which to write new element  */	\
	elemsType		buffer[bufferSize];		   /* vector of elements                   */	\
}

GLOBAL RUNNING_CONFIGURATION_t runningConfiguration;
GLOBAL volatile _Bool validConfiguration;

/*	Time vars	4 bytes  */
GLOBAL volatile TIME_t currentTime;
GLOBAL volatile DATE_t currentDate;
GLOBAL volatile WEEKDAY_t currentWeek;
GLOBAL volatile _Bool validTime;
GLOBAL volatile _Bool validDate;



#endif /* GLOBALS_H_ */