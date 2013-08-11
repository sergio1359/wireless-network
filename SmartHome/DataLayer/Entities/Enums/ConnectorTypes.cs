#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 
#endregion

namespace DataLayer.Entities.Enums
{
    public enum ConnectorTypes
    {
        SwitchLOW,
        SwitchHI,
        PWMTTL,
        Dimmer,
        LogicInput,
        RGB,
        DimmerPassZero,
        ConectorSensorBoard
    }
}
