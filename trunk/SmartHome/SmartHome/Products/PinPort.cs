using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartHome.Plugins;
using SmartHome.Network;

namespace SmartHome.Products
{
    public class PinPort
    {
        //Direction
        public char Port { set; get; }
        public byte Pin { set; get; }

        public PinPort(char Port, byte Pin)
        {
            this.Port = Port;
            this.Pin = Pin;
        }

        public PinPort(string direction)
        {
            Port = direction[0];
            Pin = byte.Parse(direction[1].ToString());
        }

        public PinPort(int pinPortNumber)
        {
            Port = (char)((pinPortNumber/8) + 'A');
            Pin = (byte)(pinPortNumber % 8);
        }

        public PinPort(byte port, byte pin)
        {
            Port = (char)(port + 'A');
            Pin = pin;
        }
    }
}
