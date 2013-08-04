/*
 * APP_SESSION.h
 *
 * Created: 04/08/2013 12:19:54
 *  Author: Victor
 */ 


#ifndef APP_SESSION_H_
#define APP_SESSION_H_

enum
{
	OK = 0,
	ERROR_FRAGMENT_TOTAL_NOT_EXPECTED,
	ERROR_FRAGMENT_ORDER,
	ERROR_WAITING_FIRST_FRAGMENT,
	ERROR_BUSY_RECEIVING_STATE
}WRITE_SESION_STATUS_CODES;

typedef struct  
{
	unsigned int receivingState : 1;	//LSB
	unsigned int waitingForReset : 1;
	
	uint8_t currentRecvFragment;
	uint8_t totalRecvExpected;
	uint16_t currentRecvIndex;
	
	uint8_t* writeBuffer;
}WriteSession_t;

typedef struct
{
	unsigned int sendingState : 1;	//LSB
	
	uint16_t destinationAddress;
	
	uint8_t currentSendFragment;
	uint8_t totalSendExpected;
	uint16_t currentSendIndex;
	uint16_t currentSendFrameSize;
	
	uint8_t* readBuffer;
}ReadSession_t;

#endif /* APP_SESSION_H_ */