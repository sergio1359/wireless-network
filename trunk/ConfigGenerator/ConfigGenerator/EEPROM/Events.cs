using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigGenerator.EEPROM
{
    class Event
    {
        public UInt16 Address;
        public Byte OPCode;
        //[TODO]Argumentos



        public Byte[] ToBinary() 
        { 
            //Byte[] result = new Byte[3 + ]
            throw new NotImplementedException();
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
    }

    //Time Events
    class TimeEvent
    {
        public Event Event { get; set; }
        public DateTime Time { get; set; }
        public Boolean Enable { get; set; }
    }
}
