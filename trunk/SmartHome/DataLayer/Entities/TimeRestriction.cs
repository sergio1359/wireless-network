#region Using Statements
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 
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
