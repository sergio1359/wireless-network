#include <arduino_lit.h>
#include "libraries/NokiaLCD/nokiaLCD.h"

//#define PRINTMODE

#ifdef PRINTMODE
#define SERIAL_BEGIN(x) serial_begin(x)
#define SERIAL_AVAILAIBLE() serial_available()
#define SERIAL_READ() serial_getc()
#define PRINTF(x, ...) PRINT(x, ##__VA_ARGS__)
#else
#define SERIAL_BEGIN(x)
#define SERIAL_AVAILAIBLE() 0
#define SERIAL_READ() 0
#define PRINTF(x, ...)
#endif

//#define LCD_ON

#ifdef LCD_ON
#define LCD_PRINT(x) LcdString(x)
#define LCD_PRINT_CHAR(x) LcdCharacter(x)
#define LCD_PRINT_NUM(x) LcdCharacter((char)x+48)
#define LCD_GOTO(x, y) LcdGoToXY(x,y)
#define LCD_CLEAR() LcdClear()
#define LCD_INIT() LcdInitialise()
#else
#define LCD_PRINT(x)
#define LCD_PRINT_CHAR(x)
#define LCD_PRINT_NUM(x)
#define LCD_GOTO(x, y)
#define LCD_CLEAR()
#define LCD_INIT()
#endif
