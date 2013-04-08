using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartHome.Enums;

namespace SmartHome.Products
{
    class Base
    {
        //Identificador de micro
        public UInt32 DeviceSignature { private set; get; }
        public Enums.BaseType UController { set; get; }

        //numero de puertos que tiene el micro, y cada puerto tiene NumPins pines
        public UInt16 NumPorts { set; get; }
        public Byte NumPins { set; get; }

        //Esta informacion debe de estar organizada por orden Alfabetico
        public String[] AnalogPorts { set; get; }
        public String[] PWMPorts { set; get; }
        public String[] UnavailablePorts { set; get; }

        //Endianidad
        public Boolean LittleEndian { set; get; }


        public Base(Enums.BaseType controller)
        {
            UController = controller;
            switch (controller)
            {
                case Enums.BaseType.ATMega128RFA1:
                    DeviceSignature = 128;
                    NumPorts = 7;
                    NumPins = 8;

                    AnalogPorts = new String[8] { "F0", "F1", "F2", "F3", "F4", "F5", "F6", "F7" };
                    PWMPorts = new String[8] { "B4", "B5", "B6", "B7", "E3", "E4", "E5", "G5" };  //VERSION MINOLO:{ "B4", "B7", "G5" } el B7 y el G5 estan compartidos con el mismo timer
                    UnavailablePorts = new String[2] { "G3", "G4" };  //TODO: de momento estos pero hay que chequear

                    LittleEndian = true;
                    break;
                default:
                    throw new Exception();
            }
        }

        /// <summary>
        /// Return true if IsAnalog
        /// </summary>
        /// <param name="portPin">Name of port and number of pin, Example: F3, A1</param>
        /// <returns></returns>
        public bool IsAnalog(String portPin)
        {
            return AnalogPorts.Any(x => x == portPin);
        }

        /// <summary>
        /// Return true if IsAnalog
        /// </summary>
        /// <param name="portPin">Name of port and number of pin, Example: F3, A1</param>
        /// <returns></returns>
        public bool IsPWM(String portPin)
        {
            return PWMPorts.Any(x => x == portPin);
        }

        /// <summary>
        /// Return true if available this pin
        /// </summary>
        /// <param name="portPin">Name of port and number of pin, Example: F3, A1</param>
        /// <returns></returns>
        public bool IsAvailabe(String portPin)
        {
            if (Int16.Parse(portPin[1].ToString()) > 7)
                return false;
            if (portPin[0] - 'A' + 1 > NumPorts || portPin[0] - 'A' < 0)
                return false;
            return !UnavailablePorts.Any(x => x == portPin);
        }
    }
}
