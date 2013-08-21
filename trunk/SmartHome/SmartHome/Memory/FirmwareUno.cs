#region Using Statements
using DataLayer.Entities;
using DataLayer.Entities.Enums;
using DataLayer.Entities.HomeDevices;
using SmartHome.BusinessEntities;
using SmartHome.Products;
using System;
using System.Collections.Generic;
using System.Linq;
#endregion

namespace SmartHome.Memory
{
    public partial class FirmwareUno
    {
        private Node node;
        private Dictionary<ushort, Operation[]> OperationDictionary;
        private Dictionary<ushort, Operation> OperationTimeRestictionDictionary;
        private Dictionary<ushort, Operation> OperationConditionRestrictionsDictionary;
        private List<ushort> dinamicIndex;
        private byte systemFlags;
        private List<byte> tempMemory;
        private Base baseConfiguration;

        public FirmwareUno(Node node, byte systemFlags)
        {
            this.node = node;
            OperationTimeRestictionDictionary = new Dictionary<ushort, Operation>();
            OperationConditionRestrictionsDictionary = new Dictionary<ushort, Operation>();
            OperationDictionary = new Dictionary<ushort, Operation[]>();
            this.systemFlags = systemFlags;
            baseConfiguration = node.GetBaseConfiguration();
        }


