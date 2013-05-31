using SmartHome.Network;
using SmartHome.Network.HomeDevices;
using SmartHome.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Memory
{
    public partial class FirmwareUno
    {
        private Node node;
        private Dictionary<ushort, SmartHome.Network.Action> portOperationTimeRestictionDictionary;
        private List<ushort> dinamicIndex;
        private byte systemFlags;
        private List<byte> tempMemory;
        private Base baseConfiguration;

        public FirmwareUno(Node node, byte systemFlags)
        {
            this.node = node;
            portOperationTimeRestictionDictionary = new Dictionary<ushort, SmartHome.Network.Action>();
            this.systemFlags = systemFlags;
            baseConfiguration = node.GetBaseConfiguration();
        }


        public byte[] GenerateEEPROM()
        {
            tempMemory = new List<Byte>();

            //Generar toda la memoria (la memoria se genera con CRC16 a "00 00")

            tempMemory.AddRange(DeviceInfo());

            tempMemory.AddRange(NetworkConfig());

            tempMemory.AddRange(PortsIOConfig());

            tempMemory.AddRange(PWMConfig());

            tempMemory.AddRange(AnalogInputsConfig());

            ushort pointerStartDinamicIndex = (ushort)tempMemory.Count;
            dinamicIndex = new List<ushort>();

            //fill with 00 in memory for Dinamic Index
            for (int i = 0; i < 10; i++)
            {
                tempMemory.Add(0x00);
                tempMemory.Add(0x00);
            }

            //PortOperationIndex
            dinamicIndex.Add((ushort)tempMemory.Count);
            tempMemory.AddRange(PortOperationIndex((ushort)tempMemory.Count));

            //PortsOperation
            dinamicIndex.Add((ushort)tempMemory.Count);
            tempMemory.AddRange(PortsOperation((ushort)tempMemory.Count));

            //PortOperationTimeRestriction
            dinamicIndex.Add((ushort)tempMemory.Count);
            tempMemory.AddRange(PortOperationTimeRestriction());

            //TimeOperation
            dinamicIndex.Add((ushort)tempMemory.Count);
            tempMemory.AddRange(TimeOperation());

            //Module Config
            tempMemory.AddRange(ModuleConfig((ushort)tempMemory.Count));

            //free region
            dinamicIndex.Add((ushort)tempMemory.Count);

            //change directions of DinamicIndex using the DinamicIndexList
            DinamicIndex(pointerStartDinamicIndex);
            //DINAMIC END


            byte[] memory = tempMemory.ToArray();

            //Calculamos el tamaño en bytes
            ushort sizeMemory = (ushort)memory.Length;

            memory[1] = sizeMemory.UshortToByte(baseConfiguration.LittleEndian)[0];
            memory[2] = sizeMemory.UshortToByte(baseConfiguration.LittleEndian)[1];

            //Generar el CRC
            byte[] crc = new Crc16().ComputeChecksumBytes(memory, baseConfiguration.LittleEndian);

            //sustituimos el valor de CRC que esta en la posicion de memoria 0x02 0x03, no hace falta contar con endianidad
            memory[3] = crc[0];
            memory[4] = crc[1];

            return memory;
        }

        private byte[] DeviceInfo()
        {
            List<byte> result = new List<byte>();

            result.Add((byte)node.Base);

            //Default Lenght (unknow at the moment)
            result.Add(0x00);
            result.Add(0x00);

            //Default CRC16 to 00 00
            result.Add(0x00);
            result.Add(0x00);

            //updateDate
            result.AddRange(DateTime.Now.ToBinaryDate(baseConfiguration.LittleEndian));

            //updateTime
            result.AddRange(DateTime.Now.ToBinaryTime());

            //systemFlags
            result.Add(systemFlags);

            //networkRetriesLimit
            result.Add(node.NetworkRetries);

            return result.ToArray();
        }

        //Cogemos por defecto la configuration del NetworkManager
        private byte[] NetworkConfig()
        {
            List<byte> result = new List<byte>();

            //deviceAddress
            result.AddRange(node.Address.UshortToByte(baseConfiguration.LittleEndian));

            //chanel
            result.Add(NetworkManager.Security.Channel);

            //panID
            result.AddRange(NetworkManager.Security.PanId.UshortToByte(baseConfiguration.LittleEndian));

            //securityKey
            result.AddRange(NetworkManager.Security.GetSecurityKey());

            return result.ToArray();
        }

        private byte[] PortsIOConfig()
        {
            List<byte> result = new List<byte>();

            for (byte i = 0; i < baseConfiguration.NumPorts; i++)
            {
                result.AddRange(PinIOConfig((char)(i + 'A')));
            }

            return result.ToArray();
        }

        private byte[] PWMConfig()
        {
            List<byte> result = new List<Byte>();

            foreach (String pinPort in baseConfiguration.PWMPorts)
            {
                if (node.GetPinPortConfiguration(new PinPort(pinPort)) != null)
                    result.Add(node.GetPinPortConfiguration(new PinPort(pinPort)).DefaultValueA);
                else
                {
                    result.Add(0x00);
                }
            }
            return result.ToArray();
        }

        private byte[] AnalogInputsConfig()
        {
            List<byte> result = new List<byte>();
            PinPortConfiguration p = null;

            foreach (string pinPort in baseConfiguration.AnalogPorts)
            {
                p = node.GetPinPortConfiguration(new PinPort(pinPort));
                result.Add(p.Increment);
                result.Add(p.Threshold);
            }

            return result.ToArray();
        }

        private void DinamicIndex(ushort startDinamicIndexMemory)
        {
            ushort i = startDinamicIndexMemory;
            foreach (var item in dinamicIndex)
            {
                tempMemory[i++] = item.UshortToByte(baseConfiguration.LittleEndian)[0];
                tempMemory[i++] = item.UshortToByte(baseConfiguration.LittleEndian)[1];
            }
        }

        private byte[] PortOperationIndex(ushort startMemoryDir)
        {
            List<byte> result = new List<byte>();
            ushort pointer = (ushort)(startMemoryDir + (baseConfiguration.NumPorts * baseConfiguration.NumPins * 2));

            for (byte i = 0; i < baseConfiguration.NumPorts; i++)
            {
                for (byte j = 0; j < baseConfiguration.NumPins; j++)
                {
                    result.AddRange(pointer.UshortToByte(baseConfiguration.LittleEndian));
                    if (node.GetPinPortList().Contains(new PinPort(i, j)))
                    {
                        Connector connector = node.GetConnector(new PinPort(i, j));

                        if (connector.ConnectorType != ConnectorType.RGB)
                            pointer += SizePinEvents(connector.GetActionsConnector());
                        else
                        {
                            if (new PinPort(i, j) == connector.GetPinPort()[0])
                                pointer += SizePinEvents(connector.GetActionsConnector()); //si es RGB y estamos en su R entonces añadimos puntero con las acciones
                        }
                    }
                }
            }

            return result.ToArray();
        }

        private byte[] PortsOperation(ushort startDirection)
        {
            List<byte> result = new List<byte>();
            Connector c = null;
            for (byte i = 0; i < baseConfiguration.NumPorts; i++)
            {
                for (byte j = 0; j < baseConfiguration.NumPins; j++)
                {
                    c = node.GetConnector(new PinPort(i, j));
                    if (c != null && new PinPort(i, j).Equals(c.GetPinPort()[0]))
                    {
                        foreach (var a in c.GetActionsConnector())
                        {
                            if (a.TimeRestrictions != null && a.TimeRestrictions.Count > 0)
                                portOperationTimeRestictionDictionary.Add((ushort)(result.Count + startDirection), a);

                            result.AddRange(ToBinaryOperation(a, baseConfiguration.LittleEndian));
                        }
                    }
                }
            }

            return result.ToArray();
        }

        private byte[] PortOperationTimeRestriction()
        {
            List<byte> result = new List<byte>();

            foreach (KeyValuePair<ushort, SmartHome.Network.Action> pe in portOperationTimeRestictionDictionary)
            {

                foreach (TimeRestriction tr in pe.Value.TimeRestrictions)
                {
                    result.AddRange(pe.Key.UshortToByte(baseConfiguration.LittleEndian));
                    result.Add(tr.MaskWeekDays);
                    result.AddRange(tr.Start.ToBinaryTime());
                    result.AddRange(tr.End.ToBinaryTime());
                }

            }
            return result.ToArray();
        }

        private byte[] TimeOperation()
        {
            List<byte> result = new List<byte>();
            //TODO: Organizamos por tiempo
            TimeAction[] timeAct = node.GetTimeActions().OrderBy(x => x.Time).ToArray();

            foreach (TimeAction te in timeAct)
            {
                result.Add(te.MascWeekDays);
                result.AddRange(te.Time.ToBinaryTime());
                result.AddRange(ToBinaryOperation((ActionAbstract)te, baseConfiguration.LittleEndian));
            }

            return result.ToArray();
        }

        private byte[] ModuleConfig(ushort directionMemory)
        {
            List<byte> result = new List<byte>();
            //RGB
            dinamicIndex.Add((ushort)tempMemory.Count);
            tempMemory.AddRange(ModuleConfigRGB());

            //Presencia
            dinamicIndex.Add((ushort)tempMemory.Count);
            tempMemory.AddRange(ModuleConfigPresence());

            //Temp/Hum
            dinamicIndex.Add((ushort)tempMemory.Count);
            tempMemory.AddRange(ModuleConfigTemperatureHumidity());

            //Potencia
            dinamicIndex.Add((ushort)tempMemory.Count);
            tempMemory.AddRange(ModuleConfigPower());

            //IntensidadLuminica
            dinamicIndex.Add((ushort)tempMemory.Count);
            tempMemory.AddRange(ModuleConfigLuminosity());

            return result.ToArray();
        }

        private byte[] ModuleConfigRGB()
        {
            List<byte> result = new List<byte>();
            result.Add(0x00);
            foreach (Connector item in node.Connectors)
            {
                if (item.HomeDevice != null)
                {
                    if (item.HomeDevice.HomeDeviceType == HomeDeviceType.RGBLight)
                    {
                        result[0]++;
                        foreach (var pinPort in item.GetPinPort())
                        {
                            result.Add(pinPort.GetPinPortNumber());
                        }
                    }
                }
            }
            return result.ToArray();
        }

        private byte[] ModuleConfigPresence()
        {
            List<byte> result = new List<byte>();
            result.Add(0x00);
            foreach (Connector item in node.Connectors)
            {
                if (item.HomeDevice != null)
                {
                    if (item.HomeDevice.HomeDeviceType == HomeDeviceType.PresenceSensor)
                    {
                        result[0]++;
                        result.Add(item.GetPinPort()[0].GetPinPortNumber());
                        result.Add(((PresenceSensor)item.HomeDevice).Sensibility);
                    }
                }
            }
            return result.ToArray();
        }

        private byte[] ModuleConfigTemperatureHumidity()
        {
            List<byte> result = new List<byte>();

            result.Add(0x00);
            foreach (Connector item in node.Connectors)
            {
                if (item.HomeDevice != null)
                {
                    if (item.HomeDevice.HomeDeviceType == HomeDeviceType.TemperatureHumiditySensor)
                    {
                        result[0]++;
                        result.Add(item.GetPinPort()[0].GetPinPortNumber());
                    }
                }
            }

            return result.ToArray();
        }

        private byte[] ModuleConfigPower()
        {
            List<byte> result = new List<byte>();

            result.Add(0x00);
            foreach (Connector item in node.Connectors)
            {
                if (item.HomeDevice != null)
                {
                    if (item.HomeDevice.HomeDeviceType == HomeDeviceType.PowerSensor)
                    {
                        result[0]++;
                        result.Add(item.GetPinPort()[0].GetPinPortNumber());
                        result.Add(((PowerSensor)item.HomeDevice).Sensibility);
                    }
                }
            }

            return result.ToArray();
        }

        private byte[] ModuleConfigLuminosity()
        {
            List<byte> result = new List<byte>();

            result.Add(0x00);
            foreach (Connector item in node.Connectors)
            {
                if (item.HomeDevice != null)
                {
                    if (item.HomeDevice.HomeDeviceType == HomeDeviceType.LuminositySensor)
                    {
                        result[0]++;
                        result.Add(item.GetPinPort()[0].GetPinPortNumber());
                        result.Add(((LuminositySensor)item.HomeDevice).Sensibility);
                    }
                }
            }

            return result.ToArray();
        }

    }
}
