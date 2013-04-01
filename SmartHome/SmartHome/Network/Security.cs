using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Network
{
    public class Security
    {
        public const Byte CHANNEL = 0x00;
        public const UInt16 PANID = 0x00;

        public int ID { set; get; }
        public Byte Channel { set; get; }
        public UInt16 PanId { set; get; }
        public String SecurityKey { set; get; }
    }
}
