using System;
using System.Collections.Generic;

namespace DomoticNetwork.NetworkModel
{
    class Event
    {
        public Byte OPCode { get; set; }
        public Byte[] Args { get; set; }

        public Event()
        {
            OPCode = 0xFF;
            Args = new Byte[4] { 0x01, 0x02, 0x03, 0x04 };
        }

        public Event(Byte op, Byte[] args)
        {
            OPCode = op;
            Args = args;
        }

        public Event(Enums.OPCodeType type, Byte[] args)
        {
            OPCode = (byte)type;
            Args = args;
        }

    }

    //Port Events and Internals Events
    class BasicEvent
    {
        public Event Event { get; set; }
        public List<TimeRestriction> TimeRestrictions { get; set; }

        public BasicEvent(Event e)
        {
            Event = e;
            TimeRestrictions = new List<TimeRestriction>();
        }

    }

    class TimeRestriction
    {
        public DateTime Start;
        public DateTime End;

        public TimeRestriction(int startHour, int startMinute, int startSecond, int endHour, int endMinute, int endSecond)
        {
            Start = new DateTime(1, 1, 1, startHour, startMinute, startSecond);
            End = new DateTime(1, 1, 1, endHour, endMinute, endSecond);
        }

    }

    //Time Events
    class TimeEvent : IComparable<TimeEvent>
    {
        public Event Event { get; set; }
        public DateTime Time { get; set; }


        public TimeEvent(Event e, int hour, int minute, int second)
        {
            Event = e;
            Time = new DateTime(1, 1, 1, hour, minute, second);
        }

        public int CompareTo(TimeEvent other)
        {
            return Time.CompareTo(other.Time);
        }
    }
}
