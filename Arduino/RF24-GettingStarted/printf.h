/*
 * printf.h
 *
 *  Created on: 17/05/2012
 *      Author: Victor
 */

#ifndef PRINTF_H_
#define PRINTF_H_

#include <stdarg.h>
void prf(char *fmt, ... ){
        char tmp[128]; // resulting string limited to 128 chars
        va_list args;
        va_start (args, fmt );
        vsnprintf(tmp, 128, fmt, args);
        va_end (args);
        Serial.print(tmp);
}


#endif /* PRINTF_H_ */
