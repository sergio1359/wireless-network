/*
* config_module.c
*
* Created: 31/05/2013 19:41:10
*  Author: Victor
*/
#include "modulesManager.h"
#include "globals.h"
#include <util/crc16.h>
#include <sysTimer.h>

#include "APP_SESSION.h"

struct
{
	OPERATION_HEADER_t header;
	FIRMWARE_VERSION_READ_RESPONSE_MESSAGE_t response;
}firmwareResponse;

struct
{
	OPERATION_HEADER_t header;
	SHIELD_MODEL_READ_RESPONSE_MESSAGE_t response;
}shieldModelResponse;

struct
{
	OPERATION_HEADER_t header;
	BASE_MODEL_READ_RESPONSE_MESSAGE_t response;
}baseModelResponse;

struct
{
	OPERATION_HEADER_t header;
	CONFIG_WRITE_RESPONSE_MESSAGE_t response;
}configWriteResponse;

struct
{
	OPERATION_HEADER_t header;
	CONFIG_READ_RESPONSE_HEADER_MESSAGE_t response;
}configReadResponse;

struct
{
	OPERATION_HEADER_t header;
	CONFIG_CHECKSUM_RESPONSE_MESSAGE_t response;
}checksumResponse;


enum
{
	ERROR_CONFIG_SIZE_TOO_BIG = 5,
	ERROR_CONFIG_INVALID_CHECKSUM,
	ERROR_CONFIG_SIZE_NOT_EXPECTED,
}CONFIG_WRITE_STATUS_CODES;

static WriteSession_t writeConfigSession;

static ReadSession_t readConfigSession;

static RUNNING_CONFIGURATION_t configWriteBuffer;

static SYS_Timer_t configResetTimer;

static void configResetTimerHandler(SYS_Timer_t *timer);
static inline uint8_t validateReceivedConfig(void);

void configModule_Init(void)
{
	//Set responses opCodes
	firmwareResponse.header.opCode		= FirmwareVersionReadResponse;
	shieldModelResponse.header.opCode	= ShieldModelReadResponse;
	baseModelResponse.header.opCode		= BaseModelReadResponse;
	configWriteResponse.header.opCode	= ConfigWriteResponse;
	configReadResponse.header.opCode	= ConfigReadResponse;
	checksumResponse.header.opCode		= ConfigChecksumResponse;
	
	//Configure Timer
	configResetTimer.interval = 2000;
	configResetTimer.mode = SYS_TIMER_INTERVAL_MODE;
	configResetTimer.handler = configResetTimerHandler;
	
	writeConfigSession.receivingState = false;
	writeConfigSession.writeBuffer = (uint8_t*)configWriteBuffer.raw;
	
	readConfigSession.sendingState = false;
	readConfigSession.readBuffer = (uint8_t*)runningConfiguration.raw;
}

void configModule_NotificationInd(uint8_t sender, OPERATION_HEADER_t* notification)
{
	
}

/*- Reset --------------------------------------------------------*/
void configReset_Handler(OPERATION_HEADER_t* operation_header)
{
	softReset();
}


/*- Firmware Version ---------------------------------------------*/
void configFirmware_Handler(OPERATION_HEADER_t* operation_header)
{
	if(operation_header->opCode == FirmwareVersionRead)
	{
		firmwareResponse.header.sourceAddress = runningConfiguration.topConfiguration.networkConfig.deviceAddress;
		firmwareResponse.header.destinationAddress = operation_header->sourceAddress;
		firmwareResponse.response.version = FIRMWARE_VERSION;
		
		OM_ProccessResponseOperation(&firmwareResponse.header);
	}else if(operation_header->opCode == FirmwareVersionReadResponse)
	{
		//TODO: SEND NOTIFICATION
	}
}

/*- Shield Model -------------------------------------------------*/
void configShield_Handler(OPERATION_HEADER_t* operation_header)
{
	if(operation_header->opCode == ShieldModelRead)
	{
		shieldModelResponse.header.sourceAddress = runningConfiguration.topConfiguration.networkConfig.deviceAddress;
		shieldModelResponse.header.destinationAddress = operation_header->sourceAddress;
		shieldModelResponse.response.model = shieldModel;
		
		OM_ProccessResponseOperation(&shieldModelResponse.header);
	}else if(operation_header->opCode == ShieldModelReadResponse)
	{
		//TODO: SEND NOTIFICATION
	}
}


/*- Base Model ---------------------------------------------------*/
void configBaseModel_Handler(OPERATION_HEADER_t* operation_header)
{
	if(operation_header->opCode == BaseModelRead)
	{
		baseModelResponse.header.sourceAddress = runningConfiguration.topConfiguration.networkConfig.deviceAddress;
		baseModelResponse.header.destinationAddress = operation_header->sourceAddress;
		baseModelResponse.response.model = baseModel;
		
		OM_ProccessResponseOperation(&baseModelResponse.header);
	}else if(operation_header->opCode == BaseModelReadResponse)
	{
		//TODO: SEND NOTIFICATION
	}
}


