using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Comunications.Messages
{
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

    public enum LogicWriteValues : byte
    {
        Clear = 0x00,
        Set = 0x01,
        SetRelative = 0x80,
        SetRelativeInverse = 0x81
    }
}
