using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OperationUARTSender
{
    public class OutputMessage
    {
        #region Properties
        public bool SecurityEnabled { get; set; }

        public bool RoutingEnabled { get; set; }

        public int EndPoint { get; set; }

        public int Retries { get; set; }

        public Operation Content { get; set; } 
        #endregion

        public OutputMessage()
        {
            Content = new Operation();
        }

        public byte[] ToBinary()
        {
            List<byte> result = new List<byte>();

            byte headerByte = 0;

            if (SecurityEnabled)
                headerByte |= (1 << 7);

            if (RoutingEnabled)
                headerByte |= (1 << 6);

            headerByte |= (byte)(EndPoint & 0x3F);

            result.Add(headerByte);

            result.Add((byte)Retries);
            result.AddRange(Content.ToBinary());

            return result.ToArray();
        }

        public void FromBinary(byte[] buffer)
        {
            SecurityEnabled = (buffer[0] & 0x80) != 0;

            RoutingEnabled  = (buffer[0] & 0x40) != 0;

            EndPoint = (buffer[0] & 0x3F);

            Retries = buffer[1];

            Content.FromBinary(buffer, 2);
        }
    }
}
