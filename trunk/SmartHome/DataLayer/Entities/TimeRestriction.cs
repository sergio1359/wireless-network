#region Using Statements
using System;
using System.ComponentModel.DataAnnotations;

#endregion

namespace DataLayer.Entities
{
    public class TimeRestriction
    {
        [Key]
        public int Id { get; set; }

        public byte MaskWeekDays { get; set; }

        public DateTime DateStart { get; set; }

        public DateTime DateEnd { get; set; }

        public TimeSpan TimeStart { get; set; }

        public TimeSpan TimeEnd { get; set; }
    }
}
