/*
 * NokiaLCD.cpp
 *
 *  Created on: 02/06/2012
 *      Author: Victor
 */

#include "nokiaLCD.h"

void LcdCharacter(char character)
{
  LcdWrite(LCD_D, 0x00);
  for (int index = 0; index < 5; index++)
  {
    LcdWrite(LCD_D, ASCII[character - 0x20][index]);
  }
  LcdWrite(LCD_D, 0x00);
}

void LcdClear(void)
{
  for (int index = 0; index < LCD_X * LCD_Y / 8; index++)
  {
    LcdWrite(LCD_D, 0x00);
  }
}

void LcdInitialise(void)
{
	PIN_MODE(PIN_SCE,   OUTPUT);
	PIN_MODE(PIN_RESET, OUTPUT);
	PIN_MODE(PIN_DC,    OUTPUT);
	PIN_MODE(PIN_SDIN,  OUTPUT);
	PIN_MODE(PIN_SCLK,  OUTPUT);

  DIGITAL_WRITE(PIN_RESET, LOW);
 // delay(1);
  DIGITAL_WRITE(PIN_RESET, HIGH);

  LcdWrite( LCD_CMD, 0x21 );  // LCD Extended Commands.
  LcdWrite( LCD_CMD, 0xBf );  // Set LCD Vop (Contrast). //B1
  LcdWrite( LCD_CMD, 0x04 );  // Set Temp coefficent. //0x04
  LcdWrite( LCD_CMD, 0x14 );  // LCD bias mode 1:48. //0x13
  LcdWrite( LCD_CMD, 0x0C );  // LCD in normal mode. 0x0d for inverse
  LcdWrite(LCD_C, 0x20);
  LcdWrite(LCD_C, 0x0C);
}

void LcdString(char *characters)
{
  while (*characters)
  {
    LcdCharacter(*characters++);
  }
}

void LcdWrite(byte dc, byte data)
{
  if(dc)
	DIGITAL_WRITE(PIN_DC, HIGH);
  else
	DIGITAL_WRITE(PIN_DC, LOW);

  DIGITAL_WRITE(PIN_SCE, LOW);
  shiftOut(PIN_SDIN, PIN_SCLK, MSBFIRST, data);
  DIGITAL_WRITE(PIN_SCE, HIGH);
}

// gotoXY routine to position cursor
// x - range: 0 to 84
// y - range: 0 to 5

void LcdGoToXY(int x, int y)
{
  LcdWrite( 0, 0x80 | x);  // Column.
  LcdWrite( 0, 0x40 | y);  // Row.

}



void LcdDrawLine(void)
{
  unsigned char  j;
   for(j=0; j<84; j++) // top
	{
	   LcdGoToXY (j,0);
	  LcdWrite (1,0x01);
  }
  for(j=0; j<84; j++) //Bottom
	{
	  LcdGoToXY (j,5);
	  LcdWrite (1,0x80);
  }

  for(j=0; j<6; j++) // Right
	{
	  LcdGoToXY (83,j);
	  LcdWrite (1,0xff);
  }
	for(j=0; j<6; j++) // Left
	{
		LcdGoToXY (0,j);
	  LcdWrite (1,0xff);
  }

}


