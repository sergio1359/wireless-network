/**************************************************************************//**
  \file   halUid.c

  \brief  Implementation of UID interface.

  \author
      Atmel Corporation: http://www.atmel.com \n
      Support email: avr@atmel.com

    Copyright (c) 2008-2012, Atmel Corporation. All rights reserved.
    Licensed under Atmel's Limited License Agreement (BitCloudTM).

  \internal
    History:
      7/12/07 A. Khromykh - Created
 ******************************************************************************/
/******************************************************************************
 *   WARNING: CHANGING THIS FILE MAY AFFECT CORE FUNCTIONALITY OF THE STACK.  *
 *   EXPERT USERS SHOULD PROCEED WITH CAUTION.                                *
 ******************************************************************************/

/******************************************************************************
                   Includes section
******************************************************************************/
#include <gpio.h>
#include <uid.h>
#if defined(PLATFORM_STK600)
  #include <halTwi.h>
  #include <halW1.h>
#endif

/******************************************************************************
                   Define(s) section
******************************************************************************/
// twi eeprom(at24c02b) address on i2c bus
#define AT24_DEVICE_ADDRESS    0xA0
#define MAC_ADDRESS_AREA_SIZE     8
#define MAC_ADDRESS_POSITION      3
#define HAL_UID_I2C_PRESCALER     0
#define ACTION_WAITING_TIME       50

/******************************************************************************
                   Types section
******************************************************************************/
/** \brief uid type. */
typedef union
{
  uint64_t uid;
  uint8_t array[sizeof(uint64_t)];
} HalUid_t;

/******************************************************************************
                   Global variables section
******************************************************************************/
static HalUid_t halUid = {.uid = 0ull};

/******************************************************************************
                   Implementations section
******************************************************************************/
#if defined(PLATFORM_RCB)
/**************************************************************************//**
  \brief Writes/reads byte to/from SPI.
  \param[in] value - byte to write.
  \return  the byte which was read.
******************************************************************************/
static uint8_t halWriteByteSpi(uint8_t value)
{
  SPDR = value;
  asm("nop");   // This "nop" tunes up the "while" to reduce time for SPIF flag detecting.
  while (!(SPSR & (1 << SPIF)));
  return SPDR;
}

/**************************************************************************//**
Reads uid from external eeprom
******************************************************************************/
void halReadUid(void)
{
  uint8_t command = 0x03;
  uint8_t address = 0;
  uint8_t itr;

  GPIO_SPI_CS_set();
  GPIO_SPI_MISO_make_in();
  GPIO_SPI_MOSI_make_out();
  GPIO_SPI_SCK_make_out();
  GPIO_HW_SPI_CS_make_out();
  GPIO_SPI_CS_make_out();

  SPCR = ((1 << SPE) | (1 << MSTR));               // SPI enable, master mode.
  SPSR = (1 << SPI2X);                             // rate = fosc/2

  GPIO_SPI_CS_clr();

  halWriteByteSpi(command);
  halWriteByteSpi(address);
  for (itr = 0; itr < sizeof(uint64_t); itr++)
    halUid.array[itr] = halWriteByteSpi(address);

  GPIO_SPI_CS_set();
  // disable spi
  SPCR = 0;

  GPIO_HW_SPI_CS_make_in();
  GPIO_SPI_MOSI_make_in();
  GPIO_SPI_SCK_make_in();
}
#elif defined(PLATFORM_STK600)
/**************************************************************************//**
 Reads uid from external eeprom
******************************************************************************/
void halReadUid(void)
{
  uint8_t state;
  uint8_t itr;
  uint8_t waiting;

  TWCR = 0x00;
  TWSR = HAL_UID_I2C_PRESCALER; // prescaler
  // Set 250 Kb/s clock rate
  TWBR = (uint8_t)((F_CPU / 250000ul) - 16ul)/(2ul * (1ul << HAL_UID_I2C_PRESCALER) * (1ul << HAL_UID_I2C_PRESCALER));

  // disable twi interrupt
  TWCR &= (~(1 << TWIE));
  /////////////// start of dummy write  ////////////////////
  // send start condition
  TWCR = (1 << TWSTA) | (1 << TWINT) | (1 << TWEN);

  // wait for end of action
  waiting = ACTION_WAITING_TIME;
  while(!(TWCR & (1 << TWINT)) && waiting--)
    __delay_us(1);

  state = TWSR & 0xF8;
  if ((TWS_START != state) && (TWS_RSTART != state))
    return;

  // send at24 address + W
  TWDR =  AT24_DEVICE_ADDRESS;
  TWCR = (1 << TWINT) | (1 << TWEN);

  // wait for end of action
  waiting = ACTION_WAITING_TIME;
  while(!(TWCR & (1 << TWINT)) && waiting--)
    __delay_us(1);

  state = TWSR & 0xF8;
  if (TWS_MT_SLA_ACK != state)
    return;

  // send internal eeprom cell address
  for (itr = 0; itr < sizeof(uint16_t); ++itr)
  {
    // send address byte
    TWDR =  (uint8_t)((uint16_t)MAC_ADDRESS_POSITION >> (8 * itr));
    TWCR = (1 << TWINT) | (1 << TWEN);

    // wait for end of action
    while(!(TWCR & (1 << TWINT)));

    state = TWSR & 0xF8;
    if (TWS_MT_DATA_ACK != state)
      return;
  }
  /////////////// end of dummy write  ////////////////////
  /////////////// start of data read ////////////////////
  // send start condition
  TWCR = (1 << TWSTA) | (1 << TWINT) | (1 << TWEN);

  // wait for end of action
  while(!(TWCR & (1 << TWINT)));

  state = TWSR & 0xF8;
  if ((TWS_START != state) && (TWS_RSTART != state))
    return;

  // send at24 address + R
  TWDR =  AT24_DEVICE_ADDRESS | 0x01;
  TWCR = (1 << TWINT) | (1 << TWEN);

  // wait for end of action
  while(!(TWCR & (1 << TWINT)));

  state = TWSR & 0xF8;
  if (TWS_MR_SLA_ACK != state)
    return;

  // read MAC address
  for (itr = 0; itr < MAC_ADDRESS_AREA_SIZE; itr++)
  {
    if ((MAC_ADDRESS_AREA_SIZE - 1) == itr)
    { // send nack on last data byte
      TWCR &= ~(1 << TWEA);
      TWCR = (1 << TWINT) | (1 << TWEN);
    }
    else
    { // send ack
      TWCR = (1 << TWINT) | (1 << TWEN) | (1 << TWEA);
    }
    // wait for end of action
    while(!(TWCR & (1 << TWINT)));

    state = TWSR & 0xF8;
    if ((TWS_MR_DATA_ACK != state) && (TWS_MR_DATA_NACK != state))
      return;

    halUid.array[MAC_ADDRESS_AREA_SIZE - itr - 1] = TWDR;
  }
  /////////////// end of data read ////////////////////
  // send stop condition
  TWCR = (1 << TWSTO) | (1 << TWINT) | (1 << TWEN);
  // wait for end of stop condition
  while(!(TWCR & (1 << TWSTO)));
}
#else
/******************************************************************************
Reads uid from external eeprom
******************************************************************************/
void halReadUid(void)
{
}
#endif

/******************************************************************************
 Returns number which was read from external eeprom.
 Parameters:
   id - UID buffer pointer.
 Returns:
   0 - if unique ID has been found without error;
  -1 - if there are some erros during UID discovery.
******************************************************************************/
int HAL_ReadUid(uint64_t *id)
{
  if (!id)
    return -1;

  *id = halUid.uid;
  return 0;
}
// eof uid.c
