using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConfigGenerator;

namespace ConfigGenerator.EEPROM
{
    class Device
    {
        //Device Info
        public DeviceInfo DeviceInfo { get; set; }
        public Enums.DeviceType Type { get; set; }

        //NetworkConfig
        public NetworkConfig Network { get; set; }

        //PortConfig: 0=A, 1=B, 2=C....
        Port[] Ports { get; set; }

        //Events
        public List<PortEvent> PortEvents { get; set; }
        public List<TimeEvent> TimeEvents { get; set; }

        public Device(DeviceInfo device, Enums.DeviceType type)
        {
            DeviceInfo = device;
            Type = type;

            //NetworkConfig (sera una configuracion generica)

            Ports = new Port[device.NumPorts];

            for (int i = 0; i < Ports.Length; i++)
            {
                Ports[i] = new Port();
            }

            PortEvents = new List<PortEvent>();
            TimeEvents = new List<TimeEvent>();
        }

        public void AddPin(Pin pin, String direction)
        {
            if (direction.Length == 3)
            {
                direction = direction.Substring(1);
            }
            if ((!pin.Digital && !DeviceInfo.IsAnalog(direction)) || !DeviceInfo.IsAvailabe(direction))
                throw new Exception();
            else
            {
                int por = direction[0] - 'A';
                Ports[por].Pins[Convert.ToByte(direction[1])] = pin;
            }
        }

        public Byte[] ToBinary()
        {
            throw new NotImplementedException();
        }
    }


    class DeviceInfo
    {
        //Identificador de micro
        public UInt16 DeviceID { set; get; }

        //numero de puertos que tiene el micro, cada puerto tiene 8 pines
        public UInt16 NumPorts { set; get; }

        public String[] AnalogPorts { set; get; }
        public String[] PWMPorts { set; get; }
        public String[] UnavailablePorts { set; get; }

        public Boolean LittleEndian { set; get; }

        public DeviceInfo()
        {
            //Ejemplo generado con el ATmega128FRA1
            DeviceID = 128;
            NumPorts = 6;

            AnalogPorts = new String[8] { "F0", "F1", "F2", "F3", "F4", "F5", "F6", "F7" };
            PWMPorts = new String[3] { "B7", "PG5", "B4" };
            UnavailablePorts = new String[0];

            LittleEndian = true;
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
            if (Convert.ToByte(portPin[1]) > 7)
                return false;
            if (portPin[0] - 'A' + 1 < NumPorts  || portPin[0] - 'A' >= 0)
                return false;
            return !UnavailablePorts.Any(x => x == portPin);
        }
    }
}
