#include "arduino.h"
#include "storage.h"
#include "global.h"
#include "network.h"

#define AUTOMODE 0

void setup(void) {
	PIN_MODE(LED_PIN, OUTPUT);

	DIGITAL_WRITE(LED_PIN, HIGH);

	if(Storage.check())
		Storage.load();
	else
		Storage.save();

	SERIAL_BEGIN(57600);

	LCD_INIT();
	LCD_CLEAR();
	LCD_GOTO(0,0);

	Network.init();

	DIGITAL_WRITE(LED_PIN, LOW);
}

#if AUTOMODE
void loop(void) {
		PRINTF("Sending..");
		DATA_MSG msg;
		msg.data[0] = 'H';
		msg.data[1] = 'O';
		msg.data[2] = 'L';
		msg.data[3] = 'A';
		msg.parent = nodeAddress;

		PRINTF(RF_SendMsg(BROADCAST_ADDRESS, msg.raw_bytes, 8) ? "OK\r\n" : "FAIL\r\n");

	delay(AUTOMODE);
}
#else
int command;
DATA_MSG data_msg;

void loop(void){

	if(SERIAL_AVAILAIBLE())
	{
		command = SERIAL_READ();
		if(command>='0' && command<='9')
		{
			command -= 48;

			if(command==0)
				command = 0xFF;

			PRINTF("Sending to ");
			PRINTF(command, HEX);
			PRINTF(".. ");

			data_msg.header.type = 1;
			data_msg.header.reserved = 0;
			data_msg.header.id = 0;
			data_msg.from = nodeAddress;
			data_msg.to = command;
			data_msg.parent = nodeAddress;
			data_msg.data[0] = 'H';
			data_msg.data[1] = 'O';
			data_msg.data[2] = 'L';
			data_msg.data[3] = 'A';

			PRINTF(Network.sendMsg(command, &data_msg) ? "OK\r\n" : "FAIL\r\n");
		}else if(command == 'r')
		{
			if(Network.available())
			{
				Network.readMsg(&data_msg);

				PRINTF(data_msg.from, HEX);
				PRINTF(" says '");

				for(int i=0;i<sizeof(DATA_MSG) - 4;i++)
				{
					PRINTF(data_msg.data[i]);
				}

				PRINTF("' by ");
				PRINTF(data_msg.parent, HEX);
				PRINTF("\r\n");
			}else
			{
				PRINTF("Input buffer empty\r\n");
			}
		}else if(command == 'n')
		{
			Network.printNeighbors();
		}
	}
	Network.update();
}

#endif


int main(void) {
	init();

	setup();

	for (;;)
		loop();

	return 0;
}
