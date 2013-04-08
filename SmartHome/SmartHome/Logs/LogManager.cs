using SmartHome.Network.HomeDevices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Logs
{
    public class LogManager
    {
        public static event Action<Log> newMessage;
        public static List<Log> Logs { get; set; }

        public static void AddLog(string log, LogCategory category, HomeDevice homeDevice)
        {
            Log msg = new Log(log, category, homeDevice);
            Logs.Insert(0, msg);           

            if (newMessage != null)
            {
                newMessage(msg);
            }
        }

    }
}
