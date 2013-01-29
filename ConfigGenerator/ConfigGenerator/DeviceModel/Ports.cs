using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigGenerator.DeviceModel
{
    class Port
    {
        public Pin[] Pins { get; set; }

        public Port()
        {
            Pins = new Pin[8]; //suponemos 8 pines por puerto
        }

        public Byte[] PortIOToBinary(bool littleEndian)
        {
            //Si no esta definido suponemos Entrada digital
            Byte[] result = new Byte[5];

            //Input:0 - Output:1, default=0
            result[0] = 0x00;
            for (byte i = 0; i < Pins.Length; i++)
            {
                if (Pins[i] != null && Pins[i].Output == true)
                {
                    result[0] = (byte)(result[0] | (0x01 << i));
                }
            }

            //Analog:0 - Digital:1 default: 1
            result[1] = 0xFF;
            for (byte i = 0; i < Pins.Length; i++)
            {
                if (Pins[i] != null && Pins[i].Digital == false)
                {
                    result[1] = (byte)(result[1] & ~(0x01 << i));
                }
            }

            //Input:0 - Output:1, default=0
            result[2] = 0x00;
            for (byte i = 0; i < Pins.Length; i++)
            {
                if (Pins[i] != null && Pins[i].DefaultValueD == true)
                {
                    result[2] = (byte)(result[2] | (0x01 << i));
                }
            }

            //ChangetypeD None:00 Rising:10, Fall:01, Both:11
            UInt16 ctd = 0x00; //change type digital
            for (byte i = 0; i < Pins.Length; i++)
            {
                if (Pins[i] != null) ctd = (byte)(ctd | ((byte)Pins[i].changeTypeD) << (i * 2));
            }

            if (littleEndian)
            {
                result[3] = (byte)ctd;
                result[4] = (byte)(ctd >> 8);
            }
            else
            {
                result[3] = (byte)(ctd >> 8);
                result[4] = (byte)ctd;
            }

            return result;
        }

        public Byte[] AllPWMToBinary()
        {
            Byte[] result = new Byte[8];
            for (int i = 0; i < Pins.Length; i++)
            {
                if (Pins[i] != null)
                    result[i] = this.Pins[i].DefaultValueA;
                else
                    result[i] = 0x00;
            }

            return result;
        }

        public Byte[] AllAnalogInputToBinary()
        {
            Byte[] result = new Byte[16];

            for (int i = 0; i < Pins.Length; i++)
            {
                if (Pins[i] != null)
                    result[i] = this.Pins[i].Increment;
                else
                    result[i] = 0x00;
            }

            for (int i = 0; i < Pins.Length; i++)
            {
                if (Pins[i] != null)
                    result[i + 8] = this.Pins[i].Threshold;
                else
                    result[i + 8] = 0x00;
            }

            return result;
        }

    }


    class Pin
    {
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




        public Byte PWMToBinary()
        {
            return DefaultValueA;
        }

        public Byte[] AnalogInputToBinary()
        {
            Byte[] result = new Byte[2];
            
            result[0] = Increment;
            result[1] = Threshold;

            return result;
        }

    }
}
