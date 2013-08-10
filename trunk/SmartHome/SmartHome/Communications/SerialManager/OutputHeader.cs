#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartHome.Comunications.Messages;
#endregion

namespace SerialPortManager.ConnectionManager
{
    public class OutputHeader
    {
        #region Properties
        public bool SecurityEnabled { get; set; }

        public bool RoutingEnabled { get; set; }

        public int EndPoint { get; set; }

        public int Retries { get; set; }

        public IMessage Content { get; set; }

        public float Priority { get; private set; }
        #endregion

        public OutputHeader(float priority = 0)
        {
            this.Priority = priority;
        }

        public byte[] ToBinary()
        {
            List<byte> result = new List<byte>();

            byte headerByte = 0;

            if (SecurityEnabled)
                headerByte |= 0x80;

            if (RoutingEnabled)
                headerByte |= 0x40;

            headerByte |= (byte)(EndPoint & 0x0F);

            result.Add(headerByte);

            result.Add((byte)Retries);

            if (Content != null)
                result.AddRange(Content.ToBinary());

            return result.ToArray();
        }

        public void FromBinary(byte[] buffer)
        {
            SecurityEnabled = (buffer[0] & 0x80) != 0;

            RoutingEnabled = (buffer[0] & 0x40) != 0;

            EndPoint = (buffer[0] & 0x0F);

            Retries = buffer[1];

            if (Content != null)
                Content.FromBinary(buffer, 2);
        }
    }
}