        public byte[] GenerateEEPROM()
        {
            tempMemory = new List<Byte>();

            //Generar toda la memoria (la memoria se genera con CRC16 a "00 00")

            //ConfigHeader
            tempMemory.AddRange(ConfigHeader());

            //NetworkConfig
            tempMemory.AddRange(NetworkConfig());

            ushort pointerStartDinamicIndex = (ushort)tempMemory.Count;
            dinamicIndex = new List<ushort>();

            //fill with 00 in memory for Dinamic Index
            for (int i = 0; i < 13; i++)
            {
                tempMemory.Add(0x00);
                tempMemory.Add(0x00);
            }

            //DevicesConfig
            DevicesConfig();

            //TimeOperation
            dinamicIndex.Add((ushort)tempMemory.Count);
            tempMemory.AddRange(TimeOperation());

            //Operation
            dinamicIndex.Add((ushort)tempMemory.Count);
            tempMemory.AddRange(Operation((ushort)tempMemory.Count));

            //OperationTimeRestriction
            dinamicIndex.Add((ushort)tempMemory.Count);
            tempMemory.AddRange(OperationTimeRestriction());

            //OperationConditionRestrictions
            dinamicIndex.Add((ushort)tempMemory.Count);
            //TODO

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

        private byte[] ConfigHeader()
        {
            List<byte> result = new List<byte>();

            //firmware version = 1
            result.Add(0x01);

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

            return result.ToArray();
        }

        //Cogemos por defecto la configuration del NetworkManager
        private byte[] NetworkConfig()
        {
            List<byte> result = new List<byte>();

            //deviceAddress
            result.AddRange(((ushort)node.Address).UshortToByte(baseConfiguration.LittleEndian));
            
            
            //TODO!!!!


            ////chanel
            //result.Add(Home.Security.Channel);

            ////panID
            //result.AddRange(Home.Security.PanId.UshortToByte(baseConfiguration.LittleEndian));

            ////securityKey
            //result.AddRange(Home.Security.GetSecurityKey());

            //networkRetriesLimit
            result.Add((byte)node.NetworkRetries);

            return result.ToArray();
        }

        private void DevicesConfig()
        {
            //Logic
            dinamicIndex.Add((ushort)tempMemory.Count);
            tempMemory.AddRange(DeviceConfigLogic());

            //Dimmable
            dinamicIndex.Add((ushort)tempMemory.Count);
            tempMemory.AddRange(DeviceConfigDimmable());

            //RGB
            dinamicIndex.Add((ushort)tempMemory.Count);
            tempMemory.AddRange(DeviceConfigRGB());

            //Presence
            dinamicIndex.Add((ushort)tempMemory.Count);
            tempMemory.AddRange(DeviceConfigPresence());

            //Temperature
            dinamicIndex.Add((ushort)tempMemory.Count);
            tempMemory.AddRange(DeviceConfigTemperature());

            //Humidity
            dinamicIndex.Add((ushort)tempMemory.Count);
            tempMemory.AddRange(DeviceConfigHumidity());

            //EnergyMeter
            dinamicIndex.Add((ushort)tempMemory.Count);
            tempMemory.AddRange(DeviceConfigPowerMeter());

            //Luminosity
            dinamicIndex.Add((ushort)tempMemory.Count);
            tempMemory.AddRange(DeviceConfigLuminosity());
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

        private byte[] TimeOperation()
        {
            List<byte> result = new List<byte>();

            var timeActions = node.GetTimeActions();

            foreach (var item in timeActions)
            {
                result.Add(item.MaskWeekDays);
                result.AddRange(item.Time.ToBinaryTime());
                result.AddRange(item.Operation.ToBinaryOperation());
            }

            return result.ToArray();
        }

        private byte[] Operation(ushort startDirection)
        {
            List<byte> result = new List<byte>();

            foreach (var item in OperationDictionary)
            {
                //añadimos direccion a la configuracion del config configuration
                tempMemory[item.Key] = ((ushort)(result.Count + startDirection)).UshortToByte(baseConfiguration.LittleEndian)[0];
                tempMemory[item.Key + 1] = ((ushort)(result.Count + startDirection)).UshortToByte(baseConfiguration.LittleEndian)[1];

                foreach (var a in item.Value)
                {
                    if (a.TimeRestrictions != null && a.TimeRestrictions.Count > 0)
                        OperationTimeRestictionDictionary.Add((ushort)(result.Count + startDirection), a);

                    result.AddRange(a.ToBinaryOperation());
                }
            }
            return result.ToArray();
        }

        private byte[] OperationTimeRestriction()
        {
            List<byte> result = new List<byte>();

            foreach (KeyValuePair<ushort, Operation> pe in OperationTimeRestictionDictionary)
            {
                foreach (TimeRestriction tr in pe.Value.TimeRestrictions)
                {
                    result.AddRange(pe.Key.UshortToByte(baseConfiguration.LittleEndian));
                    result.Add(tr.MaskWeekDays);
                    result.AddRange(tr.TimeStart.ToBinaryTime());
                    result.AddRange(tr.TimeEnd.ToBinaryTime());
                }
            }
            return result.ToArray();
        }

        private byte[] DeviceConfigLogic()
        {
            List<byte> result = new List<byte>();
            result.Add(0x00);

            foreach (HomeDevice item in node.HomeDevices)
            {
                if (item is Button || item is SwitchButton || item is WallPlug || item is Light)
                {
                    //pointerOperation
                    OperationDictionary.Add((ushort)(tempMemory.Count + result.Count), item.Operations.ToArray());
                    result.Add(0x00);
                    result.Add(0x00);
                    result.Add((byte)item.Operations.Count);

                    //ConfigList
                    result[0]++;
                    result.AddRange(((ushort)item.Id).UshortToByte(baseConfiguration.LittleEndian));
                    result.Add(item.Connector.GetPinPort()[0].GetPinPortNumber());
                    result.Add(GetLogicConfiguration(item.Connector, item));
                }

            }
            return result.ToArray();
        }

        private byte[] DeviceConfigDimmable()
        {
            List<byte> result = new List<byte>();
            result.Add(0x00);

            PinPort dimmableZero = ProductConfiguration.GetDimmerPassZeroPinPort(this.node.Shield);

            if ( dimmableZero != null)
                result.Add(dimmableZero.GetPinPortNumber());
            else
                result.Add(0x00); //TODO: If not exits. Value default??

            
            foreach (HomeDevice item in node.HomeDevices)
            {
                if (item is Dimmable)
                {
                    //pointerOperation
                    OperationDictionary.Add((ushort)(tempMemory.Count + result.Count), item.Operations.ToArray());
                    result.Add(0x00);
                    result.Add(0x00);
                    result.Add((byte)item.Operations.Count);

                    //ConfigList
                    result[0]++;
                    result.AddRange(((ushort)item.Id).UshortToByte(baseConfiguration.LittleEndian));
                    result.Add(item.Connector.GetPinPort()[0].GetPinPortNumber());
                }
            }
            return result.ToArray();
        }

        private byte[] DeviceConfigRGB()
        {
            List<byte> result = new List<byte>();
            result.Add(0x00);
            foreach (HomeDevice item in node.HomeDevices)
            {
                if (item is RGBLight)
                {
                    //pointerOperation
                    OperationDictionary.Add((ushort)(tempMemory.Count + result.Count), item.Operations.ToArray());
                    result.Add(0x00);
                    result.Add(0x00);
                    result.Add((byte)item.Operations.Count);

                    //ConfigList
                    result[0]++;
                    result.AddRange(((ushort)item.Id).UshortToByte(baseConfiguration.LittleEndian));
                    foreach (var pinPort in item.Connector.GetPinPort())
                    {
                        result.Add(pinPort.GetPinPortNumber());
                    }
                }
            }
            return result.ToArray();
        }

        private byte[] DeviceConfigPresence()
        {
            List<byte> result = new List<byte>();
            result.Add(0x00);
            foreach (HomeDevice item in node.HomeDevices)
            {
                if (item is PresenceSensor)
                {
                    //pointerOperation
                    OperationDictionary.Add((ushort)(tempMemory.Count + result.Count), item.Operations.ToArray());
                    result.Add(0x00);
                    result.Add(0x00);
                    result.Add((byte)item.Operations.Count);

                    //ConfigList
                    result[0]++;
                    result.AddRange(((ushort)item.Id).UshortToByte(baseConfiguration.LittleEndian));
                    result.Add(item.Connector.GetPinPort()[0].GetPinPortNumber());
                    result.Add((byte)((PresenceSensor)item).Sensibility);
                }
            }
            return result.ToArray();
        }

        private byte[] DeviceConfigTemperature()
        {
            List<byte> result = new List<byte>();
            result.Add(0x00);
            foreach (HomeDevice item in node.HomeDevices)
            {
                if (item is TemperatureSensor)
                {
                    //pointerOperation
                    OperationDictionary.Add((ushort)(tempMemory.Count + result.Count), item.Operations.ToArray());
                    result.Add(0x00);
                    result.Add(0x00);
                    result.Add((byte)item.Operations.Count);

                    //ConfigList
                    result[0]++;
                    result.AddRange(((ushort)item.Id).UshortToByte(baseConfiguration.LittleEndian));
                    result.Add(item.Connector.GetPinPort()[0].GetPinPortNumber());
                }
            }
            return result.ToArray();
        }

        private byte[] DeviceConfigHumidity()
        {
            List<byte> result = new List<byte>();

            result.Add(0x00);
            foreach (HomeDevice item in node.HomeDevices)
            {
                if (item is HumiditySensor)
                {
                    //pointerOperation
                    OperationDictionary.Add((ushort)(tempMemory.Count + result.Count), item.Operations.ToArray());
                    result.Add(0x00);
                    result.Add(0x00);
                    result.Add((byte)item.Operations.Count);

                    //ConfigList
                    result[0]++;
                    result.AddRange(((ushort)item.Id).UshortToByte(baseConfiguration.LittleEndian));
                    result.Add(item.Connector.GetPinPort()[0].GetPinPortNumber());
                }
            }
            return result.ToArray();
        }

        private byte[] DeviceConfigPowerMeter()
        {
            List<byte> result = new List<byte>();

            result.Add(0x00);
            foreach (HomeDevice item in node.HomeDevices)
            {
                if (item is PowerSensor)
                {
                    //pointerOperation
                    OperationDictionary.Add((ushort)(tempMemory.Count + result.Count), item.Operations.ToArray());
                    result.Add(0x00);
                    result.Add(0x00);
                    result.Add((byte)item.Operations.Count);

                    //ConfigList
                    result[0]++;
                    result.AddRange(((ushort)item.Id).UshortToByte(baseConfiguration.LittleEndian));
                    result.Add(item.Connector.GetPinPort()[0].GetPinPortNumber());
                    result.Add((byte)((PowerSensor)item).Sensibility);
                }
            }

            return result.ToArray();
        }

        private byte[] DeviceConfigLuminosity()
        {
            List<byte> result = new List<byte>();

            result.Add(0x00);
            foreach (HomeDevice item in node.HomeDevices)
            {
                if (item is LuminositySensor)
                {
                    //pointerOperation
                    OperationDictionary.Add((ushort)(tempMemory.Count + result.Count), item.Operations.ToArray());
                    result.Add(0x00);
                    result.Add(0x00);
                    result.Add((byte)item.Operations.Count);

                    //ConfigList
                    result[0]++;
                    result.Add(item.Connector.GetPinPort()[0].GetPinPortNumber());
                    result.Add((byte)((LuminositySensor)item).Sensibility);
                }
            }
            return result.ToArray();
        }

    }
}
