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

enum
{
	OK = 0,
	ERROR_FRAGMENT_TOTAL_NOT_EXPECTED,
	ERROR_FRAGMENT_ORDER,
	ERROR_WAITING_FIRST_FRAGMENT,
	ERROR_CONFIG_SIZE_TOO_BIG,
	ERROR_CONFIG_INVALID_CHECKSUM,
	ERROR_CONFIG_SIZE_NOT_EXPECTED,
	ERROR_BUSY_RECEIVING_STATE
}RESPONSE_ERROR_CODES;


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

static _Bool receivingState;
static _Bool waitingForReset;
static uint8_t currentRecvFragment;
static uint8_t totalRecvExpected;
static uint16_t currentRecvIndex;

static _Bool sendingState;
static uint8_t currentSendFragment;
static uint8_t totalSendExpected;
static uint16_t currentSendIndex;
static uint16_t currentSendFrameSize;

static RUNNING_CONFIGURATION_t configBuffer;

static SYS_Timer_t configResetTimer;

static void configResetTimerHandler(SYS_Timer_t *timer);
static _Bool validateReceivedConfig(void);

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
	configResetTimer.interval = 1000;
	configResetTimer.mode = SYS_TIMER_INTERVAL_MODE;
	configResetTimer.handler = configResetTimerHandler;
	
	receivingState = false;
	sendingState = false;
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

void sendFirmwareResponse(uint16_t address)
{
	firmwareResponse.header.sourceAddress = runningConfiguration.topConfiguration.networkConfig.deviceAddress;
	firmwareResponse.header.destinationAddress = address;
	firmwareResponse.response.version = FIRMWARE_VERSION;
	
	OM_ProccessResponseOperation(&firmwareResponse.header);
}

/*- Shield Model -------------------------------------------------*/
void configShield_Handler(OPERATION_HEADER_t* operation_header)
{
	if(operation_header->opCode == ShieldModelRead)
	{
		shieldModelResponse.header.sourceAddress = runningConfiguration.topConfiguration.networkConfig.deviceAddress;
		shieldModelResponse.header.destinationAddress = operation_header->sourceAddress;
		shieldModelResponse.response.model = SHIELD_MODEL;
		
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
		baseModelResponse.response.model = BASE_MODEL;
		
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
		
		if(receivingState)
		{
			if(msg->fragmentTotal != totalRecvExpected)
			{
				receivingState = false;
				configWriteResponse.response.code = ERROR_FRAGMENT_TOTAL_NOT_EXPECTED;
			}else if(msg->fragment == (currentRecvFragment+1))
			{
				currentRecvFragment++;
				acceptFragment= true;
			}else
			{
				receivingState = false;
				configWriteResponse.response.code = ERROR_FRAGMENT_ORDER;
			}
		}else if(msg->fragment == 0) //FirstFrame
		{
			waitingForReset = false;
			currentRecvFragment = 0;
			currentRecvIndex = 0;
			
			receivingState = true;
			totalRecvExpected = msg->fragmentTotal;
			acceptFragment = true;
			configWriteResponse.response.code = OK;
		}else
		{
			receivingState = false;
			configWriteResponse.response.code = ERROR_WAITING_FIRST_FRAGMENT;
		}
		
		if(acceptFragment)
		{
			if((msg->length + currentRecvIndex) >= EEPROM_SIZE)
			{
				configWriteResponse.response.code = ERROR_CONFIG_SIZE_TOO_BIG;
			}else
			{
				memcpy((uint8_t*)&configBuffer.raw[currentRecvIndex], (uint8_t*)(msg + 1), sizeof(uint8_t) * msg->length);
				currentRecvIndex += (uint16_t)msg->length;
				
				if(currentRecvFragment == msg->fragmentTotal)//ALL RECEIVED
				{
					if(validateReceivedConfig())
					{
						EEPROM_Write_Block(configBuffer.raw, 0x00, configBuffer.topConfiguration.configHeader.length);
						
						waitingForReset = true;
					}else
					{
						configWriteResponse.response.code = ERROR_CONFIG_INVALID_CHECKSUM;
					}
					receivingState = false;
					totalRecvExpected = 0;
				}
			}
		}
		
		OM_ProccessResponseOperation(&configWriteResponse.header);
	}
}

