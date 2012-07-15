/*************************************************************************//**
  \file bspLeds.h

  \brief Declaration of leds defines.

  \author
    Atmel Corporation: http://www.atmel.com \n
    Support email: avr@atmel.com

  Copyright (c) 2008-2012, Atmel Corporation. All rights reserved.
  Licensed under Atmel's Limited License Agreement (BitCloudTM).

  \internal
  History:
    29/05/07 E. Ivanov - Created
*****************************************************************************/

#ifndef _BSPLEDS_H
#define _BSPLEDS_H

/******************************************************************************
                   Includes section
******************************************************************************/
// \cond
#include <gpio.h>
#include <leds.h>
// \endcond

/******************************************************************************
                   Define(s) section
******************************************************************************/
#if defined(ATXMEGA128A1) || defined(ATXMEGA256A3) || defined(ATXMEGA256D3)
  #define halInitFirstLed()       GPIO_E0_make_out(); GPIO_E0_set()
  #define halUnInitFirstLed()     GPIO_E0_make_in()
  #define halOnFirstLed()         GPIO_E0_clr()
  #define halOffFirstLed()        GPIO_E0_set()
  #define halReadFirstLed()       GPIO_E0_read()
  #define halToggleFirstLed()     GPIO_E0_toggle()

  #define halInitSecondLed()      GPIO_E1_make_out(); GPIO_E1_set()
  #define halUnInitSecondLed()    GPIO_E1_make_in()
  #define halOnSecondLed()        GPIO_E1_clr()
  #define halOffSecondLed()       GPIO_E1_set()
  #define halReadSecondLed()      GPIO_E1_read()
  #define halToggleSecondLed()    GPIO_E1_toggle()

  #define halInitThirdLed()       GPIO_E2_make_out(); GPIO_E2_set()
  #define halUnInitThirdLed()     GPIO_E2_make_in()
  #define halOnThirdLed()         GPIO_E2_clr()
  #define halOffThirdLed()        GPIO_E2_set()
  #define halReadThirdLed()       GPIO_E2_read()
  #define halToggleThirdLed()     GPIO_E2_toggle()

  #define halInitFourthLed()      GPIO_E3_make_out(); GPIO_E3_set()
  #define halUnInitFourthLed()    GPIO_E3_make_in()
  #define halOnFourthLed()        GPIO_E3_clr()
  #define halOffFourthLed()       GPIO_E3_set()
  #define halReadFourthLed()      GPIO_E3_read()
  #define halToggleFourthLed()    GPIO_E3_toggle()

  #define halInitFifthLed()       GPIO_E4_make_out(); GPIO_E4_set()
  #define halUnInitFifthLed()     GPIO_E4_make_in()
  #define halOnFifthLed()         GPIO_E4_clr()
  #define halOffFifthLed()        GPIO_E4_set()
  #define halReadFifthLed()       GPIO_E4_read()
  #define halToggleFifthLed()     GPIO_E4_toggle()

  #define halInitSixthLed()       GPIO_E5_make_out(); GPIO_E5_set()
  #define halUnInitSixthLed()     GPIO_E5_make_in()
  #define halOnSixthLed()         GPIO_E5_clr()
  #define halOffSixthLed()        GPIO_E5_set()
  #define halReadSixthLed()       GPIO_E5_read()
  #define halToggleSixthLed()     GPIO_E5_toggle()

  #define halInitSeventhLed()     GPIO_E6_make_out(); GPIO_E6_set()
  #define halUnInitSeventhLed()   GPIO_E6_make_in()
  #define halOnSeventhLed()       GPIO_E6_clr()
  #define halOffSeventhLed()      GPIO_E6_set()
  #define halReadSeventhLed()     GPIO_E6_read()
  #define halToggleSeventhLed()   GPIO_E6_toggle()

  #define halInitEighthLed()      GPIO_E7_make_out(); GPIO_E7_set()
  #define halUnInitEighthLed()    GPIO_E7_make_in()
  #define halOnEighthLed()        GPIO_E7_clr()
  #define halOffEighthLed()       GPIO_E7_set()
  #define halReadEighthLed()      GPIO_E7_read()
  #define halToggleEighthLed()    GPIO_E7_toggle()
#elif defined(ATMEGA128RFA1)
  #if (APP_USE_OTAU == 1)
    #define halInitFirstLed()
    #define halUnInitFirstLed()
    #define halOnFirstLed()
    #define halOffFirstLed()
    #define halReadFirstLed()
    #define halToggleFirstLed()
  #else
    #define halInitFirstLed()       GPIO_E2_make_out(); GPIO_E2_set()
    #define halUnInitFirstLed()     GPIO_E2_make_in()
    #define halOnFirstLed()         GPIO_E2_clr()
    #define halOffFirstLed()        GPIO_E2_set()
    #define halReadFirstLed()       GPIO_E2_read()
    #define halToggleFirstLed()     GPIO_E2_toggle()
  #endif // (APP_USE_OTAU == 1)

  #define halInitSecondLed()      GPIO_E3_make_out(); GPIO_E3_set()
  #define halUnInitSecondLed()    GPIO_E3_make_in()
  #define halOnSecondLed()        GPIO_E3_clr()
  #define halOffSecondLed()       GPIO_E3_set()
  #define halReadSecondLed()      GPIO_E3_read()
  #define halToggleSecondLed()    GPIO_E3_toggle()

  #define halInitThirdLed()       GPIO_E4_make_out(); GPIO_E4_set()
  #define halUnInitThirdLed()     GPIO_E4_make_in()
  #define halOnThirdLed()         GPIO_E4_clr()
  #define halOffThirdLed()        GPIO_E4_set()
  #define halReadThirdLed()       GPIO_E4_read()
  #define halToggleThirdLed()     GPIO_E4_toggle()
#endif

#endif /*_BSPLEDS_H*/
// eof bspLeds.h
