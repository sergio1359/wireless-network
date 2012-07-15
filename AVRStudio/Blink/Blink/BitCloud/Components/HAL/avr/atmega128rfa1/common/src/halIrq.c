/**************************************************************************//**
  \file  halIrq.c

  \brief Implementation of HWD IRQ interface.

  \author
      Atmel Corporation: http://www.atmel.com \n
      Support email: avr@atmel.com

    Copyright (c) 2008-2012, Atmel Corporation. All rights reserved.
    Licensed under Atmel's Limited License Agreement (BitCloudTM).

  \internal
    History:
      5/12/07 A. Khromykh - Created
 ******************************************************************************/
/******************************************************************************
 *   WARNING: CHANGING THIS FILE MAY AFFECT CORE FUNCTIONALITY OF THE STACK.  *
 *   EXPERT USERS SHOULD PROCEED WITH CAUTION.                                *
 ******************************************************************************/
#if defined(HAL_USE_EXT_IRQ)
/******************************************************************************
                   Includes section
******************************************************************************/
#include <halIrq.h>
#include <sleep.h>
#include <halSleepTimerClock.h>
#include <halAppClock.h>
#include <halDbg.h>
#include <halDiagnostic.h>

/******************************************************************************
                   Global variables section
******************************************************************************/
IrqCallback_t IrqCallbackList[HAL_NUM_IRQ_LINES] =
  {NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL};

/******************************************************************************
                   Implementations section
******************************************************************************/
/******************************************************************************
Sets configuration of pins and the registers.
Parameters:
  irqNumber - number of interrupt.
  irqMode - mode of interrupt
Returns:
  none.
******************************************************************************/
void halSetIrqConfig(uint8_t irqNumber, uint8_t irqMode)
{
  if (irqNumber < IRQ_4)
  {
    uint8_t shift = irqNumber << 1;

    DDRD &= ~(1 << irqNumber); // IRQ pin is input
    PORTD |= (1 << irqNumber); // Enable pullup
    // Setup corresponding interrupt sence control
    EICRA &= ~(3 << shift);
    EICRA |= (irqMode & 0x03) << shift;
  }
  else
  {
    uint8_t shift = (irqNumber - IRQ_4) << 1;

    DDRE &= ~(1 << irqNumber); // IRQ pin is input
    PORTE |= (1 << irqNumber); // Enable pullup
    // Setup interrupt sence control
    EICRB &= ~(3 << shift);
    EICRB |= (irqMode & 0x03) << shift;
  }

  // Clear the INTn interrupt flag
  EIFR |= (1 << irqNumber);
}

/**************************************************************************//**
\brief Clears configuration of pins and the registers.
\param[in]
  irqNumber - number of interrupt.
******************************************************************************/
void halClrIrqConfig(uint8_t irqNumber)
{
  if (irqNumber < IRQ_4)
  {
    uint8_t shift = irqNumber << 1;

    DDRD &= ~(1 << irqNumber);  // IRQ pin is input
    PORTD &= ~(1 << irqNumber); // Disable pullup
    EICRA &= ~(3 << shift);     // Clear interrupt sence control
  }
  else
  {
    uint8_t shift = (irqNumber - IRQ_4) << 1;

    DDRE &= ~(1 << irqNumber);  // IRQ pin is input
    PORTE &= ~(1 << irqNumber); // Disable pullup
    EICRB &= ~(3 << shift);     // Clear interrupt sence control
  }
}

/******************************************************************************
 External interrupt 5 handler
******************************************************************************/
ISR(INT0_vect)
{
  BEGIN_MEASURE
  halWakeupFromIrq();
  IrqCallbackList[IRQ_0 - HAL_FIRST_VALID_IRQ]();
  END_MEASURE(HALISR_INT0_VECT_TIME_LIMIT)
}

/******************************************************************************
 External interrupt 5 handler
******************************************************************************/
ISR(INT1_vect)
{
  BEGIN_MEASURE
  halWakeupFromIrq();
  IrqCallbackList[IRQ_1 - HAL_FIRST_VALID_IRQ]();
  END_MEASURE(HALISR_INT1_VECT_TIME_LIMIT)
}

/******************************************************************************
 External interrupt 5 handler
******************************************************************************/
ISR(INT2_vect)
{
  BEGIN_MEASURE
  halWakeupFromIrq();
  IrqCallbackList[IRQ_2 - HAL_FIRST_VALID_IRQ]();
  END_MEASURE(HALISR_INT2_VECT_TIME_LIMIT)
}

/******************************************************************************
 External interrupt 5 handler
******************************************************************************/
ISR(INT3_vect)
{
  BEGIN_MEASURE
  halWakeupFromIrq();
  IrqCallbackList[IRQ_3 - HAL_FIRST_VALID_IRQ]();
  END_MEASURE(HALISR_INT3_VECT_TIME_LIMIT)
}

/******************************************************************************
 External interrupt 5 handler
******************************************************************************/
ISR(INT4_vect)
{
  BEGIN_MEASURE
  halWakeupFromIrq();
  IrqCallbackList[IRQ_4 - HAL_FIRST_VALID_IRQ]();
  END_MEASURE(HALISR_INT4_VECT_TIME_LIMIT)
}

/******************************************************************************
 External interrupt 5 handler
******************************************************************************/
ISR(INT5_vect)
{
  BEGIN_MEASURE
  halWakeupFromIrq();
  IrqCallbackList[IRQ_5 - HAL_FIRST_VALID_IRQ]();
  END_MEASURE(HALISR_INT5_VECT_TIME_LIMIT)
}

/******************************************************************************
 External interrupt 5 handler
******************************************************************************/
ISR(INT6_vect)
{
  BEGIN_MEASURE
  halWakeupFromIrq();
  IrqCallbackList[IRQ_6 - HAL_FIRST_VALID_IRQ]();
  END_MEASURE(HALISR_INT6_VECT_TIME_LIMIT)
}

/******************************************************************************
 External interrupt 7 handler
******************************************************************************/
ISR(INT7_vect)
{
  BEGIN_MEASURE
  halWakeupFromIrq();
  IrqCallbackList[IRQ_7 - HAL_FIRST_VALID_IRQ]();
  END_MEASURE(HALISR_INT7_VECT_TIME_LIMIT)
}
#endif // defined(HAL_USE_EXT_IRQ)

// eof irq.c
