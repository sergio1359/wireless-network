using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Extensions;

namespace ConfigGenerator.EEPROM
{
    class Event
    {
        public UInt16 Address;
        public Byte OPCode;
        public Byte[] Args;

        public Byte[] ToBinary()
        {
            Byte[] result = new Byte[3 + Args.Length];
            //OJO! BIG LITTLE!            
            result[0] = (byte)Address;
            result[1] = (byte)(Address >> 8);
            result[2] = OPCode;
            for (int i = 0; i < Args.Length; i++)
            {
                result[i + 3] = Args[i];
            }
            return result;
        }
    }

    //Port Events
    class PortEvent
    {
        public Event Event { get; set; }
        public TimeRestriction TimeRestriction { get; set; } //If not exist time restriction then null
        public Boolean Enable { get; set; }
    }

    class TimeRestriction
    {
        public DateTime Start;
        public DateTime End;

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
    }
}
