/*
 * EEPROM.h
 *
 * Created: 12/10/2012 17:42:55
 *  Author: Victor
 */ 


#ifndef EEPROM_H_
#define EEPROM_H_

#include "RTC.h"

#define FALLING_EDGE 1
#define RISIN_EDGE 2
#define BOTH_EDGE 3

typedef struct{
	uint16_t deviceAddress;
	uint8_t channel;
	uint16_t panId;
	uint8_t securityKey[16];
}NETWORK_CONFIG;

typedef struct{
	uint8_t maskIO;   //0-> inputs		1-> outputs
	uint8_t maskAD;   //0-> analog		1-> digital
	uint8_t defaultValue;   //Initial value
}PORT_CONFIG;

typedef struct{
	unsigned pin0_ChangeType : 2;//Two for pin	10-> risingEdge		01-> fallingEdge	11->both
	unsigned pin1_ChangeType : 2;
	unsigned pin2_ChangeType : 2;
	unsigned pin3_ChangeType : 2;
	unsigned pin4_ChangeType : 2;
	unsigned pin5_ChangeType : 2;
	unsigned pin6_ChangeType : 2;
	unsigned pin7_ChangeType : 2;
	
	//MESSAGE[8][] events;   //Events to send on activation
}DIGITAL_EVENT_CONFIG;

typedef struct{
	uint8_t increment;   //Difference between the last and current value
	uint8_t threshold;	 //hysteresis value to exceed
	//MESSAGE[8][] events;   //Events to send on activation
}ANALOG_EVENT_CONFIG;

typedef struct{
	TIME time;				 //Activation time
	//MESSAGE[] events;   //Events to send on activation
}TIME_EVENT_CONFIG;

uint8_t EEPROM_Read(int address);
void EEPROM_Write(int address, uint8_t value);

#endif /* EEPROM_H_ */