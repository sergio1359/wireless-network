using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Products
{
    public class PinPortConfiguration
    {
        public const bool DEFAULT_OUTPUT = false; //Entrada
        public const bool DEFAULT_DIGITAL = true; //Digital

        public Boolean Output { get; set; }
        public Boolean Digital { get; set; }

        public enum Trigger : byte { None = 0x00, FallingEdge = 0x01, RisingEdge = 0x10, Both = 0x11 }

        //Digital-------------------------------------
        //output
        public Boolean DefaultValueD { get; set; }

        //input
        public Trigger changeTypeD { get; set; }


        //Analog-------------------------------------
        //output
        public Byte DefaultValueA { get; set; }

        //input
        public Byte Increment { get; set; }
        public Byte Threshold { get; set; }
    }
}
