using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OperationUARTSender
{
    public class Operation
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
            FromBinary(buffer, 0);
        }

        public void FromBinary(byte[] buffer, int offset)
        {
            SourceAddress = (ushort)((((ushort)buffer[1 + offset]) << 8) | (ushort)buffer[0 + offset]);
            DestinationAddress = (ushort)((((ushort)buffer[3 + offset]) << 8) | (ushort)buffer[2 + offset]);
            OpCode = buffer[4 + offset];
            Args = new byte[buffer.Length - 5 - offset];
            Buffer.BlockCopy(buffer, 5 + offset, Args, 0, Args.Length);
        }
    }
}
