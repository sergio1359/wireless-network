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

        public void FromBinary(byte[] buffer)
        {
            SourceAddress = (ushort)((((ushort)buffer[1]) << 8) | (ushort)buffer[0]);
            DestinationAddress = (ushort)((((ushort)buffer[3]) << 8) | (ushort)buffer[2]);
            OpCode = buffer[4];
            Args = new byte[buffer.Length - 5];
            Buffer.BlockCopy(buffer, 5, Args, 0, Args.Length);
        }
    }
}
