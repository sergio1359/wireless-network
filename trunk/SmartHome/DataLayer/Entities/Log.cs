using DataLayer.Entities.Enums;
using DataLayer.Entities.HomeDevices;
using System;
using System.ComponentModel.DataAnnotations;

namespace DataLayer.Entities
{
    public class Log : IComparable<Log>
    {
        [Key]
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
