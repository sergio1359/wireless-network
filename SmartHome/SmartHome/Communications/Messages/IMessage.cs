using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Comunications.Messages
{
    public interface IMessage
    {
        byte[] ToBinary();

        void FromBinary(byte[] buffer, int offset);
    }
}
