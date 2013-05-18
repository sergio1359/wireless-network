/*
* LCD_4BITS.c
*
* Created: 18/05/2013 17:28:21
*  Author: Victor
*/
#include "LCD_4BITS.h"


#include <util/delay.h>
#include "DIGITAL.h"

#define D0 _PB0
#define D1 _PB1
#define D2 _PB2
#define D3 _PB3
#define EE _PB4
#define RS _PB5
//#define RW _PB6 NOT CONNECTED

#define CCAT(a, b, c) a ## b ## c
#define SET(x) CCAT(HAL_GPIO, x, _set())
#define CLR(x) CCAT(HAL_GPIO, x, _clr())
#define OUT(x) CCAT(HAL_GPIO, x, _out())

#define	DATA_REGISTER		0
#define	COMMAND_REGISTER	1

unsigned char position;


/*
**	Function:		initializeLCD
**	Parameters:		<none>
**	Purpose:		Initializes the LCD into the following mode:
**					<some mode>
**	Returns:		<none>
*/
void initializeLCD(void)
{
	//	Set the position counter to 0
	position = 0;
	
	//	Set the proper data direction for output
	OUT(D0);
	OUT(D1);
	OUT(D2);
	OUT(D3);
	OUT(EE);
	OUT(RS);

	//	First, delay for at least 15ms after power on
	_delay_ms(15);
	
	//	Set for 4-bit LCD mode
	writeNibbleToLCD(COMMAND_REGISTER, 0x03);
	_delay_ms(5);
	
	writeNibbleToLCD(COMMAND_REGISTER, 0x03);
	_delay_ms(5);
	
	writeNibbleToLCD(COMMAND_REGISTER, 0x03);
	_delay_ms(5);
	
	writeNibbleToLCD(COMMAND_REGISTER, 0x02);
	_delay_ms(5);
	
	//	Function set
	writeByteToLCD(COMMAND_REGISTER, 0x28);
	_delay_ms(13);
	
	//	Turn display off
	writeByteToLCD(COMMAND_REGISTER, 0x08);
	_delay_ms(13);
	
	//	Clear LCD and return home
	writeByteToLCD(COMMAND_REGISTER, 0x01);
	_delay_ms(13);
	
	//	Turn on display, turn off cursor and blink
	writeByteToLCD(COMMAND_REGISTER, 0x0f);   // blinking cursor
	_delay_ms(13);
}


/*
**	Function:		waitForLCD
**	Parameters:		<none>
**	Purpose:		Polls the ready bit on the LCD to ensure that the
**					LCD is able to receive a new command or new data, or
**					delays for about 50 microseconds.
**	Returns:		<none>
*/
void waitForLCD(void)
{
	//	Poll the busy flag until it goes clear -- which means
	//	that new instructions may be given to the LCD screen
	//while((readByteFromLCD(COMMAND_REGISTER) & 0x80) == 0x80);
	
	//	Or.... delay 50us
	_delay_us(50);
}


/*
**	Function:		writeNibbleToLCD
**	Parameters:		selectedRegister - either the command or data register
**									   that will be written to
**					nibble - the four bits to be written to the LCD
**	Purpose:		Writes a nibble to the LCD.
**	Returns:		<none>
*/

void writeNibbleToLCD(unsigned char selectedRegister, unsigned char nibble)
{
	//DigitalOut_SetValue(RW,0);

	//	Pull the enable line high
	
	SET(EE);
	//	Output the nibble to the LCD
	if((nibble>>3) & 0x01)
		SET(D3);
	else
		CLR(D3);
		
	if((nibble>>2) & 0x01)
		SET(D2);
	else
		CLR(D2);
	
	if((nibble>>1) & 0x01)
		SET(D1);
	else
		CLR(D1);
	
	if(nibble & 0x01)
		SET(D0);
	else
		CLR(D0);
	
	//	Determine if the register select bit needs to be set
	if(selectedRegister == DATA_REGISTER)
	{
		//	If so, then set it
		SET(RS);
	}
	else
	{
		//	Otherwise, clear the register select bit
		CLR(RS);
	}
	
	//	Toggle the enable line to latch the nibble
	CLR(EE);
}

