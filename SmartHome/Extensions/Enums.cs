using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Extensions.Enums
{
    public class Enums
    {
        public enum ConnectorType
        {
            SwitchLOW,
            SwitchHI,
            PWMTTL,
            Dimmer,
            IOLogic,
        }



        public enum BaseType
        {
            ATMega128RFA1,
        }



        public enum OPCode
        {
            DigitalWrite,
            DigitalSwitch,
            DigitalWriteTime,
            DigitalSwitchTime,
            DigitalRead,
            DigitalReadResponse,
            AnalogWrite,
            AnalogWriteTime,
            AnalogRead,
            AnalogReadResponse,
            Reset,
            RouteTableRead,
            RouteTableReadResponse,
            TimeWrite,
            TimeRead,
            TimeReponse,
            Warning,
            Error,
            ColorWrite,
            ColorWhiteRandom,
            ColorSecuenceWrite,
            ColorSortedSecuenceWrite,
            ColorReadResponse,
        }


        public enum LogCategory
        {
            All,
            Info,
            Error,
            Config,
            Network,
            Other,
        }

        public enum PlugState
        {
        }


        public enum ShieldType
        {
            Roseta,
            Central,
            Lampara,
            Regleta,
        }



    }
}
