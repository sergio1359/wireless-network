using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Comunications.Messages
{
    public abstract class IMessage
    {
        public ushort DestinationAddress { get; set; }

        public abstract byte[] ToBinary();

        public abstract void FromBinary(byte[] buffer, int offset);
    }
}
