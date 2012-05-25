/*
 * RF24_init.h
 *
 *  Created on: 18/05/2012
 *      Author: Victor
 */

#ifndef RF24_INIT_H_
#define RF24_INIT_H_

#include "nRF24L01.h"
#include <WProgram.h>

//Bank1 Register Configuartion Operate==============================================
const volatile  uint32_t  Bank1_Reg0_Reg13[] =
{
    0xE2014B40,
    0x00004BC0,
    0x028CFCD0,
    0x41390099,
    0x0B869ED9,
    0xA67F0624,
    0x00000000,
    0x00000000,
    0x00000000,
    0x00000000,
    0x00000000,
    0x00000000,
    0x00127300,
    0x36B48000,
};

const uint8_t   Bank1_Reg14[11] =
{
    0X41, 0X20, 0X08, 0X04, 0X81,
    0X20, 0XCF, 0XF7,0XFE, 0XFF, 0XFF
};

//Bank0_Register Configuration Operate============================================
const uint8_t Bank0_RegAct[2][2] =
{
	{DYNPD,		0x00},		//Disable dynamic payload length
	{FEATURE,	0x04}		//EN_DPL= 1, EN_ACK_PAY = 0, EN_DYN_ACK = 0
};

const volatile uint8_t Bank0_Reg_Init[21][2] =
{
	{CONFIG,		0x0F},	//PRX,CRC=2,ENCRC,POWRUP;
	{EN_AA,			0x3F},	//all data pipe AUTO ACK;
	{EN_RXADDR,		0x03},	//RX address data pipe 0 an 1;
	{SETUP_AW,		0x03},	//RX/TX address width 5B
	{SETUP_RETR,	0x1F},	//auto retransmit count 15,delay 500us;
	{RF_CH,			0x4C},  //2400+0x4C = 2476MHz;
	{RF_SETUP,		0x17},  //CONT_WAVE=0 RF_DR=1Mbps PLL_LOCK=1 RF_PWR=0dBm;
	{STATUS,		0x70},	//Clear interrupt flag;
	{OBSERVE_TX,	0x00},
	{CD,			0x00},
	{RX_ADDR_P2,	0xc3},
	{RX_ADDR_P3,	0xc4},
	{RX_ADDR_P4,	0xc5},
	{RX_ADDR_P5,	0xc6},
	{RX_PW_P0,		0x08},	//RX Payload Length = 8 bytes
	{RX_PW_P1,		0x08},
	{RX_PW_P2,		0x08},
	{RX_PW_P3,		0x08},
	{RX_PW_P4,		0x08},
	{RX_PW_P5,		0x08},
	{FIFO_STATUS,	0x11}
};

//Write RX0 And TX Address=======================================================
const uint64_t RX_Address_P0 = 0xE7E7E7E7E7LL;
const uint64_t RX_Address_P1 = 0xC2C2C2C2C2LL;

#endif /* RF24_INIT_H_ */
