/*
* DISPLAY.c
*
* Created: 18/05/2013 17:28:21
*  Author: Victor
*/
#include "DISPLAY.h"

#include <util/delay.h>
#include "DIGITAL.h"

#define LCD_LINE_SIZE (16+1)
#define LCD_LINES 2

#define	DATA_REGISTER		0
#define	COMMAND_REGISTER	1

unsigned char position;

VARPIN(D4);
VARPIN(D5);
VARPIN(D6);
VARPIN(D7);
VARPIN(EE);
VARPIN(RS);

void waitForLCD(void);
void writeCharacterToLCD(unsigned character);
void writeNibbleToLCD(unsigned char selectedRegister, unsigned char nibble);
void writeByteToLCD(unsigned char selectedRegister, unsigned char byte);

void DISPLAY_Init(uint16_t EE_PinAddress, uint16_t RS_PinAddress, uint16_t D4_PinAddress, uint16_t D5_PinAddress, uint16_t D6_PinAddress, uint16_t D7_PinAddress)
{
	//	Set the position counter to 0
	position = 0;
	
	VARPIN_UPDATE(EE, EE_PinAddress);
	VARPIN_UPDATE(RS, RS_PinAddress);
	VARPIN_UPDATE(D4, D4_PinAddress);
	VARPIN_UPDATE(D5, D5_PinAddress);
	VARPIN_UPDATE(D6, D6_PinAddress);
	VARPIN_UPDATE(D7, D7_PinAddress);
	
	//	Set the proper data direction for output
	VARPIN_OUT(EE);
	VARPIN_OUT(RS);
	VARPIN_OUT(D4);
	VARPIN_OUT(D5);
	VARPIN_OUT(D6);
	VARPIN_OUT(D7);

	//	First, delay for at least 15ms after power on
	_delay_ms(50);
	
	//	Set for 4-bit LCD mode
	writeNibbleToLCD(COMMAND_REGISTER, 0x03);
	_delay_ms(5);
	
	writeNibbleToLCD(COMMAND_REGISTER, 0x03);
	_delay_ms(5);
	
	writeNibbleToLCD(COMMAND_REGISTER, 0x03);
	_delay_ms(5);
	
	writeNibbleToLCD(COMMAND_REGISTER, 0x02);
	
	//	Function set
	writeByteToLCD(COMMAND_REGISTER, 0x28);
	_delay_ms(5);
	
	//	Turn display off
	writeByteToLCD(COMMAND_REGISTER, 0x08);
	_delay_ms(13);
	
	//	Clear LCD and return home
	writeByteToLCD(COMMAND_REGISTER, 0x01);
	_delay_ms(13);
	
	writeByteToLCD(COMMAND_REGISTER, 0x0c); // not blinking cursor
	//writeByteToLCD(COMMAND_REGISTER, 0x0f);   // blinking cursor
	_delay_ms(13);
}

void DISPLAY_WriteByte(char byte)
{
	writeByteToLCD(DATA_REGISTER, byte);
}

void DISPLAY_WriteString(char* stringPointer)
{
	//	So long as the data that is pointed to by the string pointer
	//	is valid (ie, not the null pointer) then the data will be written
	//	to the LCD
	while (*stringPointer)
	writeByteToLCD(DATA_REGISTER, *stringPointer++);
}

void DISPLAY_WriteNumber(int integer, unsigned char digits)
{
	//	Break down the original number into the thousands, hundreds, tens,
	//	and ones places and then immediately write that value to the LCD
	unsigned char thousands = integer / 1000;
	if(digits>3)
	writeByteToLCD(DATA_REGISTER, thousands + '0');
	
	unsigned char hundreds = (integer - thousands*1000) / 100;
	if(digits>2)
	writeByteToLCD(DATA_REGISTER, hundreds + '0');
	
	unsigned char tens = (integer - thousands*1000 - hundreds*100 ) / 10;
	if(digits>1)
	writeByteToLCD(DATA_REGISTER, tens + '0');
	
	unsigned char ones = (integer - thousands*1000 - hundreds*100 - tens*10);
	writeByteToLCD(DATA_REGISTER, ones + '0');
}

void DISPLAY_Clear(void)
{
	//	Send command to LCD (0x01)
	writeByteToLCD(COMMAND_REGISTER, 0x01);
	
	_delay_ms(2);
	
	//	Set the current position to 0
	position = 0;
}

void DISPLAY_SetCursor(unsigned char column, unsigned char row)
{
	int row_offsets[] = { 0x00, 0x40, 0x14, 0x54 };
	if ( row > LCD_LINES )
		row = LCD_LINES - 1;    // we count rows starting w/0

	position = (row * 16) + column;

	writeByteToLCD(COMMAND_REGISTER,0x80 | (column + row_offsets[row]));
}

void waitForLCD(void)
{
	//	Poll the busy flag until it goes clear -- which means
	//	that new instructions may be given to the LCD screen
	//while((readByteFromLCD(COMMAND_REGISTER) & 0x80) == 0x80);
	
	//	Or.... delay 50us
	_delay_us(50);
}

void writeNibbleToLCD(unsigned char selectedRegister, unsigned char nibble)
{
	//	Pull the enable line high
	VARPIN_SET(EE);
	
	//	Output the nibble to the LCD
	if((nibble>>3) & 0x01)
	VARPIN_SET(D7);
	else
	VARPIN_CLR(D7);
	
	if((nibble>>2) & 0x01)
	VARPIN_SET(D6);
	else
	VARPIN_CLR(D6);
	
	if((nibble>>1) & 0x01)
	VARPIN_SET(D5);
	else
	VARPIN_CLR(D5);
	
	if(nibble & 0x01)
	VARPIN_SET(D4);
	else
	VARPIN_CLR(D4);
	
	//	Determine if the register select bit needs to be set
	if(selectedRegister == DATA_REGISTER)
	{
		//	If so, then set it
		VARPIN_SET(RS);
	}
	else
	{
		//	Otherwise, clear the register select bit
		VARPIN_CLR(RS);
	}
	//	Toggle the enable line to latch the nibble
	VARPIN_CLR(EE);
}

void writeByteToLCD(unsigned char selectedRegister, unsigned char byte)
{
	//	Generate the high and low part of the bytes that will be
	//	written to the LCD
	unsigned char upperNibble = (byte >> 4) & 0x0F;
	unsigned char lowerNibble = byte & 0x0F;
	
	//	Assuming a 2 line by 16 character display, ensure that
	//	everything goes where it is supposed to go:
	if(selectedRegister == DATA_REGISTER)
	{	
		if(position == 16)
		{
			//DISPLAY_SetCursor(0,1);
			_delay_ms(2);
		}else if(position == 32)
		{
			position = 0;
			//DISPLAY_SetCursor(0,0);
			_delay_ms(2);
		}
		
		position++;
	}
	
	//	Wait for the LCD to become ready
	waitForLCD();
	
	//	First, write the upper four bits to the LCD
	writeNibbleToLCD(selectedRegister, upperNibble);
	
	//	Finally, write the lower four bits to the LCD
	writeNibbleToLCD(selectedRegister, lowerNibble);
}
