using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OperationUARTSender
{
    public class InputMessage
    {
        #region Properties
		public bool SecurityEnabled { get; set; }

        public bool RoutingEnabled { get; set; }

        public int EndPoint { get; set; }

        public int NextHop { get; set; }

        public int RSSI { get; set; }

        public Operation Content { get; set; } 
	#endregion

        public InputMessage()
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

            result.AddRange(BitConverter.GetBytes(NextHop));

            result.Add((byte)RSSI);
            result.AddRange(Content.ToBinary());

            return result.ToArray();
        }

        public void FromBinary(byte[] buffer)
        {
            SecurityEnabled = (buffer[0] & 0x80) != 0;

            RoutingEnabled  = (buffer[0] & 0x40) != 0;

            EndPoint = (buffer[0] & 0x3F);

            NextHop = (ushort)((((ushort)buffer[2]) << 8) | (ushort)buffer[1]);

            RSSI = buffer[3];

            Content.FromBinary(buffer, 4);
        }
    }
}
