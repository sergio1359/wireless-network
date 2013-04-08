using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Extensions.Logs
{
    public class LogManager
    {
        public static event Action<Log> newMessage;

        public static List<Log> Messages { get; set; }

        public static void InformationMessage(string information)
        {
            Log msg = new Log("Info", information);
            messageList.Insert(0, msg);
            System.Console.ForegroundColor = ConsoleColor.Blue;
            System.Console.WriteLine("Info: " + information);
            System.Console.ForegroundColor = ConsoleColor.White;
            if (newMessage != null)
            {
                newMessage(msg);
            }
        }

    }
}
