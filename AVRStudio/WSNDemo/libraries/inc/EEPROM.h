/*
 * EEPROM.h
 *
 * Created: 12/10/2012 17:42:55
 *  Author: Victor
 */ 


#ifndef EEPROM_H_
#define EEPROM_H_

#include "RTC.h"
#include <avr/eeprom.h>
#include "config.h"

#define FALLING_EDGE 1
#define RISIN_EDGE 2
#define BOTH_EDGE 3

typedef struct{
	uint8_t deviceType;
	uint16_t model;
	uint16_t checkSum; //crc16
}DEVICE_INFO_t;

typedef struct{
	uint16_t deviceAddress;
	uint8_t channel;
	uint16_t panId;
	uint8_t securityKey[16];
}NETWORK_CONFIG_t;

typedef struct{
	unsigned pin0_ChangeType : 2;	//Two for pin	10-> risingEdge		01-> fallingEdge	11->both
	unsigned pin1_ChangeType : 2;
	unsigned pin2_ChangeType : 2;
	unsigned pin3_ChangeType : 2;
	unsigned pin4_ChangeType : 2;
	unsigned pin5_ChangeType : 2;
	unsigned pin6_ChangeType : 2;
	unsigned pin7_ChangeType : 2;
}DIGITAL_PORT_CHANGE_TYPE_t;

typedef struct{
	uint8_t maskIO;   //0-> inputs		1-> outputs
	uint8_t maskAD;   //0-> analog		1-> digital
	uint8_t defaultValuesD;		 //Initial values DIGITAL
	uint16_t changeTypeD; //Two for pin	10-> risingEdge		01-> fallingEdge	11->both
}PORT_CONFIG_t;

typedef struct{
	uint8_t defaultValuesA[8];   //Initial values ANALOG outputs (PWM)
}PWM_CONFIG_t;

typedef struct{
	uint8_t increment[8];   //Difference between the last and current value
	uint8_t threshold[8];	 //hysteresis value to exceed
}ANALOG_EVENT_CONFIG_t;

typedef struct{
	uint16_t destinationAddress;
	uint8_t operation;
}EVENT_HEADER_t;

typedef struct{
	TIME_t activationTime;		  //Activation time
	EVENT_HEADER_t eventHeader;   //Header of the event to send on activation
}TIME_EVENT_HEADER_t;

typedef struct
{
	uint8_t eventAddress; //Relative
	TIME_t start;
	TIME_t end;
}EVENT_RESTRICTION_t;


typedef struct {
	DEVICE_INFO_t deviceInfo;
	NETWORK_CONFIG_t networkConfig;
	PORT_CONFIG_t portConfig_PB;
	PORT_CONFIG_t portConfig_PD;
	PORT_CONFIG_t portConfig_PE;
	PORT_CONFIG_t portConfig_PF;
	PORT_CONFIG_t portConfig_PG;
	PWM_CONFIG_t pwmConfig_PWM0;
	PWM_CONFIG_t pwmConfig_PWM1;
	PWM_CONFIG_t pwmConfig_PWM2;
	PWM_CONFIG_t pwmConfig_PWM3;
	PWM_CONFIG_t pwmConfig_PWM4;
	PWM_CONFIG_t pwmConfig_PWM5;
	PWM_CONFIG_t pwmConfig_PWM6;
	PWM_CONFIG_t pwmConfig_PWM7;
	ANALOG_EVENT_CONFIG_t analogConfig_ADC0;
	ANALOG_EVENT_CONFIG_t analogConfig_ADC1;
	ANALOG_EVENT_CONFIG_t analogConfig_ADC2;
	ANALOG_EVENT_CONFIG_t analogConfig_ADC3;
	ANALOG_EVENT_CONFIG_t analogConfig_ADC4;
	ANALOG_EVENT_CONFIG_t analogConfig_ADC5;
	ANALOG_EVENT_CONFIG_t analogConfig_ADC6;
	ANALOG_EVENT_CONFIG_t analogConfig_ADC7;
}TOP_CONFIGURATION;

typedef struct
{
	TOP_CONFIGURATION  topConfiguration;
	uint8_t raw[EEPROM_SIZE];
}RUNNING_CONFIGURATION_t;

void inline EEPROM_Init();

uint8_t inline EEPROM_Read_Byte(int address);
void inline  EEPROM_Read_Block(void * buffer, const void * address, size_t length);
void inline EEPROM_Write_Byte(int address, uint8_t value);
void inline EEPROM_Write_Block(const void * buffer, const void * address, size_t length);

#endif /* EEPROM_H_ */