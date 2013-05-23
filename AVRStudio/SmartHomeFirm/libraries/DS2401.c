/*
 * _DS2401.c
 *
 * Created: 10/04/2013 13:57:23
 *  Author: Victor
 *
 * Description: This file contains routines to work with the DS2401 Electronic Serial Number 
 */ 
#include "DS2401.h"
#include <util/delay.h>
#include "DIGITAL.h"

#if SNUM_CHIP == SNUM_DS2401

/* Set 1-Wire low or high. */
#define OUTP_0() {							\
	 HAL_GPIO_PD5_out();					\
	 HAL_GPIO_PD5_clr();					\
}	 

#define OUTP_1() {							\
	HAL_GPIO_PD5_in();						\
	HAL_GPIO_PD5_set();		/* pull up */	\
}

/* Read one bit. */
#define INP() HAL_GPIO_PD5_read()

#define NOP __asm__ __volatile__ ("cp r0,r0\n")
#define NOP3 NOP; NOP; NOP

/*
 * Recommended delay times in us.
 */
#define udelay_tA() udelay_6()
#define tA 10         /*      max 15 */
#define tB 100
#define tC 100           /* max 120 */
#define tD 10
#define tE 30            /* max 15 */
#define tF 60
#define tG 0
#define tH 1500
#define tI 50
#define tJ 360
#define tK NOP3


uint8_t serialNumber[SERIAL_NUMBER_SIZE];
_Bool serialNumber_OK;

_Bool owreset(void)
{
    uint8_t result;
    OUTP_0();
    _delay_us(tH);
    OUTP_1();           /* Releases the bus */
    _delay_us(tI);
    result = INP();
    _delay_us(tJ);
    return result;
}

void owwriteb(uint8_t byte)
{
    int i = 7;
    do {
        if (byte & 0x01) {
            OUTP_0();
            _delay_us(tA);
            OUTP_1();           /* Releases the bus */
            _delay_us(tB);
        } else {
            OUTP_0();
            _delay_us(tC);
            OUTP_1();           /* Releases the bus */
            _delay_us(tD);
        }
        if (i == 0)
            return;
        i--;
        byte >>= 1;
    } while (1);
}

uint8_t owreadb(void)
{
    uint8_t result = 0;
    int i = 7;
    do {
        OUTP_0();
        tK;
        OUTP_1();           /* Releases the bus */
        _delay_us(tE);
        if (INP())
            result |= 0x80;     /* LSbit first */
        _delay_us(tF);
        if (i == 0)
            return result;
        i--;
        result >>= 1;
    } while (1);
}

uint8_t crc8( uint8_t *addr, uint8_t len)
{
	uint8_t crc=0;
	
	for (uint8_t i=0; i<len;i++)
	{
		uint8_t inbyte = addr[i];
		for (uint8_t j=0;j<8;j++)
		{
			uint8_t mix = (crc ^ inbyte) & 0x01;
			crc >>= 1;
			if (mix)
			crc ^= 0x8C;
			
			inbyte >>= 1;
		}
	}
	return crc;
}

_Bool DS2401_Init() {
    uint8_t crc, acc;
    int i;
    uint8_t buffer[SERIAL_NUMBER_SIZE + 1];

    if (serialNumber_OK) return 1;

    OUTP_1();
    _delay_us(50000);
    if (owreset()) return 0; // fail

	ATOMIC_SECTION_ENTER
    owwriteb(0x33);     /* Read ROM command. */
    buffer[0] = owreadb(); //family byte (0x01)
	
    /* We receive 6 bytes in the reverse order, LSbyte first. */
    for (i = 1; i <= SERIAL_NUMBER_SIZE; i++) {
        buffer[i] = owreadb();
    }
    crc = owreadb();
	ATOMIC_SECTION_LEAVE
	
	acc = crc8(buffer, SERIAL_NUMBER_SIZE + 1);

    if (buffer[0] != 0x01 || acc != crc) return 0; // fail

    serialNumber_OK = 1;
	for (i = 1; i <= SERIAL_NUMBER_SIZE; i++) {
		serialNumber[SERIAL_NUMBER_SIZE - i] = buffer[i];
	}
    
    return 1;
}

#endif // SNUM_CHIP == SNUM_DS2401
