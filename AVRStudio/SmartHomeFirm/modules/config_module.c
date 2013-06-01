/*
 * config_module.c
 *
 * Created: 31/05/2013 19:41:10
 *  Author: Victor
 */ 
#include "modules.h"
#include "globals.h"
#include <util/crc16.h>

struct
{
	OPERATION_HEADER_t header;
	CONFIG_CHECKSUM_RESPONSE_MESSAGE_t response;
}checksumResponse;

_Bool receivingState;
uint16_t index;
uint8_t currentFragment;
uint8_t totalExpected;

RUNNING_CONFIGURATION_t configBuffer;

_Bool validateReceivedConfig(void);

void configModule_Init(void)
{
	checksumResponse.header.opCode = ConfigChecksumResponse;
	
	receivingState = false;
	index = 0;
}

void configModule_NotificationInd(uint8_t sender, OPERATION_HEADER_t* notification)
{
	
}

CONFIG_WRITE_HEADER_MESSAGE_t* msg;
void configWrite_Handler(OPERATION_HEADER_t* operation_header)
{
	if(operation_header->opCode == ConfigWrite)
	{
		_Bool acceptFragment = false;
		msg = (CONFIG_WRITE_HEADER_MESSAGE_t*)(operation_header + 1);
		if(receivingState)
		{
			if(msg->fragmentTotal != totalExpected)
			{
				receivingState = false;
				//TODO:SEND ERROR MESSAGE (FRAGMENT TOTAL NOT EXPECTED)
			}else if(msg->fragment == (currentFragment+1))
			{
				currentFragment++;
				acceptFragment= true;
			}else
			{
				receivingState = false;
				//TODO:SEND ERROR MESSAGE (ERROR FRAGMENT ORDER)
			}
		}else if(msg->fragment == 0) //FirstFrame
		{
			index = 0;
			receivingState = true;
			totalExpected = msg->fragmentTotal;
			acceptFragment = true;
		}else
		{
			receivingState = false;
			//TODO:SEND ERROR MESSAGE (ERROR WAITING FIRST FRAGMENT)
		}
		
		if(acceptFragment)
		{
			memcpy((uint8_t*)configBuffer.raw, (uint8_t*)(msg + 1), sizeof(uint8_t) * msg->length);//(uint16_t)msg->length);
			index += (uint16_t)msg->length;
			
			if(currentFragment == msg->fragmentTotal)//ALL RECEIVED
			{
				totalExpected = 0;
				if(validateReceivedConfig())
				{
					//memcpy((uint8_t*)runningConfiguration.raw, (uint8_t*)configBuffer.raw, configBuffer.topConfiguration.deviceInfo.length);
					validConfiguration = true;
					
					//TODO: Copy to EEPROM and restart instead
					//EEPROM_Write_Block(configBuffer.raw, 0x00, configBuffer.topConfiguration.deviceInfo.length);
					//softReset();
				}
			}
		}
	}
}

void configRead_Handler(OPERATION_HEADER_t* operation_header)
{
	if(operation_header->opCode == ConfigRead)
	{
		if(receivingState)
		{
			//TODO: SEND OR LOG ERROR (BUSY RECEIVING STATE)
		}else
		{
		
		}
	}else if(operation_header->opCode == ConfigReadResponse)
	{
		//TODO: NOTIFICATION
	}		
}

void configChecksum_Handler(OPERATION_HEADER_t* operation_header)
{
	if(operation_header->opCode == ConfigChecksum)
	{
		checksumResponse.header.destinationAddress = operation_header->sourceAddress;
		checksumResponse.header.sourceAddress = runningConfiguration.topConfiguration.networkConfig.deviceAddress;
		checksumResponse.response.checksum = runningConfiguration.topConfiguration.deviceInfo.checkSum;
		
		//TODO: Send a CONFIG_CHECKSUM_RESPONSE_MESSAGE_t (check)
		Radio_AddMessageByCopy(&checksumResponse.header);
	}else if(operation_header->opCode == ConfigChecksumResponse)
	{
		//TODO: NOTIFICATION
	}		
}

_Bool validateReceivedConfig()
{
	uint16_t eeprom_size = configBuffer.topConfiguration.deviceInfo.length;
	uint16_t eeprom_crc = configBuffer.topConfiguration.deviceInfo.checkSum;
	
	if(eeprom_size != 0xFFFF && eeprom_size != 0x00)
	{
		configBuffer.topConfiguration.deviceInfo.checkSum = 0;
		
		uint16_t acc = 0;
		for(int i = 0; i < eeprom_size; i++)
		acc = _crc16_update(acc, configBuffer.raw[i]);
		
		configBuffer.topConfiguration.deviceInfo.checkSum = eeprom_crc;
		
		return eeprom_crc == acc;
	}
	
	return false;
}