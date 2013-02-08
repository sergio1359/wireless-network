using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConfigGenerator;

namespace ConfigGenerator.DeviceModel
{
    class Device
    {
        //Device Info
        public DeviceInfo DeviceInfo { get; set; }
        public Enums.Shields ShieldModel { get; set; }

        //NetworkConfig
        public NetworkConfig Network { get; set; }

        //PortConfig: 0=A, 1=B, 2=C....
        public Port[] Ports { get; set; }

        //Events
        public List<TimeEvent> TimeEvents { get; set; }

        public Device(DeviceInfo device, Enums.Shields type)
        {
            DeviceInfo = device;
            ShieldModel = type;

            //NetworkConfig (sera una configuracion generica)

            Ports = new Port[device.NumPorts];

            for (int i = 0; i < Ports.Length; i++)
            {
                Ports[i] = new Port();
            }

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

        public List<PortEvent> GetPortsEvents(String direction)
        {
            throw new NotImplementedException();
        }

        public Pin GetPin(String direction)
        {
            if (direction.Length == 3)
            {
                direction = direction.Substring(1);
            }

            if(!DeviceInfo.IsAvailabe(direction))
            {
                return null;
            }
            else
            {
                return Ports[direction[0] - 'A'].Pins[Convert.ToByte(direction[1])];
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
        public UInt32 DeviceSignature { set; get; }

        //numero de puertos que tiene el micro, y cada puerto tiene NumPins pines
        public UInt16 NumPorts { set; get; }
        public Byte NumPins { set; get; }

        //Esta informacion debe de estar organizada por orden Alfabetico
        public String[] AnalogPorts { set; get; }
        public String[] PWMPorts { set; get; }
        public String[] UnavailablePorts { set; get; }

        public Boolean LittleEndian { set; get; }

        public DeviceInfo()
        {
            //Ejemplo generado con el ATmega128FRA1
            DeviceSignature = 128;
            NumPorts = 6;
            NumPins = 8;

            AnalogPorts = new String[8] { "F0", "F1", "F2", "F3", "F4", "F5", "F6", "F7" };
            PWMPorts = new String[3] {"B4", "B7", "G5"};  //el B7 y el G5 estan compartidos con el mismo timer
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
