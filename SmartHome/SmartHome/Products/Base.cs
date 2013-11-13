#region Using Statements
using DataLayer.Entities.Enums;
using System;
using System.Linq;
using DataLayer.Entities; 
#endregion

namespace SmartHome.Products
{
    public class Base
    {
        //Identificador de micro
        public UInt32 DeviceSignature { set; get; }
        public BaseTypes UController { set; get; }

        //numero de puertos que tiene el micro, y cada puerto tiene NumPins pines
        public UInt16 NumPorts { set; get; }
        public Byte NumPins { set; get; }

        //Esta informacion debe de estar organizada por orden Alfabetico
        public string[] AnalogPorts { set; get; }
        public string[] PWMPorts { set; get; }
        public string[] UnavailablePorts { set; get; }

        public Boolean LittleEndian { set; get; }
    }
}
