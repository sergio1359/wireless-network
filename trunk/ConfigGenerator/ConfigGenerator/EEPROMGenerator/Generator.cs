using ConfigGenerator.DeviceModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Extensions;

namespace ConfigGenerator.EEPROMGenerator
{
    //firm version 1.0
    class Generator
    {
        public Device Device { get; set; }

        public Generator(Device device)
        {
            Device = device;
        }

        public Byte[] GenerateEEPROM()
        {
            List<Byte> result = new List<Byte>();

            //Generar toda la memoria (la memoria se genera con CRC16 a "00 00")

            result.AddRange(DeviceInfo());

            result.AddRange(NetworkConfig());

            result.AddRange(PortsIOConfig());

            result.AddRange(PWMConfig());

            result.AddRange(AnalogInputsConfig());






            Byte[] memory = result.ToArray();

            //Calculamos el tamaño del byte
            UInt16 sizeMemory = (UInt16)memory.Length;

            if (Device.DeviceInfo.LittleEndian)
            {
                memory[1] = (byte)sizeMemory;
                memory[2] = (byte)(sizeMemory >> 8);
            }
            else
            {
                memory[1] = (byte)(sizeMemory >> 8);
                memory[2] = (byte)sizeMemory;
            }

            //Generar el CRC
            Byte[] crc = new Crc16().ComputeChecksumBytes(memory, Device.DeviceInfo.LittleEndian);

            //sustituimos el valor de CRC que esta en la posicion de memoria 0x02 0x03, no hace falta contar con endianidad
            memory[3] = crc[0];
            memory[4] = crc[1];

            return memory;
        }

        private Byte[] DeviceInfo()
        {
            Byte[] result = new Byte[5];

            result[0] = (byte)Device.ShieldModel;

            //Default Lenght (unkown at the moment)
            result[1] = 0x00;
            result[2] = 0x00;

            //Default CRC16 to 00 00
            result[3] = 0x00;
            result[4] = 0x00;

            return result;
        }

        private Byte[] NetworkConfig()
        {
            return Device.Network.ToBinary(Device.DeviceInfo.LittleEndian);
        }

        private Byte[] PortsIOConfig()
        {
            List<Byte> result = new List<Byte>();

            foreach (Port p in Device.Ports)
            {
                result.AddRange(p.PortIOToBinary(Device.DeviceInfo.LittleEndian));
            }

            return result.ToArray();
        }

        private Byte[] PWMConfig()
        {
            List<Byte> result = new List<Byte>();

            foreach (String pin in Device.DeviceInfo.PWMPorts)
            {
                if (Device.GetPin(pin) != null)
                    result.Add(Device.GetPin(pin).PWMToBinary());
                else
                {
                    result.Add(0x00);
                }
            }
            return result.ToArray();
        }

        private Byte[] AnalogInputsConfig()
        {
            List<Byte> result = new List<Byte>();

            foreach (String pin in Device.DeviceInfo.AnalogPorts)
            {
                if (Device.GetPin(pin) != null)
                    result.AddRange(Device.GetPin(pin).AnalogInputToBinary());
                else
                {
                    result.Add(0x00);
                }
            }

            return result.ToArray();
        }

        private Byte[] Dinamic()
        {
            List<Byte> result = new List<Byte>();

            result.AddRange(TableEventList());
            UInt16 TimeEventTablePointer = 


            return result.ToArray();
        }



        private Byte[] TableEventList()
        {
            List<Byte> result = new List<Byte>();
            UInt16 pointer = 0;

            //añadimos la primera direccion, que siempre será 0x00
            result.Add(0x00);

            for (int i = 0; i < Device.DeviceInfo.NumPorts; i++)
            {
                for (int j = 0; j < Device.DeviceInfo.NumPins; j++)
                {
                    if (Device.Ports[i].Pins[j] != null)
                    {
                        pointer += Device.Ports[i].Pins[j].SizePortEvents();
                    }
                    result.AddRange(pointer.Uint16ToByte(Device.DeviceInfo.LittleEndian));
                }
            }

            //direccion lista de eventos de tiempo, ya esta introducida por que basicamente, es la ulitma iteracion del for

            //direccion restricciones temporales de los eventos de puertos OJO!!!
            result.Add(0x00);
            result.Add(0x00);

            //direccion banderas de habilitación de eventos
            result.Add(0x00);
            result.Add(0x00);

            return result.ToArray();
        }

        private Byte[] PortsEvents()
        {
            List<Byte> result = new List<Byte>();
            for (int i = 0; i < Device.DeviceInfo.NumPorts; i++)
            {
                for (int j = 0; j < Device.DeviceInfo.NumPins; j++)
                {
                    if (Device.Ports[i].Pins[j] != null)
                    {
                        foreach (PortEvent pe in Device.Ports[i].Pins[j].PortEvents)
                        {
                            result.AddRange(pe.Event.ToBinary(Device.DeviceInfo.LittleEndian));
                        }
                    }
                }
            }

            return result.ToArray();
        }

        private Byte[] TimeEvents()
        {
            List<Byte> result = new List<Byte>();


            return result.ToArray();
        }

        private Byte[] PortEventTimeRestriction()
        {
            List<Byte> result = new List<Byte>();


            return result.ToArray();
        }

        private Byte[] TableEnableEventsFlags()
        {
            List<Byte> result = new List<Byte>();


            return result.ToArray();
        }

        private Byte[] TableEvensFlags()
        {
            List<Byte> result = new List<Byte>();


            return result.ToArray();
        }

    }



}
