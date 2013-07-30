/*
 * randomManager.h
 *
 * Created: 30/07/2013 20:24:30
 *  Author: Victor
 */ 


#ifndef RANDOMMANAGER_H_
#define RANDOMMANAGER_H_

#include <stdbool.h>

#include "globals.h"

#ifdef PHY_ENABLE_RANDOM_NUMBER_GENERATOR
void RAND_Next(void* callback);
#endif

#endif /* RANDOMMANAGER_H_ */