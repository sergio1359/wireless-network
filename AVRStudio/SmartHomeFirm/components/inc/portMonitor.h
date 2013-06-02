/*
 * portMonitor.h
 *
 * Created: 25/01/2013 17:27:24
 *  Author: Victor
 */ 


#ifndef PORTMONITOR_H_
#define PORTMONITOR_H_

#include "EEPROM.h"
#include "DIGITAL.h"
#include "ANALOG.h"
#include "globals.h"
#include "config.h"
#include "modules.h"
#include "timeManager.h"

void PortMonitor_TaskHandler();
void PortMonitor_LaunchOperations(uint8_t pinAddress);

extern uint8_t lastValuesD[NUM_PORTS];
extern uint8_t lastValuesA[ANALOG_PINS];

#endif /* PORTMONITOR_H_ */