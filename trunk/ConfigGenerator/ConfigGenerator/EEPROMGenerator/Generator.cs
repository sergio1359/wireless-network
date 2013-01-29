using ConfigGenerator.DeviceModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigGenerator.EEPROMGenerator
{
    class Generator
    {
        public Device Device { get; set; }
        public Byte FirmVersion { get; set; }

        public Generator(Device device)
        {
            Device = device;
            FirmVersion = 0x01;
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

            //Generar el CRC
            Byte[] crc = new Crc16().ComputeChecksumBytes(memory, Device.DeviceInfo.LittleEndian);

            //sustituimos el valor de CRC que esta en la posicion de memoria 0x02 0x03, no hace falta contar con endianidad
            memory[2] = crc[0];
            memory[3] = crc[1];

            return memory;
        }

        private Byte[] DeviceInfo()
        {
            Byte[] result = new Byte[4];

            result[0] = (byte)Device.Type;
            result[1] = FirmVersion;

            //Default CRC16 to 00 00
            result[2] = 0x00;
            result[3] = 0x00;

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

    }



}
