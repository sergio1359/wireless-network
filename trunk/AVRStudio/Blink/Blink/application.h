/*
 * application.h
 *
 * Created: 19/07/2012 13:20:00
 *  Author: Victor
 */ 


#ifndef APPLICATION_H_
#define APPLICATION_H_

#define APP_DEVICE_TYPE DEVICE_TYPE_ROUTER

typedef enum
{
	APP_INITING_STATE,
	APP_STARTING_NETWORK_STATE,
	APP_IN_NETWORK_STATE,
	APP_LEAVING_NETWORK_STATE,
	APP_STOP_STATE
} AppState_t;

#endif /* APPLICATION_H_ */