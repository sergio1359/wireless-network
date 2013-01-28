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

        public void AddPort(Port port)
        {
            throw new NotImplementedException();



        }

        public Boolean TryAddPort(Port port)
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

            AnalogPorts = new String[8] { "PF0", "PF1", "PF2", "PF3", "PF4", "PF5", "PF6", "PF7" };
            //PWMPorts = 
            //UnavailablePorts ==

            LittleEndian = true;
        }

        //ESTO TENEMOS QUE VER COMO LO VAMOS A HACER
        public bool IsAnalog(String portPin)
        {
            return AnalogPorts.Any(x => x == portPin);
        }

        public bool IsPWM(String portPin)
        {
            return PWMPorts.Any(x => x == portPin);
        }

        public bool IsAvailabe(String portPin)
        {
            //ojo!! num ports!
            return !UnavailablePorts.Any(x => x == portPin);
        }
    }
}
