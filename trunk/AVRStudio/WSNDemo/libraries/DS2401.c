/*
 * _DS2401.c
 *
 * Created: 10/04/2013 13:57:23
 *  Author: Victor
 *
 * Description: This file contains routines to work with the DS2401 Electronic Serial Number 
 */ 
#include "inc/DS2401.h"

uint8_t Initialise_DS2401(void) 
{ 
// This is a routine to test for a presence signal.  It actually returns a byte which represents a count of how long the pulse is in 5us sections.
// I'm not sure what the length of the presence pulse might indicate but I think it should be a minimum number. 
    uint8_t count = 0; 
   uint8_t initialvalue = 0; 
   uint8_t i = 0; 
    
   DDRD &= ~(1 << 5); // Clear bit 5 in DDRD pin 5 to make it an input 
   asm("nop"); asm("nop"); asm("nop"); 
   // _delay_us(1); 
   DDRD |= (1 << 5); // Make bit 5 an output - this will send it low because PORTD pin5 is already cleared to 0 
    _delay_us(520); // The minimum for the DS2401 is 480us 
   DDRD &= ~(1 << 5); // Clear bit 5 in DDRD pin 5 to make it an input 
   _delay_us(5); // tpdh minimum is 15us so start sampling before the pulse is driven low by the DS2401 
   for(i=0;i<96;i++) // 96 counts of 5us intervals is 480us
   { 
      initialvalue = (PIND & (1 << 5)); // If high the value returned will be 0010 0000 (20h) 
      if (initialvalue == 0x20) 
         count++; 
      _delay_us(5); 
   } 
   return(count); // If there is a DS2401 present the maximum count must be 84 for a 96 x 5us loop count. 
} 

uint8_t Check_Line_High(void) 
{ 
// This is a routine to check that the control line for the DS2401 is high ie. the DS2401 is not pulling it low 
   uint8_t checkvalue = 0; 
    
   DDRD &= ~(1 << 5); // Clear bit 5 in DDRD pin 5 to make it an input 
   asm("nop"); asm("nop"); asm("nop"); 
   // _delay_us(1); 
   // Below is the routine to read the pin ... 
   checkvalue = (PIND & (1 << 5)); // If high the value returned will be 0010 0000 (20h) 
   checkvalue = (checkvalue << 2); // If the value returned is '1' number sent will be 1000 0000 (80h) 
   _delay_us(1); 
   return(checkvalue); 
} 

void Write0_DS2401(void) 
{ 
// This is a routine to write a 0 to the DS2401. 
// The routine has a total cycle time of approx 80us. 
    
   DDRD &= ~(1 << 5); // Clear bit 5 in DDRD pin 5 to make it an input 
   asm("nop"); asm("nop"); asm("nop"); 
   // _delay_us(1); 
   DDRD |= (1 << 5); // Make bit 5 an output - this will send it low because PORTD pin5 is already cleared to 0 
   _delay_us(70); // The minimum for the DS2401 is 60us 
   DDRD &= ~(1 << 5); // Clear bit 5 in DDRD pin 5 to make it an input ... this will tri-state the pin allowing the pullup to pull the line high 
   _delay_us(10); 
} 

void Write1_DS2401(void) 
{ 
// This is a routine to write a 1 to the DS2401. 
// The routine has a total cycle time of approx 80us. 
    
   DDRD &= ~(1 << 5); // Clear bit 5 in DDRD pin 5 to make it an input 
   asm("nop"); asm("nop"); asm("nop"); 
   // _delay_us(1); 
   DDRD |= (1 << 5); // Make bit 5 an output - this will send it low because PORTD pin5 is already cleared to 0 
   _delay_us(10); // The minimum for the DS2401 is 1us with a maximum of 15us 
   DDRD &= ~(1 << 5); // Clear bit 5 in DDRD pin 5 to make it an input ... this will tri-state the pin allowing the pullup to pull the line high 
   _delay_us(70); 
} 

/* 
bool Read_bit_DS2401(void) 
{ 
// This is a routine to read a bit from the DS2401. 
// The routine has a total cycle time of approx 80us. 

   uint8_t bitvalue = 0; 
   bool boolvalue = 0; 
   uint8_t i = 0; 
    
   DDRD &= ~(1 << 5); // Clear bit 5 in DDRD pin 5 to make it an input 
   asm("nop"); asm("nop"); asm("nop"); 
   //_delay_us(1); 
   DDRD |= (1 << 5); // Make bit 5 an output - this will send it low because PORTD pin5 is already cleared to 0 
   for (i=0;i<16;i++) 
      asm("nop"); // This delay is 16 x 54ns - roughly 865ns. The DS2401 needs a maximum of 1us. 
   DDRD &= ~(1 << 5); // Clear bit 5 in DDRD pin 5 to make it an input ... this will tri-state the pin allowing the pullup to pull the line high 
   _delay_us(15); // The nominal time (trdv) for valid data on the DS2401 is 15us 
   // Below is the routine to read the pin ... 
   bitvalue = (PIND & (1 << 5)); // If high the value returned will be 0010 0000 (20h) 
//   bitvalue = (bitvalue << 2); // If the bit returned is '1' value will be 10000000 (08h) (otherwise 0) 
   _delay_us(65); // trelease is 45us 
   return((bitvalue == 0x20)?1:0); 
} 

uint8_t Read_Family_Code_DS2401(void) 
{ 
// This routine returns a byte from the DS2401 ESN.  It needs to be called 8 times in succession to read the whole DS2410 ROM. 
   uint8_t i = 0; 
   uint8_t bytevalue = 0; 
   uint8_t bitvalue = 0; 
    
   for (i=0;i<7;i++) 
   { 
      //bitvalue = Read_bit_DS2401(); 
       
      if (Read_bit_DS2401() == 0x80) 
      { 
         bytevalue = (bytevalue | 0x80); 
      } 
      bytevalue = (bytevalue >> 1); 
   } 
   // _delay_us(10); 
   return(bytevalue); 
}    
*/ 



