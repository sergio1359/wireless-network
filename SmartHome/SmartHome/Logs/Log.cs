using SmartHome.Network.HomeDevices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Logs
{
    public class Log : IComparable<Log>
    {
        public DateTime Date { get; set; }
        public string Log { get; set; }
        public LogCategory Category {get; set;}
        public HomeDevice HomeDevice {get; set;}


        public Log(string log, LogCategory category, HomeDevice homeDevice)
        {
            Date = DateTime.Now;
            Log = log;
            Category = category;
            HomeDevice = homeDevice;
        }

        public int CompareTo(Log other)
        {
            return DateTime.Compare(this.Date, other.Date);
        }

        public override string ToString()
        {
            return "TODO";
        }
    }
}
