using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Threading;

///COPY TO INTELLIROOM :(

namespace SmartHome.Messages
{
    public class SerialSingleton
    {
        private static Serial serial;

        public static Serial Serial
        {
            get
            {
                if (serial == null)
                {
                    serial = new Serial();
                }
                return serial;
            }
        }
    }

    public class Serial : IConnexion
    {
        
    }
}