/**************************************************************************//**
\file buttons.c

\brief Implementation of buttons interface.

\author
    Atmel Corporation: http://www.atmel.com \n
    Support email: avr@atmel.com

  Copyright (c) 2008-2012, Atmel Corporation. All rights reserved.
  Licensed under Atmel's Limited License Agreement (BitCloudTM).

\internal
  History:
    12.10.10 A. Taradov - Created
*******************************************************************************/
#if APP_DISABLE_BSP != 1

/******************************************************************************
                   Includes section
******************************************************************************/
#include <types.h>
#include <buttons.h>
#include <irq.h>
#include <bspTaskManager.h>
#include <gpio.h>
#ifdef ATMEGA128RFA1
#include <halDelay.h>
#else
#include <halRfCtrl.h>
#endif

/******************************************************************************
                   Define(s) section
******************************************************************************/
#define PRESSED              1
#define RELEASED             0
#define BSP_BUTTONS_IDLE     0
#define BSP_BUTTONS_BUSY     1
#define BSP_BUTTONS_AMOUNT   3
#define ACT_ON               1
#define ACT_OFF              0

/******************************************************************************
                   Types section
******************************************************************************/
typedef struct _BSP_ButtonsAction_t
{
  uint8_t wasPressed   : 1;
  uint8_t wasReleased  : 1;
  uint8_t currentState : 1;
} BSP_ButtonsAction_t;

typedef struct _BSP_ButtonsDescriptor_t
{
  /* Pin number concerned with button */
  uint8_t pinNumber;
  /* Button action map */
  volatile BSP_ButtonsAction_t action;
  /* Pin pullup processing function */
  void (*enablePinPullup)(void);
  /* Read pin processing function */
  uint8_t (*readPin)(void);
  /* Busy flag*/
  bool busy;
} BSP_ButtonDescriptor_t;

/******************************************************************************
                   Prototypes section
******************************************************************************/
/**************************************************************************//**
\brief  HAL's event handlers about any key has changed state.
******************************************************************************/
static void bspKeysInterruptHandler(void);
static void bspAddButton(uint8_t pinNumber, void (*enablePinPullup)(void), uint8_t (*readPin)(void));

/******************************************************************************
                   Global variables section
******************************************************************************/
static uint8_t state = BSP_BUTTONS_IDLE;
static BSP_ButtonDescriptor_t bspButtons[BSP_BUTTONS_AMOUNT];
static BSP_ButtonsEventFunc_t bspButtonPressHandle;   // callback
static BSP_ButtonsEventFunc_t bspButtonReleaseHandle; // callback

/******************************************************************************
                   Implementations section
******************************************************************************/

/**************************************************************************//**
\brief Creates new button entity

\param[in]
    pinNumber - pin number concerned with button
\param[in]
    enablePinPullup - pin pullup processing handler
\param[in]
    readPin - read pin processing handler
******************************************************************************/
static void bspAddButton(uint8_t pinNumber, void (*enablePinPullup)(void), uint8_t (*readPin)(void))
{
  for (uint8_t i = 0; i < BSP_BUTTONS_AMOUNT; i++)
  {
    if (!bspButtons[i].busy)
    {
      bspButtons[i].pinNumber = pinNumber;
      bspButtons[i].enablePinPullup = enablePinPullup;
      bspButtons[i].readPin = readPin;
      bspButtons[i].busy = true;
      return;
    }
  }
}

/**************************************************************************//**
\brief Initializes buttons module.
******************************************************************************/
static void bspInitButtons(void)
{
#if defined(ATXMEGA128A1) || defined(ATXMEGA256A3) || defined(ATXMEGA256D3)
  HAL_IrqMode_t irqMode;

  irqMode.pin0 = IRQ_ANY_EDGE;
  irqMode.pin1 = IRQ_ANY_EDGE;
  irqMode.pin2 = IRQ_IS_NOT_CHANGED;
  irqMode.pin3 = IRQ_IS_NOT_CHANGED;
  irqMode.pin4 = IRQ_IS_NOT_CHANGED;
  irqMode.pin5 = IRQ_ANY_EDGE;
  irqMode.pin6 = IRQ_IS_NOT_CHANGED;
  irqMode.pin7 = IRQ_IS_NOT_CHANGED;
  HAL_RegisterIrq(IRQ_F0, irqMode, bspKeysInterruptHandler);
  HAL_EnableIrq(IRQ_F0);

  bspAddButton(BSP_KEY0, GPIO_F0_make_pullup, GPIO_F0_read);
  bspAddButton(BSP_KEY1, GPIO_F1_make_pullup, GPIO_F1_read);
  bspAddButton(BSP_KEY5, GPIO_F5_make_pullup, GPIO_F5_read);

#elif defined(ATMEGA128RFA1)
  HAL_RegisterIrq(IRQ_0, IRQ_ANY_EDGE, bspKeysInterruptHandler);
  HAL_RegisterIrq(IRQ_1, IRQ_ANY_EDGE, bspKeysInterruptHandler);
  HAL_RegisterIrq(IRQ_5, IRQ_ANY_EDGE, bspKeysInterruptHandler);
  HAL_EnableIrq(IRQ_0);
  HAL_EnableIrq(IRQ_1);
  HAL_EnableIrq(IRQ_5);

  bspAddButton(BSP_KEY0, GPIO_D0_make_pullup, GPIO_D0_read);
  bspAddButton(BSP_KEY1, GPIO_D1_make_pullup, GPIO_D1_read);
  bspAddButton(BSP_KEY5, GPIO_D5_make_pullup, GPIO_D5_read);
#endif

  for (uint8_t i = 0; i < BSP_BUTTONS_AMOUNT; i++)
  {
    bspButtons[i].enablePinPullup();
    HAL_Delay(1); // delay to ensure pullup setting
    bspButtons[i].action.currentState = bspButtons[i].readPin() ? RELEASED : PRESSED;
  }
}