/*- Config Write -------------------------------------------------*/
void configWrite_Handler(OPERATION_HEADER_t* operation_header)
{
	if(operation_header->opCode == ConfigWrite)
	{
		_Bool acceptFragment = false;
		CONFIG_WRITE_HEADER_MESSAGE_t* msg = (CONFIG_WRITE_HEADER_MESSAGE_t*)(operation_header + 1);
		
		configWriteResponse.header.destinationAddress = operation_header->sourceAddress;
		configWriteResponse.header.sourceAddress = runningConfiguration.topConfiguration.networkConfig.deviceAddress;
		configWriteResponse.response.fragment = msg->fragment;
		configWriteResponse.response.fragmentTotal = msg->fragmentTotal;
		
		if(writeConfigSession.receivingState)
		{
			if(msg->fragmentTotal != writeConfigSession.totalRecvExpected)
			{
				writeConfigSession.receivingState = false;
				configWriteResponse.response.code = ERROR_FRAGMENT_TOTAL_NOT_EXPECTED;
			}else if(msg->fragment == (writeConfigSession.currentRecvFragment + 1))
			{
				writeConfigSession.currentRecvFragment++;
				acceptFragment= true;
			}else
			{
				writeConfigSession.receivingState = false;
				configWriteResponse.response.code = ERROR_FRAGMENT_ORDER;
			}
		}else if(msg->fragment == 0) //FirstFrame
		{
			writeConfigSession.waitingForReset = false;
			writeConfigSession.currentRecvFragment = 0;
			writeConfigSession.currentRecvIndex = 0;
			
			writeConfigSession.receivingState = true;
			writeConfigSession.totalRecvExpected = msg->fragmentTotal;
			
			acceptFragment = true;
			configWriteResponse.response.code = OK;
		}else
		{
			writeConfigSession.receivingState = false;
			configWriteResponse.response.code = ERROR_WAITING_FIRST_FRAGMENT;
		}
		
		if(acceptFragment)
		{
			if((msg->length + writeConfigSession.currentRecvIndex) >= EEPROM_SIZE)
			{
				configWriteResponse.response.code = ERROR_CONFIG_SIZE_TOO_BIG;
			}else
			{
				memcpy(&writeConfigSession.writeBuffer[writeConfigSession.currentRecvIndex], (uint8_t*)(msg + 1), sizeof(uint8_t) * msg->length);
				writeConfigSession.currentRecvIndex += (uint16_t)msg->length;
				
				if(writeConfigSession.currentRecvFragment == msg->fragmentTotal)//ALL RECEIVED
				{
					configWriteResponse.response.code = validateReceivedConfig();
					if(configWriteResponse.response.code == OK)
					{
						EEPROM_Write_Block(writeConfigSession.writeBuffer, 0x00, configWriteBuffer.topConfiguration.configHeader.length);
						
						writeConfigSession.waitingForReset = true;
					}
					
					writeConfigSession.receivingState = false;
					writeConfigSession.totalRecvExpected = 0;
				}
			}
		}
		
		OM_ProccessResponseOperation(&configWriteResponse.header);
	}
}

void configWrite_DataConf(OPERATION_DataConf_t *req)
{
	//TODO: There is a problem when we send the config by radio. Something is wrong here
	if (req->sendOk)
	{
		if(writeConfigSession.waitingForReset)//All receviced. Wainting for reset
		{
			//SYS_TimerStart(&configResetTimer);	
		}
	}else
	{
		OM_ProccessResponseOperation(&configWriteResponse.header); //Resend last response
	}
}


