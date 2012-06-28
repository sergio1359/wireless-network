#include <C8051F340.h>

sbit SDA = P1^3;
sbit SCL = P1^4;

void i2c_dly()
{
int i;
for(i=0;i<100;i++);
}

void i2c_start(void)
{
  SDA = 1;             // i2c start bit sequence
  i2c_dly();
  SCL = 1;
  i2c_dly();
  SDA = 0;
  i2c_dly();
  SCL = 0;
  i2c_dly();
}

void i2c_stop(void)
{
  SDA = 0;             // i2c stop bit sequence
  i2c_dly();
  SCL = 1;
  i2c_dly();
  SDA = 1;
  i2c_dly();
}

unsigned char i2c_rx(char ack)
{
  char i, midato=0;
  SDA = 1; 
  SCL = 1;
  i2c_dly();

	for (i=7;i>=0;i--)
	{
		midato = (midato<<1)|SDA;
		SCL = 1;
		i2c_dly();
		SCL = 0;
		i2c_dly();
	}
	if(ack) 
		SDA = 0;
  	else 
		SDA = 1;

  SCL = 1;
  i2c_dly();             // send (N)ACK bit
  SCL = 0;
  SDA = 1;
  i2c_dly();
  return midato;
}

bit i2c_tx(unsigned char d)
{
char i;
static bit b;
	for (i=7;i>=0;i--)
	{
		SDA = (d>>i)&1;
		i2c_dly();
		SCL = 1;
		i2c_dly();
		SCL = 0;
		i2c_dly();
	}

  SDA = 1;
  SCL = 1;
  i2c_dly();
  b = SDA;          // possible ACK bit
  SCL = 0;
  i2c_dly();
  return b;
}


void write_register(char device, char addr, char cmd)
{
i2c_start();              // send start sequence
i2c_tx(device);             // SRF08 I2C address with R/W bit clear
i2c_tx(addr);             // SRF08 command register address
i2c_tx(cmd);             // command to start ranging in cm
i2c_stop();               // send stop sequence 
}

char read_register(char device, char addr)
{
char midato;
i2c_start();              // send start sequence
i2c_tx(device);             // SRF08 I2C address with R/W bit clear
i2c_tx(addr);             // SRF08 light sensor register address
i2c_start();              // send a restart sequence
i2c_tx(device|1);             // SRF08 I2C address with R/W bit set
midato = i2c_rx(0);     // get low byte of the range - note we don't acknowledge the last byte.
i2c_stop();               // send stop sequence 
return midato;
}
