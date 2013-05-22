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

        public bool Output { get; set; }
        public bool Digital { get; set; }

        public enum Trigger : byte { None = 0x00, FallingEdge = 0x01, RisingEdge = 0x10, Both = 0x11 }

        //Digital-------------------------------------
        //output
        public bool DefaultValueD { get; set; }

        //input
        public Trigger ChangeTypeD { get; set; }


        //Analog-------------------------------------
        //output
        public byte DefaultValueA { get; set; }

        //input
        public byte Increment { get; set; }
        public byte Threshold { get; set; }


    }
}