/*
**	Function:		writeByteToLCD
**	Parameters:		selectedRegister - either the command or data register
**									   that will be written to
**					byte - the eight bits to be written to the LCD
**	Purpose:		Writes and 8-bit value to the LCD screen.
**	Returns:		<none>
*/
void writeByteToLCD(unsigned char selectedRegister, unsigned char byte)
{
	//	Generate the high and low part of the bytes that will be
	//	written to the LCD
	unsigned char upperNibble = byte >> 4;
	unsigned char lowerNibble = byte & 0x0f;
	
	//	Assuming a 2 line by 16 character display, ensure that
	//	everything goes where it is supposed to go:
	if(selectedRegister == DATA_REGISTER && position == 16)
	{
		writeByteToLCD(COMMAND_REGISTER, 0xC0);
		//_delay_us(2);
		_delay_us(1250);
	}
	
	//	Wait for the LCD to become ready
	waitForLCD();
	
	//	First, write the upper four bits to the LCD
	writeNibbleToLCD(selectedRegister, upperNibble);
	
	//	Finally, write the lower four bits to the LCD
	writeNibbleToLCD(selectedRegister, lowerNibble);
	
	//	Reset the position count if it is equal to 80
	if(selectedRegister == DATA_REGISTER && ++position == 32)
	{
		writeByteToLCD(COMMAND_REGISTER, 0x80);
		//_delay_us(2);
		_delay_us(1250);
		position = 0;
	}
}

/*
**	Function:		writeStringToLCD
**	Parameters:		stringPointer - a pointer to the array of characters
**									that will be written to the LCD
**	Purpose:		Writes a series of bytes to the LCD data register so
**					that a character array may be given and displayed.
**	Returns:		<none>
*/
void writeStringToLCD(char* stringPointer)
{
	//	So long as the data that is pointed to by the string pointer
	//	is valid (ie, not the null pointer) then the data will be written
	//	to the LCD
	while (*stringPointer)
		writeByteToLCD(DATA_REGISTER, *stringPointer++);
}

/*
**	Function:		writeIntegerToLCD
**	Parameters:		integer - the integer that will be written to the LCD
**	Purpose:		Coverts a standard 16-bit int into the ASCII
**					representation of the number and writes that number
**					to the LCD.
**					*** Note:	The maximum value that can be written is
**								9999.  This is because there is no
**								ten thousands place support.
**	Returns		<none>
*/
void writeIntegerToLCD(int integer)
{
	//	Break down the original number into the thousands, hundreds, tens,
	//	and ones places and then immediately write that value to the LCD
	unsigned char thousands = integer / 1000;
	writeByteToLCD(DATA_REGISTER, thousands + 0x30);
	
	unsigned char hundreds = (integer - thousands*1000) / 100;
	writeByteToLCD(DATA_REGISTER, hundreds + 0x30);
	
	unsigned char tens = (integer - thousands*1000 - hundreds*100 ) / 10;
	writeByteToLCD(DATA_REGISTER, tens + 0x30);
	
	unsigned char ones = (integer - thousands*1000 - hundreds*100 - tens*10);
	writeByteToLCD(DATA_REGISTER, ones + 0x30);
}

/*
**	Function:		clearLCD
**	Parameters:		<none>
**	Purpose:		Writes the "clear and go home" command to the LCD.
**	Returns:		<none>
*/
void clearLCD(void)
{
	//	Send command to LCD (0x01)
	writeByteToLCD(COMMAND_REGISTER, 0x01);
	
	//	Set the current position to 0
	position = 0;
}

/*
**	Function:		moveToXY
**	Parameters:		row - the row to be moved to
**					column - the column to be moved to
**	Purpose:		Moves the cursor to the requested (row, column) position
**					on the LCD.  This function was originally written by
**					William Dubel (dubel@ufl.edu).
**	Returns:		<none>
*/
void moveToXY(unsigned char row, unsigned char column)
{
	//	Determine the new position
	position = (row * 16) + column;
	
	//	Send the correct commands to the command register of the LCD
	if(position < 16)
		writeByteToLCD(COMMAND_REGISTER, 0x80 | position);
	else
		writeByteToLCD(COMMAND_REGISTER, 0x80 | (position % 16 + 0x40));
}
