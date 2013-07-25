/*
 * LCD_4BITS.h
 *
 * Created: 18/05/2013 17:28:33
 *  Author: Victor
 *
 *	Purpose:	Interface a Hitachi-compatible LCD screen to an 8-bit
 *				microcontroller
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
 */


#ifndef DISPLAY_H_
#define DISPLAY_H_

#include <stdint.h>

/*
**	Function Declarations
*/

void DISPLAY_Init(uint16_t EE_PinAddress, uint16_t RS_PinAddress, uint16_t D4_PinAddress, uint16_t D5_PinAddress, uint16_t D6_PinAddress, uint16_t D7_PinAddress);

void DISPLAY_WriteByte(char byte);

void DISPLAY_WriteString(char* stringPointer);

void DISPLAY_WriteNumberDEC(int integer, unsigned char digits);

void DISPLAY_WriteNumberHEX(unsigned int num);

void DISPLAY_Clear(void);

void DISPLAY_SetCursor(unsigned char column, unsigned char row);

#endif /* DISPLAY_H_ */