using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DomoticNetwork.NetworkModel
{
    class Shield
    {
        public Base ShieldBase { set; get; }
        public Enums.ShieldType Type { set; get; }

        public Conector[] Conectors { set; get; }

        public List<TimeEvent> TimeEvents { set; get; }


        public Conector GetConector(Char port, Byte pin)
        {
            return Conectors.First<Conector>(x => x.Pin == pin && x.Port == port);
        }

        public Conector GetConector(Byte port, Byte pin)
        {
            return Conectors.First<Conector>(x => x.Pin == pin && (x.Port - 'A') == port);
        }

    }

    class Base
    {
        //Identificador de micro
        public UInt32 DeviceSignature { set; get; }
        public Enums.UControllerType UController { set; get; }

        //numero de puertos que tiene el micro, y cada puerto tiene NumPins pines
        public UInt16 NumPorts { set; get; }
        public Byte NumPins { set; get; }

        //Esta informacion debe de estar organizada por orden Alfabetico
        public String[] AnalogPorts { set; get; }
        public String[] PWMPorts { set; get; }
        public String[] UnavailablePorts { set; get; }

        //Endianidad
        public Boolean LittleEndian { set; get; }
    }

    class Conector
    {
        public Enums.ConectorType Type { set; get; }

        //Pseudonimo
        public String name { set; get; }

        //Direction
        public Char Port { set; get; }
        public Byte Pin { set; get; }

        //Type
        public List<PortEvent> ConnectorEvent { set; get; }

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


        public UInt16 SizePortEvents()
        {
            UInt16 size = 0;
            foreach (PortEvent pe in ConnectorEvent)
            {
                size += pe.Event.Size();
            }
            return size;
        }
    }
}
