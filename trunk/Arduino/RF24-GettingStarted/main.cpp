#include <WProgram.h>
#include <arduino.h>
#include "libraries/SPI/SPI.h"
#include <SPI.h>
#include "libraries/RF24/RF24.h"
#include "storage.h"
#include "global.h"
#include "network.h"

#define AUTOMODE 0

void setup(void) {
	pinMode(LED_PIN, OUTPUT);

	digitalWrite(LED_PIN, HIGH);

	if(Storage.check())
		Storage.load();
	else
		Storage.save();

	Serial.begin(57600);

	Network.init();

	digitalWrite(LED_PIN, LOW);
}

#if AUTOMODE
void loop(void) {
		Serial.print("Sending..");
		DATA_MSG msg;
		msg.data[0] = 'H';
		msg.data[1] = 'O';
		msg.data[2] = 'L';
		msg.data[3] = 'A';
		msg.parent = nodeAddress;

		Serial.println(RF_SendMsg(BROADCAST_ADDRESS, msg.raw_bytes, 8) ? "OK" : "FAIL");

	delay(AUTOMODE);
}
#else
int command;
DATA_MSG data_msg;

void loop(void){

	if(Serial.available())
	{
		command = Serial.read();
		if(command>='0' && command<='9')
		{
			command -= 48;

			if(command==0)
				command = 0xFF;

			Serial.print("Sending to ");
			Serial.print(command, HEX);
			Serial.print(".. ");

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

			Serial.println(Network.sendMsg(command, &data_msg) ? "OK" : "FAIL");
		}else if(command == 'r')
		{
			if(Network.available())
			{
				Network.readMsg(&data_msg);

				Serial.print(data_msg.from, HEX);
				Serial.print(" says '");

				for(int i=0;i<sizeof(DATA_MSG) - 4;i++)
				{
					Serial.print(data_msg.data[i]);
				}

				Serial.print("' by ");
				Serial.println(data_msg.parent, HEX);
			}else
			{
				Serial.println("Buffer de entrada vacío");
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
