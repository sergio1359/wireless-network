using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartHome.Plugins;
using SmartHome.Network;

namespace SmartHome.Products
{
    class PinPort
    {
        //Direction
        public Char Port { set; get; }
        public Byte Pin { set; get; }

        public PinPort(Char Port, Byte Pin)
        {
            this.Port = Port;
            this.Pin = Pin;
        }

        public PinPort(string direction)
        {
            Port = direction[0];
            Pin = Byte.Parse(direction[1].ToString());
        }
    }
}
