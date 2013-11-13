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
        private Node _node;
        private Dictionary<ushort, Operation[]> _operationDictionary;
        private Dictionary<ushort, Operation> _operationTimeRestictionDictionary;
        private List<ushort> _dinamicIndex;
        private byte _systemFlags;
        private List<byte> _tempMemory;
        private Base _baseConfiguration;

        public FirmwareUno(Node node, byte systemFlags)
        {
            _node = node;
            _operationTimeRestictionDictionary = new Dictionary<ushort, Operation>();
            _operationDictionary = new Dictionary<ushort, Operation[]>();
            _systemFlags = systemFlags;
            _baseConfiguration = node.GetBaseConfiguration();
        }


        /// <summary>
        /// Generates the EEPROM configuration.
        /// </summary>
        /// <param name="home">The home.</param>
        /// <returns>A tuple containing the checksum (Item1) and the raw bytes (Item2)</returns>
        public Tuple<ushort, byte[]> GenerateEEPROM(Home home)
        {
            _tempMemory = new List<Byte>();

            //Generar toda la memoria (la memoria se genera con CRC16 a "00 00")

            //ConfigHeader
            _tempMemory.AddRange(ConfigHeader());

            //NetworkConfig
            _tempMemory.AddRange(NetworkConfig(home));

            ushort pointerStartDinamicIndex = (ushort)_tempMemory.Count;
            _dinamicIndex = new List<ushort>();

            //fill with 00 in memory for Dinamic Index
            for (int i = 0; i < 13; i++)
            {
                _tempMemory.Add(0x00);
                _tempMemory.Add(0x00);
            }

            //DevicesConfig
            DevicesConfig();

            //TimeOperation
            _dinamicIndex.Add((ushort)_tempMemory.Count);
            _tempMemory.AddRange(TimeOperation());

            //Operation
            _dinamicIndex.Add((ushort)_tempMemory.Count);
            _tempMemory.AddRange(Operation((ushort)_tempMemory.Count));

            //OperationTimeRestriction
            _dinamicIndex.Add((ushort)_tempMemory.Count);
            _tempMemory.AddRange(OperationTimeRestriction());

            //OperationConditionRestrictions
            _dinamicIndex.Add((ushort)_tempMemory.Count);
            //TODO

            //free region
            _dinamicIndex.Add((ushort)_tempMemory.Count);

            //change directions of DinamicIndex using the DinamicIndexList
            DinamicIndex(pointerStartDinamicIndex);
            //DINAMIC END

            byte[] memory = _tempMemory.ToArray();

            //Calculamos el tamaño en bytes
            ushort sizeMemory = (ushort)memory.Length;

            memory[3] = sizeMemory.UshortToByte(_baseConfiguration.LittleEndian)[0];
            memory[4] = sizeMemory.UshortToByte(_baseConfiguration.LittleEndian)[1];

            //Generar el CRC
            byte[] crc = new Crc16().ComputeChecksumBytes(memory, _baseConfiguration.LittleEndian);

            //sustituimos el valor de CRC que esta en la posicion de memoria 0x05 0x06, no hace falta contar con endianidad
            memory[5] = crc[0];
            memory[6] = crc[1];

            ushort crcResult = BitConverter.ToUInt16(crc, 0);

            return new Tuple<ushort,byte[]>(crcResult, memory);
        }

        private byte[] ConfigHeader()
        {
            List<byte> result = new List<byte>();

            //base model
            result.Add((byte)this._node.Base);

            //firmware version = 1
            result.Add(0x01);

            //shield mode
            result.Add((byte)this._node.Shield);

            //Default Lenght (unknow at the moment)
            result.Add(0x00);
            result.Add(0x00);

            //Default CRC16 to 00 00
            result.Add(0x00);
            result.Add(0x00);

            //updateDate
            result.AddRange(DateTime.Now.ToBinaryDate(_baseConfiguration.LittleEndian));

            //updateTime
            result.AddRange(DateTime.Now.ToBinaryTime());

            //systemFlags
            result.Add(_systemFlags);

            return result.ToArray();
        }

        //Cogemos por defecto la configuration del NetworkManager
        private byte[] NetworkConfig(Home home)
        {
            List<byte> result = new List<byte>();

            //deviceAddress
            result.AddRange(((ushort)_node.Address).UshortToByte(_baseConfiguration.LittleEndian));

            ////chanel
            result.Add((byte)home.Security.Channel);

            ////panID
            result.AddRange(((ushort)home.Security.PanId).UshortToByte(_baseConfiguration.LittleEndian));

            ////securityKey
            result.AddRange(home.Security.GetSecurityKey());

            //networkRetriesLimit
            result.Add((byte)_node.NetworkRetries);

            return result.ToArray();
        }

        private void DevicesConfig()
        {
            //Logic
            _dinamicIndex.Add((ushort)_tempMemory.Count);
            _tempMemory.AddRange(DeviceConfigLogic());

            //Dimmable
            _dinamicIndex.Add((ushort)_tempMemory.Count);
            _tempMemory.AddRange(DeviceConfigDimmable());

            //RGB
            _dinamicIndex.Add((ushort)_tempMemory.Count);
            _tempMemory.AddRange(DeviceConfigRGB());

            //Presence
            _dinamicIndex.Add((ushort)_tempMemory.Count);
            _tempMemory.AddRange(DeviceConfigPresence());

            //Temperature
            _dinamicIndex.Add((ushort)_tempMemory.Count);
            _tempMemory.AddRange(DeviceConfigTemperature());

            //Humidity
            _dinamicIndex.Add((ushort)_tempMemory.Count);
            _tempMemory.AddRange(DeviceConfigHumidity());

            //EnergyMeter
            _dinamicIndex.Add((ushort)_tempMemory.Count);
            _tempMemory.AddRange(DeviceConfigPowerMeter());

            //Luminosity
            _dinamicIndex.Add((ushort)_tempMemory.Count);
            _tempMemory.AddRange(DeviceConfigLuminosity());
        }

        private void DinamicIndex(ushort startDinamicIndexMemory)
        {
            ushort i = startDinamicIndexMemory;
            foreach (var item in _dinamicIndex)
            {
                _tempMemory[i++] = item.UshortToByte(_baseConfiguration.LittleEndian)[0];
                _tempMemory[i++] = item.UshortToByte(_baseConfiguration.LittleEndian)[1];
            }
        }

        private byte[] TimeOperation()
        {
            List<byte> result = new List<byte>();

            var timeActions = _node.GetTimeActions();

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

            foreach (var item in _operationDictionary)
            {
                //añadimos direccion a la configuracion del config configuration
                _tempMemory[item.Key] = ((ushort)(result.Count + startDirection)).UshortToByte(_baseConfiguration.LittleEndian)[0];
                _tempMemory[item.Key + 1] = ((ushort)(result.Count + startDirection)).UshortToByte(_baseConfiguration.LittleEndian)[1];

                foreach (var a in item.Value)
                {
                    if (a.TimeRestrictions != null && a.TimeRestrictions.Count > 0)
                        _operationTimeRestictionDictionary.Add((ushort)(result.Count + startDirection), a);

                    result.AddRange(a.ToBinaryOperation());
                }
            }
            return result.ToArray();
        }

        private byte[] OperationTimeRestriction()
        {
            List<byte> result = new List<byte>();

            foreach (KeyValuePair<ushort, Operation> pe in _operationTimeRestictionDictionary)
            {
                foreach (TimeRestriction tr in pe.Value.TimeRestrictions)
                {
                    result.AddRange(pe.Key.UshortToByte(_baseConfiguration.LittleEndian));
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

            foreach (HomeDevice item in _node.HomeDevices)
            {
                if (item is Button || item is SwitchButton || item is WallPlug || item is Light)
                {
                    //pointerOperation
                    _operationDictionary.Add((ushort)(_tempMemory.Count + result.Count), item.Operations.ToArray());
                    result.Add(0x00);
                    result.Add(0x00);
                    result.Add((byte)item.Operations.Count);

                    //ConfigList
                    result[0]++;
                    result.AddRange(((ushort)item.Id).UshortToByte(_baseConfiguration.LittleEndian));
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

            PinPort dimmableZero = ProductConfiguration.GetDimmerPassZeroPinPort(this._node.Shield);

            if ( dimmableZero != null)
                result.Add(dimmableZero.GetPinPortNumber());
            else
                result.Add(0x00); //TODO: If not exits. Value default??

            
            foreach (HomeDevice item in _node.HomeDevices)
            {
                if (item is Dimmable)
                {
                    //pointerOperation
                    _operationDictionary.Add((ushort)(_tempMemory.Count + result.Count), item.Operations.ToArray());
                    result.Add(0x00);
                    result.Add(0x00);
                    result.Add((byte)item.Operations.Count);

                    //ConfigList
                    result[0]++;
                    result.AddRange(((ushort)item.Id).UshortToByte(_baseConfiguration.LittleEndian));
                    result.Add(item.Connector.GetPinPort()[0].GetPinPortNumber());
                }
            }
            return result.ToArray();
        }

        private byte[] DeviceConfigRGB()
        {
            List<byte> result = new List<byte>();
            result.Add(0x00);
            foreach (HomeDevice item in _node.HomeDevices)
            {
                if (item is RGBLight)
                {
                    //pointerOperation
                    _operationDictionary.Add((ushort)(_tempMemory.Count + result.Count), item.Operations.ToArray());
                    result.Add(0x00);
                    result.Add(0x00);
                    result.Add((byte)item.Operations.Count);

                    //ConfigList
                    result[0]++;
                    result.AddRange(((ushort)item.Id).UshortToByte(_baseConfiguration.LittleEndian));
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
            foreach (HomeDevice item in _node.HomeDevices)
            {
                if (item is PresenceSensor)
                {
                    //pointerOperation
                    _operationDictionary.Add((ushort)(_tempMemory.Count + result.Count), item.Operations.ToArray());
                    result.Add(0x00);
                    result.Add(0x00);
                    result.Add((byte)item.Operations.Count);

                    //ConfigList
                    result[0]++;
                    result.AddRange(((ushort)item.Id).UshortToByte(_baseConfiguration.LittleEndian));
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
            foreach (HomeDevice item in _node.HomeDevices)
            {
                if (item is TemperatureSensor)
                {
                    //pointerOperation
                    _operationDictionary.Add((ushort)(_tempMemory.Count + result.Count), item.Operations.ToArray());
                    result.Add(0x00);
                    result.Add(0x00);
                    result.Add((byte)item.Operations.Count);

                    //ConfigList
                    result[0]++;
                    result.AddRange(((ushort)item.Id).UshortToByte(_baseConfiguration.LittleEndian));
                    result.Add(item.Connector.GetPinPort()[0].GetPinPortNumber());
                }
            }
            return result.ToArray();
        }

        private byte[] DeviceConfigHumidity()
        {
            List<byte> result = new List<byte>();

            result.Add(0x00);
            foreach (HomeDevice item in _node.HomeDevices)
            {
                if (item is HumiditySensor)
                {
                    //pointerOperation
                    _operationDictionary.Add((ushort)(_tempMemory.Count + result.Count), item.Operations.ToArray());
                    result.Add(0x00);
                    result.Add(0x00);
                    result.Add((byte)item.Operations.Count);

                    //ConfigList
                    result[0]++;
                    result.AddRange(((ushort)item.Id).UshortToByte(_baseConfiguration.LittleEndian));
                    result.Add(item.Connector.GetPinPort()[0].GetPinPortNumber());
                }
            }
            return result.ToArray();
        }

        private byte[] DeviceConfigPowerMeter()
        {
            List<byte> result = new List<byte>();

            result.Add(0x00);
            foreach (HomeDevice item in _node.HomeDevices)
            {
                if (item is PowerSensor)
                {
                    //pointerOperation
                    _operationDictionary.Add((ushort)(_tempMemory.Count + result.Count), item.Operations.ToArray());
                    result.Add(0x00);
                    result.Add(0x00);
                    result.Add((byte)item.Operations.Count);

                    //ConfigList
                    result[0]++;
                    result.AddRange(((ushort)item.Id).UshortToByte(_baseConfiguration.LittleEndian));
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
            foreach (HomeDevice item in _node.HomeDevices)
            {
                if (item is LuminositySensor)
                {
                    //pointerOperation
                    _operationDictionary.Add((ushort)(_tempMemory.Count + result.Count), item.Operations.ToArray());
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
