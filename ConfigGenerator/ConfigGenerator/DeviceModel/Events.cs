using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Extensions;

namespace ConfigGenerator.DeviceModel
{
    class Event
    {
        public UInt16 Address;
        public Byte OPCode;
        public Byte[] Args;

        public Byte[] ToBinary(bool littleEndian)
        {
            Byte[] result = new Byte[3 + Args.Length];

            if (littleEndian)
            {
                result[0] = (byte)Address;
                result[1] = (byte)(Address >> 8);
            }
            else
            {
                result[0] = (byte)(Address >> 8);
                result[1] = (byte)Address;
            }

            result[2] = OPCode;
            for (int i = 0; i < Args.Length; i++)
            {
                result[i + 3] = Args[i];
            }
            return result;
        }

        /// <summary>
        /// Size of Event in Bytes
        /// </summary>
        /// <returns></returns>
        public UInt16 Size()
        {
            return (UInt16) ToBinary(true).Length;
        }
    }

    //Port Events
    class PortEvent
    {
        public Event Event { get; set; }
        public TimeRestriction TimeRestriction { get; set; } //If not exist time restriction then null
        public Boolean Enable { get; set; }

        public PortEvent(Event e, Boolean enable)
        {
            Event = e;
            TimeRestriction = null;
            Enable = enable;
        }

        public PortEvent(Event e, int startHour, int startMinute, int endHour, int endMinute, Boolean enable)
        {
            Event = e;
            TimeRestriction = new TimeRestriction(startHour, startMinute, endHour, endMinute);
            Enable = enable;
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

        public Byte[] ToBinary()
        {
            List<Byte> result = new List<Byte>();
            result.AddRange(Start.ToBinaryEEPROM());
            result.AddRange(End.ToBinaryEEPROM());
            return result.ToArray();
        }
    }

    //Time Events
    class TimeEvent
    {
        public Event Event { get; set; }
        public DateTime Time { get; set; }
        public Boolean Enable { get; set; }

        public TimeEvent(Event e, int hour, int minute, Boolean enable)
        {
            Event = e;
            Time = new DateTime(0, 0, 0, hour, minute, 0);
            Enable = enable;
        }
    }
}
