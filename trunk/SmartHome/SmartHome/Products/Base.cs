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

        //Endianidad
        public Boolean LittleEndian { set; get; }

        /// <summary>
        /// Return true if IsAnalog
        /// </summary>
        /// <param name="portPin">Name of port and number of pin, Example: F3, A1</param>
        /// <returns></returns>
        public bool IsAnalog(string portPin)
        {
            return AnalogPorts.Any(x => x == portPin);
        }

        /// <summary>
        /// Return true if IsAnalog
        /// </summary>
        /// <param name="portPin">Name of port and number of pin, Example: F3, A1</param>
        /// <returns></returns>
        public bool IsPWM(string portPin)
        {
            return PWMPorts.Any(x => x == portPin);
        }

        /// <summary>
        /// Return true if available this pin
        /// </summary>
        /// <param name="portPin">Name of port and number of pin, Example: F3, A1</param>
        /// <returns></returns>
        public bool IsAvailabe(string portPin)
        {
            if (Int16.Parse(portPin[1].ToString()) > 7)
                return false;
            if (portPin[0] - 'A' + 1 > NumPorts || portPin[0] - 'A' < 0)
                return false;
            return !UnavailablePorts.Any(x => x == portPin);
        }
    }
}
