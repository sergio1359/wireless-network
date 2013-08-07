#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartHome.Comunications.Messages;
#endregion

namespace SmartHome.Communications.SerialManager
{
    public class InputHeader
    {
        #region Properties
        public bool SecurityEnabled { get; set; }

        public bool RoutingEnabled { get; set; }

        public bool IsConfirmation { get; set; }

        public int EndPoint { get; set; }

        public int NextHop { get; set; }

        public int RSSI { get; set; }

        public IMessage Content { get; set; }
        #endregion

        public InputHeader()
        {
        }

        public byte[] ToBinary()
        {
            List<byte> result = new List<byte>();

            byte headerByte = 0;

            if (SecurityEnabled)
                headerByte |= 0x80;

            if (RoutingEnabled)
                headerByte |= 0x40;

            if (IsConfirmation)
                headerByte |= 0x20;

            headerByte |= (byte)(EndPoint & 0x0F);

            result.Add(headerByte);

            result.AddRange(BitConverter.GetBytes(NextHop));

            result.Add((byte)RSSI);

            if(Content != null)
                result.AddRange(Content.ToBinary());

            return result.ToArray();
        }

        public void FromBinary(byte[] buffer)
        {
            SecurityEnabled = (buffer[0] & 0x80) != 0;

            RoutingEnabled = (buffer[0] & 0x40) != 0;

            IsConfirmation = (buffer[0] & 0x20) != 0;

            EndPoint = (buffer[0] & 0x0F);

            NextHop = (ushort)((((ushort)buffer[2]) << 8) | (ushort)buffer[1]);

            RSSI = (sbyte)buffer[3];

            if(Content != null)
                Content.FromBinary(buffer, 4);
        }
    }
}
