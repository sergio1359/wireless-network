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
        public Object Args { get; set; }
        public Boolean Enable { get; set; }

        public virtual void Execute(){}
    }

    public class Action : ActionAbstract
    {
        
        public override string ToString()
        {
            return base.ToString();
        }
    }

    public class TimeAction : ActionAbstract
    {
        public DateTime Time;

        public override string ToString()
        {
            return base.ToString();
        }
    }

    public class TimeRestriction
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }

        public TimeRestriction(int fromHour, int fromMin, int FromSeg, int ToHour, int ToMin, int ToSeg)
        {
            Start = new DateTime(1, 1, 1, fromHour, fromMin, FromSeg);
            End = new DateTime(1, 1, 1, ToHour, ToMin, ToSeg);
        }

        public TimeRestriction(int fromHour, int fromMin, int ToHour, int ToMin)
        {
            Start = new DateTime(1, 1, 1, fromHour, fromMin, 0);
            End = new DateTime(1, 1, 1, ToHour, ToMin, 0);
        }

        public TimeRestriction(DateTime fromTime, DateTime toTime)
        {
            Start = fromTime;
            End = toTime;
        }
    }
}