uint8_t Read_bit_DS2401(void) 
{ 
// This is a routine to read from the DS2401. 
// The routine has a total cycle time of approx 80us. 

   uint8_t bitvalue = 0; 
   uint8_t i = 0; 
    
   DDRD &= ~(1 << 5); // Clear bit 5 in DDRD pin 5 to make it an input 
   asm("nop"); asm("nop"); asm("nop");
   //_delay_us(1); 
   DDRD |= (1 << 5); // Make bit 5 an output - this will send it low because PORTD pin5 is already cleared to 0 
   for (i=0;i<16;i++) 
      asm("nop"); // This delay is 16 x 54ns - roughly 865ns. The DS2401 needs a maximum of 1us. 
   DDRD &= ~(1 << 5); // Clear bit 5 in DDRD pin 5 to make it an input ... this will tri-state the pin allowing the pullup to pull the line high 
   _delay_us(20); // The nominal time (trdv) for valid data on the DS2401 is 15us 
   // Below is the routine to read the pin ... 
   bitvalue = (PIND & (1 << 5)); // If high the value returned will be 0010 0000 (20h) 
   bitvalue = (bitvalue << 2); // If the bit returned is '1' value will be 10000000 (08h) (otherwise 0) 
   _delay_us(60); // trelease is 45us 
   return(bitvalue); 
} 


uint8_t Read_Byte_DS2401(void) 
{ 
// This is a routine to read from the DS2401. 
// The routine has a total cycle time of approx 70us. 

   uint8_t bitvalue, bytevalue = 0; 
   uint8_t i, j = 0; 
    
   for (j=0;j<7;j++) 
   { 
      DDRD &= ~(1 << 5); // Clear bit 5 in DDRD pin 5 to make it an input 
      asm("nop"); asm("nop"); asm("nop"); 
      //_delay_us(1); 
      DDRD |= (1 << 5); // Make bit 5 an output - this will send it low because PORTD pin5 is already cleared to 0 
      for (i=0;i<16;i++) 
         asm("nop"); // This delay is 12 x 54ns - roughly 650ns. The DS2401 needs a maximum of 1us. 
      DDRD &= ~(1 << 5); // Clear bit 5 in DDRD pin 5 to make it an input ... this will tri-state the pin allowing the pullup to pull the line high 
      _delay_us(15); // The nominal time (trdv) for valid data on the DS2401 is 15us 
      // Below is the routine to read the pin ... 
      bitvalue = (PIND & (1 << 5)); // If high the value returned will be 0010 0000 (20h) 
      bitvalue = (bitvalue << 2); // If the bit returned is '1' value will be 10000000 (80h) (otherwise 0) 
      _delay_us(45); // trelease is 45us 
      if (bitvalue == 0x80) 
      { 
         bytevalue = (bytevalue | 0x80); 
      } 
      bytevalue = (bytevalue >> 1); 
   } 
   return(bytevalue); 
} 


/* 
uint8_t Read_Byte_DS2401(void) 
{ 
// This routine returns a byte from the DS2401 ESN.  It needs to be called 8 times in succession to read the whole DS2410 ROM. 
   uint8_t i = 0; 
   uint8_t bytevalue = 0; 
   uint8_t bitvalue = 0; 
    
   for (i=0;i<7;i++) 
   { 
      //bitvalue = Read_bit_DS2401(); 
       
      if (Read_bit_DS2401() == 0x80) 
      { 
         bytevalue = (bytevalue | 0x80); 
      } 
      bytevalue = (bytevalue >> 1); 
   } 
   // _delay_us(10); 
   return(bytevalue); 
}    
*/ 

uint8_t Find_ROM_DS2401(void) 
{ 
/* This function firstly initialises the ROM and checks for its presence then writes the command (33H) to the chip before reading the full 64 bits of information held in the chip.
   This 64 bits consists of a byte of 'family code', 6 bytes of ID and lastly a CRC byte. 
*/ 
    uint8_t loopcount = 0; 
   uint8_t presence = 0; 
   uint8_t readromcommand = 0x33; 
   presence = Initialise_DS2401(); 
   if (presence > 83) return(presence); //If there is no chip the value returned for 'presence' will be 96 hopefully
   // The following loop writes the Read Rom command to the chip 
   while (loopcount < 8) 
   { 
      if ((readromcommand & 0x01) == 1) 
      { 
         Write1_DS2401(); 
      } 
      else 
      { 
         Write0_DS2401(); 
      }; 
      readromcommand = (readromcommand >> 1); 
      loopcount++; 
   }; 
   _delay_us(10); 
   return(presence); 
} 
