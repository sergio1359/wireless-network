using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartHome.Plugins;
using SmartHome.Network.HomeDevices;
using SmartHome.Messages;

namespace SmartHome.Network
{
    public abstract class ActionAbstract
    {
        public int ID { get; set; }
        public String Name { get; set; }
        public HomeDevice ToHomeDevice { get; set; }
        public OPCode OPCode { get; set; }
        public byte[] Args { get; set; }
        public Boolean Enable { get; set; }

        public virtual void Execute() { }
    }

    public class Action : ActionAbstract
    {
        public List<TimeRestriction> TimeRestrictions { get; set; }
        public override string ToString()
        {
            throw new NotImplementedException();
        }
    }

    public class TimeAction : ActionAbstract
    {
        public byte MascWeekDays { get; set; }
        public DateTime Time;


        public override string ToString()
        {
            throw new NotImplementedException();
        }
    }

    public class TimeRestriction
    {
        public byte MaskWeekDays { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }

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

        public TimeRestriction(byte maskWeekDays, int fromHour, int fromMin, int FromSeg, int ToHour, int ToMin, int ToSeg)
        {
            MaskWeekDays = maskWeekDays;
            Start = new DateTime(1, 1, 1, fromHour, fromMin, FromSeg);
            End = new DateTime(1, 1, 1, ToHour, ToMin, ToSeg);
        }

        public TimeRestriction(byte maskWeekDays, int fromHour, int fromMin, int ToHour, int ToMin)
        {
            MaskWeekDays = maskWeekDays;
            Start = new DateTime(1, 1, 1, fromHour, fromMin, 0);
            End = new DateTime(1, 1, 1, ToHour, ToMin, 0);
        }

        public TimeRestriction(byte maskWeekDays, DateTime fromTime, DateTime toTime)
        {
            MaskWeekDays = maskWeekDays;
            Start = fromTime;
            End = toTime;
        }
    }
}
