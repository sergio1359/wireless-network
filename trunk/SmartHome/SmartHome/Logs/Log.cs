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
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Message { get; set; }
        public LogCategory Category {get; set;}
        public HomeDevice HomeDevice {get; set;}


        public Log(string msg, LogCategory category, HomeDevice homeDevice)
        {
            Date = DateTime.Now;
            Message = msg;
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
