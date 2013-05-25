using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OperationUARTSender
{
    class Operation
    {
        public ushort SourceAddress { get; set; }
        public ushort DestinationAddress { get; set; }
        public byte OpCode { get; set; }
        public byte[] Args { get; set; }

        public byte[] ToBinary()
        {
            List<byte> result = new List<byte>();
            result.AddRange(BitConverter.GetBytes(SourceAddress));
            result.AddRange(BitConverter.GetBytes(DestinationAddress));
            result.Add(OpCode);
            result.AddRange(Args);

            return result.ToArray();
        }
    }
}
