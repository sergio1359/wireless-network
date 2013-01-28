using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigGenerator
{
    public static class Enums
    {
        //Tipo Central: 0x00
        //Tipo Roseta: 0x01
        //Tipo Regleta: 0x02
        //Tipo Lampara: 0x03
        public enum DeviceType: byte{Central=0x00, Roseta=0x01, Regleta=0x02, Lampara=0x03};
    }
}
