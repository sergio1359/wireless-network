using SmartHome.Network;
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
        private Dictionary<UInt16, List<TimeRestriction>> PortEventTimeRestrictionDictionary;

        public FirmwareUno(Node node)
        {
            this.node = node;
            PortEventTimeRestrictionDictionary = new Dictionary<ushort, List<TimeRestriction>>();
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

            result.AddRange(Dinamic());

            byte[] memory = result.ToArray();

            //Calculamos el tamaño del byte
            UInt16 sizeMemory = (UInt16)memory.Length;

            memory[1] = sizeMemory.Uint16ToByte(node.GetBaseConfiguration().LittleEndian)[0];
            memory[2] = sizeMemory.Uint16ToByte(node.GetBaseConfiguration().LittleEndian)[1];

            //Generar el CRC
            byte[] crc = new Crc16().ComputeChecksumBytes(memory, node.GetBaseConfiguration().LittleEndian);

            //sustituimos el valor de CRC que esta en la posicion de memoria 0x02 0x03, no hace falta contar con endianidad
            memory[3] = crc[0];
            memory[4] = crc[1];

            return memory;
        }

        private byte[] DeviceInfo()
        {
            List<byte> result = new List<byte>();

            result.Add((byte)node.Shield);

            //Default Lenght (unknow at the moment)
            result.Add(0x00);
            result.Add(0x00);

            //Default CRC16 to 00 00
            result.Add(0x00);
            result.Add(0x00);

            //updateDate
            result.AddRange(DateTime.Now.ToBinaryDate(node.GetBaseConfiguration().LittleEndian));

            //updateTime
            result.AddRange(DateTime.Now.ToBinaryTime());

            return result.ToArray();
        }

        //Cogemos por defecto la configuration del NetworkManager
        private byte[] NetworkConfig()
        {
            List<byte> result = new List<byte>();

            //deviceAddress
            result.AddRange(node.Address.Uint16ToByte(node.GetBaseConfiguration().LittleEndian));

            //chanel
            result.Add(NetworkManager.Security.Channel);

            //panID
            result.AddRange(NetworkManager.Security.PanId.Uint16ToByte(node.GetBaseConfiguration().LittleEndian));

            //securityKey
            result.AddRange(NetworkManager.Security.GetSecurityKey());

            return result.ToArray();
        }

        private Byte[] PortsIOConfig()
        {
            List<byte> result = new List<byte>();

            for (byte i = 0; i < node.GetBaseConfiguration().NumPorts; i++)
            {
                result.AddRange(PinIOConfig((char)(i + 'A')));
            }

            return result.ToArray();
        }

        

        private byte[] PWMConfig()
        {
            List<byte> result = new List<Byte>();

            foreach (String pinPort in node.GetBaseConfiguration().PWMPorts)
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

            foreach (string pinPort in node.GetBaseConfiguration().AnalogPorts)
            {
                p = node.GetPinPortConfiguration(new PinPort(pinPort));
                result.Add(p.Increment);
                result.Add(p.Threshold);
            }

            return result.ToArray();
        }

        //Este metodo genera toda la parte dinamica y parten de el todas las zonas de memorias restantes
        private byte[] Dinamic()
        {
            List<byte> result = new List<byte>();

            result.AddRange(TableEventList());
            UInt16 PortEventTimeRestrictionPointer = (UInt16)(result.Count - 4); //4 posiciones de memoria que tenemos en 0x00
            UInt16 FreeRegionPointer = (UInt16)(result.Count - 2);

            List<byte> resultPortEvents = new List<byte>();

            resultPortEvents.AddRange(PortsOperations());
            resultPortEvents.AddRange(TimeOperations());

            //port event time restriction
            result[PortEventTimeRestrictionPointer] = resultPortEvents.Count.Uint16ToByte(node.GetBaseConfiguration().LittleEndian)[0];
            result[PortEventTimeRestrictionPointer + 1] = resultPortEvents.Count.Uint16ToByte(node.GetBaseConfiguration().LittleEndian)[1];

            resultPortEvents.AddRange(PortOperationTimeRestriction());



            //Tables Module Config


            //Module config




            //free region
            result[FreeRegionPointer] = resultPortEvents.Count.Uint16ToByte(node.GetBaseConfiguration().LittleEndian)[0];
            result[FreeRegionPointer + 1] = resultPortEvents.Count.Uint16ToByte(node.GetBaseConfiguration().LittleEndian)[1];

            result.AddRange(resultPortEvents);

            return result.ToArray();
        }

        private Byte[] TableEventList()
        {
            List<byte> result = new List<byte>();
            UInt16 pointer = 0;

            for (byte i = 0; i < node.GetBaseConfiguration().NumPorts; i++)
            {
                for (byte j = 0; j < node.GetBaseConfiguration().NumPins; j++)
                {
                    result.AddRange(pointer.Uint16ToByte(node.GetBaseConfiguration().LittleEndian));
                    if (node.GetPinPortList().Contains(new PinPort(i, j)))
                    {
                        Connector connector = node.GetConnector(new PinPort(i, j));

                        pointer += SizePinEvents(connector.GetActionsConnector());
                    }

                }
            }

            //direccion lista de eventos de tiempo
            result.AddRange(pointer.Uint16ToByte(node.GetBaseConfiguration().LittleEndian));

            //direccion restricciones temporales de los eventos de puertos OJO!!!
            result.Add(0x00);
            result.Add(0x00);

            //direccion region libre
            result.Add(0x00);
            result.Add(0x00);

            return result.ToArray();
        }

        private byte[] PortsOperations()
        {
            List<byte> result = new List<byte>();
            Connector c = null;
            for (byte i = 0; i < node.GetBaseConfiguration().NumPorts; i++)
            {
                for (byte j = 0; j < node.GetBaseConfiguration().NumPins; j++)
                {
                    c = node.GetConnector(new PinPort(i, j));
                    if (c != null)
                    {
                        foreach (PinPort pinPort in c.GetPinPort())
                        {
                            foreach (BasicEvent e in pinPort.PinEvents)
                            {
                                if (e.TimeRestrictions.Count > 0)
                                {
                                    PortEventTimeRestrictionDictionary.Add((UInt16)result.Count, e.TimeRestrictions);
                                }
                                result.AddRange(ToBinaryEvent(e.Event, ShieldNode.ShieldBase.LittleEndian));
                            }

                        }
                    }
                }
            }

            return result.ToArray();
        }

        private byte[] TimeOperations()
        {
            List<byte> result = new List<byte>();
            //Organizamos por tiempo
            TimeAction[] timeAct = node.GetTimeActions();
            
            foreach (TimeAction te in timeAct)
            {
                result.AddRange(te.Time.ToBinaryTime());
                result.AddRange(ToBinaryEvent((ActionAbstract)te, node.GetBaseConfiguration().LittleEndian));
            }

            return result.ToArray();
        }

        private byte[] PortOperationTimeRestriction()
        {
            List<byte> result = new List<byte>();

            foreach (KeyValuePair<UInt16, List<TimeRestriction>> pe in PortEventTimeRestrictionDictionary)
            {
                foreach (TimeRestriction tr in pe.Value)
                {
                    result.AddRange(pe.Key.Uint16ToByte(node.GetBaseConfiguration().LittleEndian));
                    result.AddRange(tr.Start.ToBinaryTime());
                    result.AddRange(tr.End.ToBinaryTime());
                }

            }
            return result.ToArray();
        }


        private byte[] TablesModuleConfig()
        {
            return new byte[0];
            throw new NotImplementedException();
        }

        private byte[] ModuleConfig()
        {
            return new byte[0];
            throw new NotImplementedException();
        }

    }
}
