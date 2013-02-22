using System;

namespace DomoticNetwork.NetworkModel
{
    class Event
    {
        public Byte OPCode { get; set; }
        public Byte[] Args { get; set; }

    }

    //Port Events and Internals Events
    class BasicEvent
    {
        public Event Event { get; set; }
        public TimeRestriction TimeRestriction { get; set; } //If not exist time restriction then null

        public BasicEvent(Event e, Boolean enable)
        {
            Event = e;
            TimeRestriction = null;
        }

        public BasicEvent(Event e, int startHour, int startMinute, int endHour, int endMinute, Boolean enable)
        {
            Event = e;
            TimeRestriction = new TimeRestriction(startHour, startMinute, endHour, endMinute);
        }

    }

    class TimeRestriction
    {
        public DateTime Start;
        public DateTime End;

        public TimeRestriction(int startHour, int startMinute, int endHour, int endMinute)
        {
            Start = new DateTime(0, 0, 0, startHour, startMinute, 0);
            End = new DateTime(0, 0, 0, endHour, endMinute, 0);
        }

    }

    //Time Events
    class TimeEvent : IComparable<TimeEvent>
    {
        public Event Event { get; set; }
        public DateTime Time { get; set; }

        public TimeEvent(Event e, int hour, int minute, Boolean enable)
        {
            Event = e;
            Time = new DateTime(0, 0, 0, hour, minute, 0);
        }

        public int CompareTo(TimeEvent other)
        {
            return Time.CompareTo(other);
        }
    }
}