/*- Config Read --------------------------------------------------*/
void configRead_Handler(OPERATION_HEADER_t* operation_header)
{
	if(operation_header->opCode == ConfigRead)
	{
		configReadResponse.header.sourceAddress = runningConfiguration.topConfiguration.networkConfig.deviceAddress;		
		configReadResponse.header.destinationAddress = operation_header->sourceAddress;
		
		if(readConfigSession.sendingState && operation_header->sourceAddress != readConfigSession.destinationAddress)
		{
			//BUSY SENDING CONFIG STATE
			configReadResponse.response.fragment = 0;
			configReadResponse.response.fragmentTotal = 0;
			configReadResponse.response.length = 0;
		}else
		{
			readConfigSession.sendingState = true;
			readConfigSession.destinationAddress = operation_header->sourceAddress;
			
			readConfigSession.currentSendIndex = 0;
			readConfigSession.currentSendFragment = 0;
			readConfigSession.totalSendExpected = (runningConfiguration.topConfiguration.configHeader.length / MAX_CONTENT_MESSAGE_SIZE);
			readConfigSession.currentSendFrameSize = MIN(MAX_CONTENT_MESSAGE_SIZE, runningConfiguration.topConfiguration.configHeader.length - readConfigSession.currentSendIndex);
			
			configReadResponse.response.fragment = readConfigSession.currentSendFragment;
			configReadResponse.response.fragmentTotal = readConfigSession.totalSendExpected;
			configReadResponse.response.length = readConfigSession.currentSendFrameSize;
			
			OM_ProccessResponseWithBodyOperation(&configReadResponse.header, readConfigSession.readBuffer, readConfigSession.currentSendFrameSize);
		}
	}else if(operation_header->opCode == ConfigReadConfirmation)
	{
		if(readConfigSession.sendingState)
		{
			CONFIG_READ_CONFIRMATION_MESSAGE_t* msg = (CONFIG_READ_CONFIRMATION_MESSAGE_t*)(operation_header + 1);
			
			if(msg->fragment == readConfigSession.currentSendFragment && msg->fragmentTotal == readConfigSession.totalSendExpected)
			{
				if(msg->code == 0x00) //'OK'
				{
					if(readConfigSession.currentSendFragment <= readConfigSession.totalSendExpected)	 //Something to send
					{
						readConfigSession.currentSendIndex += readConfigSession.currentSendFrameSize;
						readConfigSession.currentSendFragment++;
						
						readConfigSession.currentSendFrameSize = MIN(MAX_CONTENT_MESSAGE_SIZE, runningConfiguration.topConfiguration.configHeader.length - readConfigSession.currentSendIndex);
						
						configReadResponse.response.fragment = readConfigSession.currentSendFragment;
						configReadResponse.response.fragmentTotal = readConfigSession.totalSendExpected;
						configReadResponse.response.length = readConfigSession.currentSendFrameSize;
						
						OM_ProccessResponseWithBodyOperation(&configReadResponse.header,&readConfigSession.readBuffer[readConfigSession.currentSendIndex], readConfigSession.currentSendFrameSize);
					}else
					{
						//Finish
						readConfigSession.sendingState = false;
					}
				}else
				{
					//Something wrong at server size. Abort current session
					readConfigSession.sendingState = false;
				}
			}else
			{
				readConfigSession.sendingState = false;
				//TODO: SEND OR LOG ERROR (FRAGMENT OR FRAGMENT TOTAL NOT EXPECTED)
			}
		}else
		{
			//TODO: SEND OR LOG ERROR (NOT SENDING CONFIG)
		}
	}else if(operation_header->opCode == ConfigReadResponse)
	{
		//TODO: SEND NOTIFICATION
	}
}

void configRead_DataConf(OPERATION_DataConf_t *req)
{
	if (!req->sendOk)
	{
		OM_ProccessResponseWithBodyOperation(&configReadResponse.header,&readConfigSession.readBuffer[readConfigSession.currentSendIndex], readConfigSession.currentSendFrameSize);//Resend last response
	}
}


/*- Checksum --------------------------------------------------*/
void configChecksum_Handler(OPERATION_HEADER_t* operation_header)
{
	if(operation_header->opCode == ConfigChecksum)
	{
		checksumResponse.header.destinationAddress = operation_header->sourceAddress;
		checksumResponse.header.sourceAddress = runningConfiguration.topConfiguration.networkConfig.deviceAddress;
		checksumResponse.response.checksum = runningConfiguration.topConfiguration.configHeader.checkSum;
		
		OM_ProccessResponseOperation(&checksumResponse.header);
	}else if(operation_header->opCode == ConfigChecksumResponse)
	{
		//TODO: NOTIFICATION
	}
}


/*- WakeUp ---------------------------------------------------*/
void configWakeUp_Handler(OPERATION_HEADER_t* operation_header)
{
	if(operation_header->opCode == WakeUp)
	{
		//TODO: NOTIFICATION
	}		
}


/*- Internal functions ---------------------------------------*/
void configResetTimerHandler(SYS_Timer_t *timer)
{
	softReset();
	(void)timer;
}

uint8_t validateReceivedConfig()
{
	uint16_t eeprom_size = configWriteBuffer.topConfiguration.configHeader.length;
	uint16_t eeprom_crc = configWriteBuffer.topConfiguration.configHeader.checkSum;
	
	if(eeprom_size != 0xFFFF && eeprom_size != 0x00 && eeprom_size == writeConfigSession.currentRecvIndex)
	{
		configWriteBuffer.topConfiguration.configHeader.checkSum = 0;
		
		uint16_t acc = 0;
		for(int i = 0; i < eeprom_size; i++)
		acc = _crc16_update(acc, configWriteBuffer.raw[i]);
		
		configWriteBuffer.topConfiguration.configHeader.checkSum = eeprom_crc;
		
		return (eeprom_crc == acc) ? OK : ERROR_CONFIG_INVALID_CHECKSUM;
	}else
	{
		return ERROR_CONFIG_SIZE_NOT_EXPECTED;
	}
}