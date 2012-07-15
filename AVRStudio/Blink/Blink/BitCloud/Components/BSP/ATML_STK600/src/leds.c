/*************************************************************************//**
  \file leds.c

  \brief Leds control functionality implementation.

  \author
    Atmel Corporation: http://www.atmel.com \n
    Support email: avr@atmel.com

  Copyright (c) 2008-2012, Atmel Corporation. All rights reserved.
  Licensed under Atmel's Limited License Agreement (BitCloudTM).

  \internal
  History:
    29/05/07 E. Ivanov - Created
*****************************************************************************/
#if APP_DISABLE_BSP != 1

/******************************************************************************
                   Includes section
******************************************************************************/
#include <bspLeds.h>

/******************************************************************************
                   Implementations section
******************************************************************************/
/**************************************************************************//**
\brief Init LEDs module
******************************************************************************/
static void initLeds(void)
{
  halInitFirstLed();
  halInitSecondLed();
  halInitThirdLed();
#if defined(ATXMEGA128A1) || defined(ATXMEGA256A3) || defined(ATXMEGA256D3)
  halInitFourthLed();
  halInitFifthLed();
  halInitSixthLed();
  halInitSeventhLed();
  halInitEighthLed();
#endif
}

/**************************************************************************//**
\brief Open LEDs module

\return
    operation status
******************************************************************************/
result_t BSP_OpenLeds(void)
{
  initLeds();
  return BC_SUCCESS;
}

/**************************************************************************//**
\brief Closes leds module

\return
    operation state
******************************************************************************/
result_t BSP_CloseLeds(void)
{
  halUnInitFirstLed();
  halUnInitSecondLed();
  halUnInitThirdLed();
#if defined(ATXMEGA128A1) || defined(ATXMEGA256A3) || defined(ATXMEGA256D3)
  halUnInitFourthLed();
  halUnInitFifthLed();
  halUnInitSixthLed();
  halUnInitSeventhLed();
  halUnInitEighthLed();
#endif
  return BC_SUCCESS;
}

/**************************************************************************//**
\brief Turn on a LED

\param[in]
    id - LED index
******************************************************************************/
void BSP_OnLed(uint8_t id)
{
  switch (id)
  {
    case LED_FIRST:
      halOnFirstLed();
      break;
    case LED_SECOND:
      halOnSecondLed();
      break;
    case LED_THIRD:
      halOnThirdLed();
      break;
#if defined(ATXMEGA128A1) || defined(ATXMEGA256A3) || defined(ATXMEGA256D3)
    case LED_FOURTH:
      halOnFourthLed();
      break;
    case LED_FIFTH:
      halOnFifthLed();
      break;
    case LED_SIXTH:
      halOnSixthLed();
      break;
    case LED_SEVENTH:
      halOnSeventhLed();
      break;
    case LED_EIGHTH:
      halOnEighthLed();
      break;
#endif
  }
}

/**************************************************************************//**
\brief Turn off a LED

\param[in]
      id - LED index
******************************************************************************/
void BSP_OffLed(uint8_t id)
{
  switch (id)
  {
    case LED_FIRST:
      halOffFirstLed();
      break;
    case LED_SECOND:
      halOffSecondLed();
      break;
    case LED_THIRD:
      halOffThirdLed();
      break;
#if defined(ATXMEGA128A1) || defined(ATXMEGA256A3) || defined(ATXMEGA256D3)
    case LED_FOURTH:
      halOffFourthLed();
      break;
    case LED_FIFTH:
      halOffFifthLed();
      break;
    case LED_SIXTH:
      halOffSixthLed();
      break;
    case LED_SEVENTH:
      halOffSeventhLed();
      break;
    case LED_EIGHTH:
      halOffEighthLed();
      break;
#endif
  }
}

/**************************************************************************//**
\brief Change state of a LED

\param[in]
      id - LED index
******************************************************************************/
void BSP_ToggleLed(uint8_t id)
{
  switch (id)
  {
    case LED_FIRST:
      halToggleFirstLed();
      break;
    case LED_SECOND:
      halToggleSecondLed();
      break;
    case LED_THIRD:
      halToggleThirdLed();
      break;
#if defined(ATXMEGA128A1) || defined(ATXMEGA256A3) || defined(ATXMEGA256D3)
    case LED_FOURTH:
      halToggleFourthLed();
      break;
    case LED_FIFTH:
      halToggleFifthLed();
      break;
    case LED_SIXTH:
      halToggleSixthLed();
      break;
    case LED_SEVENTH:
      halToggleSeventhLed();
      break;
    case LED_EIGHTH:
      halToggleEighthLed();
      break;
#endif
  }
}

#endif // APP_DISABLE_BSP != 1

// eof leds.c
