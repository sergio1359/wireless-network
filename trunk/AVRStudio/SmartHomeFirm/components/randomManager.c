/*
 * randomManager.c
 *
 * Created: 30/07/2013 20:23:29
 *  Author: Victor
 */ 
#include "randomManager.h"

static void (*randomCallback)(uint16_t rnd);
static _Bool randomInitialized = false;

void RAND_Next(void* callback)
{
	if(callback != 0)
	{
		randomCallback = callback;
		PHY_RandomReq();
	}
}

void PHY_RandomConf(uint16_t rnd)
{
	if(!randomInitialized)
	{
		srand(rnd);
		randomInitialized = true;
		PHY_RandomReq();
	}else
	{
		(*randomCallback)(rnd);	
	}
}