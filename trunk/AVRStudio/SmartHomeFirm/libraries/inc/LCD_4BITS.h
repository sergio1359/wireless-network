/*
 * LCD_4BITS.h
 *
 * Created: 18/05/2013 17:28:33
 *  Author: Victor
 *
 *	Purpose:	Interface a Hitachi-compatible LCD screen to an 8-bit
 *				microcontroller making use of 7 bits of any port.  Timer
 *				delay functions are used as the ready flag polling
 *				is not needed in some applications.
 *
 *	Notes:		(1)	The LCD pinout is as follows:
 *
 *					01 - Ground (Vss)
 *					02 - 5V Power (Vcc)
 *					03 - Contrast (Vo)
 *					04 - Register Select (RS)
 *					05 - Read/Write_L (R/W_L)
 *					06 - Enable (E)
 *					07 - Data 0 (DB0)
 *					08 - Data 1 (DB1)
 *					09 - Data 2 (DB2)
 *					10 - Data 3 (DB3)
 *					11 - Data 4 (DB4)
 *					12 - Data 5 (DB5)
 *					13 - Data 6 (DB6)
 *					14 - Data 7 (DB7)
 *					15 - Backlight 5V Power (use 10 ohm resistor)
 *					16 - Ground (Vss)
 *
 *				(2)	The port pinout is considered to be the following:
 *
 *					P7 - No connect (NC)
 *					P6 - Enable (E)
 *					P5 - Read/Write_L (R/W_L)
 *					P4 - Register Select (RS)
 *					P3 - Data 7 (DB7)
 *					P2 - Data 6 (DB6)
 *					P1 - Data 5 (DB5)
 *					P0 - Data 4 (DB4)
 */


#ifndef LCD_4BITS_H_
#define LCD_4BITS_H_

/*
**	Function Declarations
*/

/*
**	Function:		initializeLCD
**	Parameters:		<none>
**	Purpose:		Initializes the LCD into the following mode:
**					<some mode>
**	Returns:		<none>
*/
void initializeLCD(void);

/*
**	Function:		waitForLCD
**	Parameters:		<none>
**	Purpose:		Polls the ready bit on the LCD to ensure that the
**					LCD is able to receive a new command or new data, or
**					delays for about 50 microseconds.
**	Returns:		<none>
*/
void waitForLCD(void);

/*
**	Function:		writeCharacterToLCD
**	Parameters:		character - unsigned character to be written to the LCD
**	Purpose:		Writes an 8-bit unsigned character to the LCD screen.
**	Returns:		<none>
*/
void writeCharacterToLCD(unsigned character);

/*
**	Function:		writeNibbleToLCD
**	Parameters:		selectedRegister - either the command or data register
**									   that will be written to
**					nibble - the four bits to be written to the LCD
**	Purpose:		Writes a nibble to the LCD.
**	Returns:		<none>
*/
void writeNibbleToLCD(unsigned char selectedRegister, unsigned char nibble);

/*
**	Function:		writeByteToLCD
**	Parameters:		selectedRegister - either the command or data register
**									   that will be written to
**					byte - the eight bits to be written to the LCD
**	Purpose:		Writes and 8-bit value to the LCD screen.
**	Returns:		<none>
*/
void writeByteToLCD(unsigned char selectedRegister, unsigned char byte);

/*
**	Function:		writeStringToLCD
**	Parameters:		stringPointer - a pointer to the array of characters
**									that will be written to the LCD
**	Purpose:		Writes a series of bytes to the LCD data register so
**					that a character array may be given and displayed.
**	Returns:		<none>
*/
void writeStringToLCD(char* stringPointer);

void writeLineToLCD(char* stringPointer, char * DbgLine);

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
void writeIntegerToLCD(int integer);

/*
**	Function:		clearLCD
**	Parameters:		<none>
**	Purpose:		Writes the "clear and go home" command to the LCD.
**	Returns:		<none>
*/
void clearLCD(void);

/*
**	Function:		moveToXY
**	Parameters:		row - the row to be moved to
**					column - the column to be moved to
**	Purpose:		Moves the cursor to the requested (row, column) position
**					on the LCD
**	Returns:		<none>
*/
void moveToXY(unsigned char row, unsigned char column);



#endif /* LCD_4BITS_H_ */