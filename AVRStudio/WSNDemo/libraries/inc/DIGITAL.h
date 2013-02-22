/*
 * DIGITAL.h
 *
 * Created: 18/11/2012 15:44:44
 *  Author: Victor
 */ 


#ifndef DIGITAL_H_
#define DIGITAL_H_

#include "halGpio.h"

// DEFINES FOR DIGITAL PORTSACCESS

/* PIN ACCESS
*  HAL_GPIO_PORT(name, port)
*
*  void    HAL_GPIO_##name##_set()
*  void    HAL_GPIO_##name##_clr()
*  void    HAL_GPIO_##name##_toggle()
*  void    HAL_GPIO_##name##_in()
*  void    HAL_GPIO_##name##_out()
*  void    HAL_GPIO_##name##_pullup()
*  uint8_t HAL_GPIO_##name##_read()
*  uint8_t HAL_GPIO_##name##_state()
*/

HAL_GPIO_PORT(PORTA, A);
HAL_GPIO_PORT(PORTB, B);
HAL_GPIO_PORT(PORTC, C);
HAL_GPIO_PORT(PORTD, D);
HAL_GPIO_PORT(PORTE, E);
HAL_GPIO_PORT(PORTF, F);
HAL_GPIO_PORT(PORTG, G);

//#define HAL_GPIO_PORT_set(port, mask)	HAL_GPIO_##port##_set(mask)
//#define HAL_GPIO_PORT_clr(port, mask)	HAL_GPIO_##port##_clr(mask)
//#define HAL_GPIO_PORT_toggle(port)		HAL_GPIO_##port##_toggle()
//#define HAL_GPIO_PORT_in(port)			HAL_GPIO_##port##_in()
//#define HAL_GPIO_PORT_out(port)			HAL_GPIO_##port##_out()
//#define HAL_GPIO_PORT_pullup(port)		HAL_GPIO_##port##_pullup()
//#define HAL_GPIO_PORT_read(port)		HAL_GPIO_##port##_read()
//#define HAL_GPIO_PORT_state(port)		HAL_GPIO_##port##_state()


/* PIN ACC
*  HAL_GPIO_PIN(name, port, bit)
*
*  void    HAL_GPIO_##name##_set()
*  void    HAL_GPIO_##name##_clr()
*  void    HAL_GPIO_##name##_toggle()
*  void    HAL_GPIO_##name##_in()
*  void    HAL_GPIO_##name##_out()
*  void    HAL_GPIO_##name##_pullup()
*  uint8_t HAL_GPIO_##name##_read()
*  uint8_t HAL_GPIO_##name##_state() 
*/

HAL_GPIO_PIN(PB0, B, 0);
HAL_GPIO_PIN(PB1, B, 1);
HAL_GPIO_PIN(PB2, B, 2);
HAL_GPIO_PIN(PB3, B, 3);
HAL_GPIO_PIN(PB4, B, 4); //OC2A
HAL_GPIO_PIN(PB5, B, 5); //OC1A
HAL_GPIO_PIN(PB6, B, 6); //OC1B
HAL_GPIO_PIN(PB7, B, 7); //OC0A

HAL_GPIO_PIN(PD0, D, 0); //INT0 SCL
HAL_GPIO_PIN(PD1, D, 1); //INT1 SDA
HAL_GPIO_PIN(PD2, D, 2); //INT2 RX1
HAL_GPIO_PIN(PD3, D, 3); //INT3 TX1

HAL_GPIO_PIN(PE0, E, 0); //RX0
HAL_GPIO_PIN(PE1, E, 1); //RX1
HAL_GPIO_PIN(PE2, E, 2); //AIN0
HAL_GPIO_PIN(PE3, E, 3); //AIN1 OC3A
HAL_GPIO_PIN(PE4, E, 4); //INT4 OC3B
HAL_GPIO_PIN(PE5, E, 5); //INT5 OC3C
HAL_GPIO_PIN(PE6, E, 6); //INT6
HAL_GPIO_PIN(PE7, E, 7); //INT7

HAL_GPIO_PIN(PF0, F, 0); //ADC0
HAL_GPIO_PIN(PF1, F, 1); //ADC1
HAL_GPIO_PIN(PF2, F, 2); //ADC2
HAL_GPIO_PIN(PF3, F, 3); //ADC3
HAL_GPIO_PIN(PF4, F, 4); //ADC4
HAL_GPIO_PIN(PF5, F, 5); //ADC5
HAL_GPIO_PIN(PF6, F, 6); //ADC6
HAL_GPIO_PIN(PF7, F, 7); //ADC7

HAL_GPIO_PIN(PG0, G, 0);
HAL_GPIO_PIN(PG1, G, 1);
HAL_GPIO_PIN(PG2, G, 2);
//HAL_GPIO_PIN(PG3, G, 3); //TOSC1
//HAL_GPIO_PIN(PG4, G, 4); //TOSC2
HAL_GPIO_PIN(PG5, G, 5); //OC0B


#endif /* DIGITAL_H_ */