/**************************************************************************//**
\brief Registers handlers for button events.

\param[in]
    pressed - the handler to process pressing the button
\param[in]
    released - the handler to process releasing the button
\param[in]
    bn - button number.
\return
  BC_FAIL - buttons module is busy, \n
  BC_SUCCESS in other case.
******************************************************************************/
result_t BSP_OpenButtons(void (*pressed)(uint8_t bn), void (*released)(uint8_t bn))
{
  if (state != BSP_BUTTONS_IDLE)
    return BC_FAIL;
  state = BSP_BUTTONS_BUSY;
  bspButtonPressHandle = pressed;
  bspButtonReleaseHandle = released;
  bspInitButtons();
  return BC_SUCCESS;
};

/**************************************************************************//**
\brief Cancel buttons handlers.
\return
  BC_FAIL - buttons module was not opened, \n
  BC_SUCCESS in other case.
******************************************************************************/
result_t BSP_CloseButtons(void)
{
  if (state != BSP_BUTTONS_BUSY)
    return BC_FAIL;

#if defined(ATXMEGA128A1) || defined(ATXMEGA256A3) || defined(ATXMEGA256D3)
  HAL_UnregisterIrq(IRQ_F0);
#elif defined(ATMEGA128RFA1)
  HAL_UnregisterIrq(IRQ_0);
  HAL_UnregisterIrq(IRQ_1);
  HAL_UnregisterIrq(IRQ_5);
#endif

  for (uint8_t i = 0; i < BSP_BUTTONS_AMOUNT; i++)
  {
    bspButtons[i].busy = false;
  }

  bspButtonPressHandle = NULL;
  bspButtonReleaseHandle = NULL;
  state = BSP_BUTTONS_IDLE;
  return BC_SUCCESS;
};

/**************************************************************************//**
\brief Reads state of buttons.

\return
    Current buttons state in a binary way. \n
    Bit 0 defines state of the button 1, \n
    bit 1 defines state of the button 2.
******************************************************************************/
uint8_t BSP_ReadButtonsState(void)
{
  uint8_t i;
  uint8_t state = 0;

  for (i = 0; i < BSP_BUTTONS_AMOUNT; i++)
    if (PRESSED == bspButtons[i].action.currentState)
       state |= (bspButtons[i].pinNumber);

  return state;
}

/**************************************************************************//**
\brief  HAL's event about KEY has changed state.
******************************************************************************/
static void bspKeysInterruptHandler(void)
{
  uint8_t i;

  for (i = 0; i < BSP_BUTTONS_AMOUNT; i++)
  {
    if (bspButtons[i].readPin())
    {
      if (PRESSED == bspButtons[i].action.currentState)
      {
        bspButtons[i].action.currentState = RELEASED;
        bspButtons[i].action.wasReleased = ACT_ON;
        bspPostTask(BSP_BUTTONS);
      }
    }
    else
    {
      if (RELEASED == bspButtons[i].action.currentState)
      {
        bspButtons[i].action.currentState = PRESSED;
        bspButtons[i].action.wasPressed = ACT_ON;
        bspPostTask(BSP_BUTTONS);
      }
    }
  }
}

/**************************************************************************//**
\brief  BSP's event about KEY has changed state.
******************************************************************************/
void bspButtonsHandler(void)
{
  uint8_t i;

  for (i = 0; i < BSP_BUTTONS_AMOUNT; i++)
  {
    if (ACT_ON == bspButtons[i].action.wasPressed)
    {
      bspButtons[i].action.wasPressed = ACT_OFF;
      if (NULL != bspButtonPressHandle)
        bspButtonPressHandle(bspButtons[i].pinNumber);
    }
    if (ACT_ON == bspButtons[i].action.wasReleased)
    {
      bspButtons[i].action.wasReleased = ACT_OFF;
      if (NULL != bspButtonReleaseHandle)
        bspButtonReleaseHandle(bspButtons[i].pinNumber);
    }
  }
}

#endif // APP_DISABLE_BSP != 1

// end of buttons.c
