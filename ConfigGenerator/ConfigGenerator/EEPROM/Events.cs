using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigGenerator.EEPROM
{
    class Event
    {
        //no definido aun
    }

    //Port Events
    class PortEvent
    {
        Event Event { get; set; }
        TimeRestriction TimeRestriction { get; set; } //If not exist time restriction then null
        Boolean Enable { get; set; }
    }

    class TimeRestriction
    {
        DateTime Start;
        DateTime End;
    }

    //Time Events
    class TimeEvent
    {
        Event Event { get; set; }
        DateTime Time { get; set; }
        Boolean Enable { get; set; }
    }
}
