/**************************************************************************//**
  \file  halInit.c

  \brief HAL start up module.

  \author
      Atmel Corporation: http://www.atmel.com \n
      Support email: avr@atmel.com

    Copyright (c) 2008-2012, Atmel Corporation. All rights reserved.
    Licensed under Atmel's Limited License Agreement (BitCloudTM).

  \internal
    History:
      29/06/07 E. Ivanov - Created
 ******************************************************************************/
/******************************************************************************
 *   WARNING: CHANGING THIS FILE MAY AFFECT CORE FUNCTIONALITY OF THE STACK.  *
 *   EXPERT USERS SHOULD PROCEED WITH CAUTION.                                *
 ******************************************************************************/

/******************************************************************************
                   Includes section
******************************************************************************/
#include <halAppClock.h>
#include <halSleepTimerClock.h>
#include <halIrq.h>
#include <halInterrupt.h>

/******************************************************************************
                   Prototypes section
******************************************************************************/
/******************************************************************************
 Reads uid from external devices.
******************************************************************************/
void halReadUid(void);

/******************************************************************************
                   Implementations section
******************************************************************************/
/******************************************************************************
Performs start up HAL initialization.
Parameters:
  none.
Returns:
  none.
******************************************************************************/
void HAL_Init(void)
{
   /* Init first diagnostic timer */
#ifdef MEASURE
  TCCR5B = (1 << CS50);
#endif

  /* Enable SRAM Data Retention */
  DRTRAM0 |= (1<<ENDRT);
  DRTRAM1 |= (1<<ENDRT);
  DRTRAM2 |= (1<<ENDRT);
  DRTRAM3 |= (1<<ENDRT);

  /* start sleep time */
  halStartSleepTimerClock();
  /* initialization work frequency & start calibration */
  halInitFreq();
  /* Reads unique ID */
  halReadUid();
  /* initialization and start application timer */
  halInitAppClock();
#ifdef HW_CONTROL_PINS_PORT_ASSIGNMENT
  /* initialization dtr interrupt */
  halSetIrqConfig(IRQ_4, IRQ_LOW_LEVEL);
#endif // HW_CONTROL_PINS_PORT_ASSIGNMENT
  /* global enable interrupt*/
  HAL_EnableInterrupts();
}
// eof halInit.c
