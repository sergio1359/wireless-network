using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomoticNetwork
{
    public static class Enums
    {
        //Tipo Central: 0x00
        //Tipo Roseta: 0x01
        //Tipo Regleta: 0x02
        //Tipo Lampara: 0x03
        public enum ShieldType : byte { Central = 0x00, Roseta = 0x01, Regleta = 0x02, Lampara = 0x03 };


        public enum PinType { Digital, Analog, PWM };


        public enum ConectorType { Dimmer, Rele, PWMTTL, IODigital, Analog };


        public enum HomeDeviceType { Light, RGBLight};


        public enum UControllerType { ATMega128RFA1 };
    }
}