void configWrite_DataConf(OPERATION_DataConf_t *req)
{
	if(waitingForReset)//All receviced. Wainting for reset
	{
		if (req->sendOk)
		{
			SYS_TimerStart(&configResetTimer);
		}else
		{
			OM_ProccessResponseOperation(&configWriteResponse.header); //Resend last response	
		}
	}
}


/*- Config Read --------------------------------------------------*/
void configRead_Handler(OPERATION_HEADER_t* operation_header)
{
	if(operation_header->opCode == ConfigRead)
	{
		configReadResponse.header.destinationAddress = operation_header->sourceAddress;
		configReadResponse.header.sourceAddress = runningConfiguration.topConfiguration.networkConfig.deviceAddress;
		
		if(sendingState)
		{
			//TODO: SEND OR LOG ERROR (BUSY SENDING CONFIG STATE)
		}else
		{
			sendingState = true;
			
			currentSendIndex = 0;
			currentSendFragment = 0;
			totalSendExpected = (runningConfiguration.topConfiguration.configHeader.length / MAX_CONTENT_MESSAGE_SIZE);
			currentSendFrameSize = MIN(MAX_CONTENT_MESSAGE_SIZE, runningConfiguration.topConfiguration.configHeader.length - currentSendIndex);
			
			configReadResponse.response.fragment = currentSendFragment;
			configReadResponse.response.fragmentTotal = totalSendExpected;
			configReadResponse.response.length = currentSendFrameSize;
			
			//TODO: Add frame from currentSendIndex(0) to currentSendFrameSize
			OM_ProccessResponseWithBodyOperation(&configReadResponse.header,&runningConfiguration.raw, currentSendFrameSize);
		}
	}else if(operation_header->opCode == ConfigReadConfirmation)
	{
		if(sendingState)
		{
			CONFIG_READ_CONFIRMATION_MESSAGE_t* msg = (CONFIG_READ_CONFIRMATION_MESSAGE_t*)(operation_header + 1);
			
			if(msg->fragment == currentSendFragment && msg->fragmentTotal == totalSendExpected)
			{
				if(msg->code == 0x00) //'OK'
				{
					if(currentSendFragment <= totalSendExpected)	 //Something to send
					{
						currentSendIndex += currentSendFrameSize;
						currentSendFragment++;
						
						currentSendFrameSize = MIN(MAX_CONTENT_MESSAGE_SIZE, runningConfiguration.topConfiguration.configHeader.length - currentSendIndex);
						
						configReadResponse.response.fragment = currentSendFragment;
						configReadResponse.response.fragmentTotal = totalSendExpected;
						configReadResponse.response.length = currentSendFrameSize;
						
						OM_ProccessResponseWithBodyOperation(&configReadResponse.header,&runningConfiguration.raw[currentSendIndex], currentSendFrameSize);
					}else
					{
						//Finish
						sendingState = false;
					}
				}else
				{
					//Something wrong at server size. Abort current session
					sendingState = false;
				}
			}else
			{
				sendingState = false;
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
	//TODO: Handle Master Responses here
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

void configResetTimerHandler(SYS_Timer_t *timer)
{
	softReset();
	(void)timer;
}

_Bool validateReceivedConfig()
{
	uint16_t eeprom_size = configBuffer.topConfiguration.configHeader.length;
	uint16_t eeprom_crc = configBuffer.topConfiguration.configHeader.checkSum;
	
	if(eeprom_size != 0xFFFF && eeprom_size != 0x00 && eeprom_size == currentRecvIndex)
	{
		configBuffer.topConfiguration.configHeader.checkSum = 0;
		
		uint16_t acc = 0;
		for(int i = 0; i < eeprom_size; i++)
		acc = _crc16_update(acc, configBuffer.raw[i]);
		
		configBuffer.topConfiguration.configHeader.checkSum = eeprom_crc;
		
		return eeprom_crc == acc;
	}else
	{
		//TODO: SEND OR LOG ERROR (ERROR CONFIG SIZE NOT EXPECTED)
	}
	
	return false;
}