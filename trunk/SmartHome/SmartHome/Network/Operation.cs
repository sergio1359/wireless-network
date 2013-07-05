using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartHome.Plugins;
using SmartHome.Network.HomeDevices;
using SmartHome.Comunications;

namespace SmartHome.Network
{
    public class Operation
    {
        public int ID { get; set; }
        public String Name { get; set; }
        public HomeDevice DestionationHomeDevice { get; set; }
        public OPCode OPCode { get; set; }
        public byte[] Args { get; set; }
        public bool Enable { get; set; }

        public List<TimeRestriction> TimeRestrictions { get; set; }
        public List<ConditionalRestriction> ConditionalRestriction { get; set; }

        public virtual void Execute() { }
    }

    public class TimeRestriction
    {
        public byte MaskWeekDays { get; set; }

        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }

        public DateTime HourStart { get; set; }
        public DateTime HourEnd { get; set; }

        public TimeRestriction(byte maskWeekDays, int fromHour, int fromMin, int FromSeg, int ToHour, int ToMin, int ToSeg)
        {
            MaskWeekDays = maskWeekDays;
            HourStart = new DateTime(1, 1, 1, fromHour, fromMin, FromSeg);
            HourEnd = new DateTime(1, 1, 1, ToHour, ToMin, ToSeg);
        }
    }

    public class ConditionalRestriction
    {
        public HomeDevice HomeDeviceValue;
        public dynamic Value;
        public Operations Operation;
        public string NamePropierty;


    }

    public static class Sheduler
    {
        public static SortedDictionary<DateTime, List<Operation>> TimeActions;

    }

    public enum WeekDays : byte
    {
        Monday = 0x40,
        Tuesday = 0x20,
        Wednesday = 0x10,
        Thursday = 0x08,
        Friday = 0x04,
        Saturday = 0x2,
        Sunday = 0x01,
    }

    public enum Operations
    {

    }
}