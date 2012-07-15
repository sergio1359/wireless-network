/**************************************************************************//**
  \file  halSleep.c

  \brief Implementation of sleep modes.

  \author
      Atmel Corporation: http://www.atmel.com \n
      Support email: avr@atmel.com

    Copyright (c) 2008-2012, Atmel Corporation. All rights reserved.
    Licensed under Atmel's Limited License Agreement (BitCloudTM).

  \internal
    History:
      1/12/09 A. Khromykh - Created
 ******************************************************************************/
/******************************************************************************
 *   WARNING: CHANGING THIS FILE MAY AFFECT CORE FUNCTIONALITY OF THE STACK.  *
 *   EXPERT USERS SHOULD PROCEED WITH CAUTION.                                *
 ******************************************************************************/

/******************************************************************************
                   Includes section
******************************************************************************/
#include <sleepTimer.h>
#include <halSleepTimerClock.h>
#include <halSleep.h>
#include <halIrq.h>
#include <halDbg.h>
#include <halAppClock.h>
#include <halEeprom.h>

/******************************************************************************
                   Defines section
******************************************************************************/
#define RF_TRX_STATE_MASK                 0x1f
#define RF_TRX_OFF_STATE                  0x08

/******************************************************************************
                   Prototypes section
******************************************************************************/
/**************************************************************************//**
\brief Performs calibration of the main clock generator.
******************************************************************************/
void halStartingCalibrate(void);

#ifdef _HAL_RF_RX_TX_INDICATOR_

/**************************************************************************//**
\brief  Turn on pin 1 (DIG3) and pin 2 (DIG4) to indicate the transmit state of
the radio transceiver.
******************************************************************************/
void phyRxTxSwitcherOn(void);

/**************************************************************************//**
\brief  Turn off pin 1 (DIG3) and pin 2 (DIG4) to indicate the transmit state of
the radio transceiver.
******************************************************************************/
void phyRxTxSwitcherOff(void);

#endif

#ifdef _HAL_ANT_DIVERSITY_
/**************************************************************************//**
\brief  Turn on pin 9 (DIG1) and pin 10 (DIG2) to enable antenna select.
******************************************************************************/
void phyAntennaSwitcherOn(void);

/**************************************************************************//**
\brief  Turn off pin 9 (DIG1) and pin 10 (DIG2) to disable antenna select.
******************************************************************************/
void phyAntennaSwitcherOff(void);

#endif // _HAL_ANT_DIVERSITY_

/******************************************************************************
                   External global variables section
******************************************************************************/
extern HalSleepControl_t halSleepControl;

/******************************************************************************
                   Global variables section
******************************************************************************/
static uint32_t halTimeStartOfSleep = 0ul;

/******************************************************************************
                   Implementations section
******************************************************************************/
/**************************************************************************//**
\brief Switch on system power.

\param[in]
  wakeupSource - wake up source
******************************************************************************/
void halPowerOn(const uint8_t wakeupSource)
{
  halSleepControl.wakeupStation = HAL_ACTIVE_MODE;
  halSleepControl.wakeupSource = wakeupSource;

  TRXPR &= ~(1 << SLPTR);

  halPostTask4(HAL_WAKEUP);
}

/******************************************************************************
Shutdowns system.
  NOTES:
  the application should be sure the poweroff will not be
  interrupted after the execution of the sleep().
******************************************************************************/
void halPowerOff(void)
{
  if (HAL_ACTIVE_MODE == halSleepControl.wakeupStation)
    return;  // it is a too late to sleep.

  // stop application timer clock
  halStopAppClock(); // will be shutdown
  if (0ul == halTimeStartOfSleep)
  { // start of sleep procedure
    // save time of stopping of the application timer
    halTimeStartOfSleep = halGetTimeOfSleepTimer();
  }

  if (HAL_SLEEP_STATE_CONTINUE_SLEEP != halSleepControl.sleepState)
  {
#ifdef _HAL_RF_RX_TX_INDICATOR_
    // disable front end driver if that is supported
    phyRxTxSwitcherOff();
#endif

#ifdef _HAL_ANT_DIVERSITY_
    // disable antenna diversity switcher
    phyAntennaSwitcherOff();
#endif //_HAL_ANT_DIVERSITY_
  }

  // wait for end of eeprom writing
  halWaitEepromReady();

  halSleepControl.sleepState = HAL_SLEEP_STATE_CONTINUE_SLEEP;

  TRXPR |= (1 << SLPTR);

  if (HAL_SLEEP_TIMER_IS_STARTED == halSleepControl.sleepTimerState)
  { // sleep timer is started
    SMCR = (1 << SM1) | (1 << SM0) | (1 << SE); // power-save
    __SLEEP;
    SMCR = 0;
  }
  else
  {
    halStopSleepTimerClock();
    SMCR = (1 << SM1) | (1 << SE); // power-down
    __SLEEP;
    SMCR = 0;
    halStartSleepTimerClock();
    halStartingCalibrate();
  }

  // wait for time about 1 TOSC1 cycle for correct re-entering from power save mode to power save mode
  // wait for time about 1 TOSC1 cycle for correct reading TCNT2 after wake up to
  OCR2B = SOME_VALUE_FOR_SYNCHRONIZATION;
  while (ASSR & HAL_ASSR_FLAGS);
}

/******************************************************************************
  Prepares system for power-save, power-down.
  Power-down the mode is possible only when internal RC is used
  Parameters:
  none.
  Returns:
  -1 there is no possibility to power-down system.
******************************************************************************/
int HAL_Sleep(void)
{
  halSleepControl.wakeupStation = HAL_SLEEP_MODE;  // the reset of sign of entry to the sleep mode.
  while (ASSR & HAL_ASSR_FLAGS);
  halPostTask4(HAL_SLEEP);
  return 0;
}

/******************************************************************************
 Handler for task manager. It is executed when system has waked up.
******************************************************************************/
void halWakeupHandler(void)
{
  uint32_t timeEndOfSleep;

  // save time of stopping of the application timer
  timeEndOfSleep = halGetTimeOfSleepTimer();

  timeEndOfSleep -= halTimeStartOfSleep;  // time of sleep
  halTimeStartOfSleep = 0ul;
  // adjust application timer interval
  halAdjustSleepInterval(timeEndOfSleep);

  // start application timer clock
  halStartAppClock();

  // Wait for radio to wake up.
  while (RF_TRX_OFF_STATE != (TRX_STATUS & RF_TRX_STATE_MASK));

  halSleepControl.sleepState = HAL_SLEEP_STATE_BEGIN;

#ifdef _HAL_ANT_DIVERSITY_
  // enable antenna diversity switcher
  phyAntennaSwitcherOn();
#endif

#ifdef _HAL_RF_RX_TX_INDICATOR_
  // enable front end driver if that is supported
  phyRxTxSwitcherOn();
#endif

  if (HAL_SLEEP_TIMER_IS_WAKEUP_SOURCE == halSleepControl.wakeupSource)
  {
    if (halSleepControl.callback)
      halSleepControl.callback();
  }
}

/*******************************************************************************
  Makes MCU enter Idle mode.
*******************************************************************************/
void HAL_IdleMode(void)
{
  SMCR = 0x1;
  __SLEEP;
  SMCR = 0;
}

// eof sleep.c
