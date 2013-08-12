using DataLayer.Entities.Enums;
using DataLayer.Entities.HomeDevices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Entities
{
    public class Log : IComparable<Log>
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Message { get; set; }
        public LogTypes Category {get; set;}
        public virtual HomeDevice HomeDevice {get; set;}


        public int CompareTo(Log other)
        {
            return DateTime.Compare(this.Date, other.Date);
        }
    }
}
