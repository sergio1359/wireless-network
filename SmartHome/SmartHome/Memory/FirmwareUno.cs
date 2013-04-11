using SmartHome.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Memory
{
    class FirmwareUno
    {
        private Node node { set; get; }
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

            Byte[] memory = result.ToArray();

            //Calculamos el tamaño del byte
            UInt16 sizeMemory = (UInt16)memory.Length;

            memory[1] = sizeMemory.Uint16ToByte(node.GetBaseConfiguration().LittleEndian)[0];
            memory[2] = sizeMemory.Uint16ToByte(node.GetBaseConfiguration().LittleEndian)[1];

            //Generar el CRC
            Byte[] crc = new Crc16().ComputeChecksumBytes(memory, node.GetBaseConfiguration().LittleEndian);

            //sustituimos el valor de CRC que esta en la posicion de memoria 0x02 0x03, no hace falta contar con endianidad
            memory[3] = crc[0];
            memory[4] = crc[1];

            return memory;
        }

        private Byte[] DeviceInfo()
        {
            List<Byte> result = new List<Byte>();

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
        private Byte[] NetworkConfig()
        {
            List<Byte> result = new List<Byte>();
            result.AddRange(node.Address.Uint16ToByte(node.GetBaseConfiguration().LittleEndian));

            result.Add(NetworkManager.Security.Channel);

            result.AddRange(NetworkManager.Security.PanId.Uint16ToByte(node.GetBaseConfiguration().LittleEndian));

            //result.AddRange(NetworkManager.Security.SecurityKey); HAY QUE HACERLO!

            return result.ToArray();
        }

        private Byte[] PortsIOConfig()
        {
            List<Byte> result = new List<Byte>();

            for (Byte i = 0; i < node.GetBaseConfiguration().NumPorts; i++)
            {
                result.AddRange(PinIOConfig(i));
            }

            return result.ToArray();
        }

        private Byte[] PinIOConfig(Byte port)
        {
            //Si no esta definido suponemos Entrada digital
            Byte[] result = new Byte[5];
            PinPort p = null;
            //Input:0 - Output:1, default=0
            result[0] = 0x00;
            for (byte i = 0; i < node.GetBaseConfiguration().NumPins; i++)
            {
                p = ShieldNode.GetPinPort(port, i);
                if (p != null && p.Output == true)
                {
                    result[0] = (byte)(result[0] | (0x01 << i));
                }
            }

            //Analog:0 - Digital:1 default: 1
            result[1] = 0xFF;
            for (byte i = 0; i < node.GetBaseConfiguration().NumPins; i++)
            {
                p = ShieldNode.GetPinPort(port, i);
                if (p != null && p.Digital == false)
                {
                    result[1] = (byte)(result[1] & ~(0x01 << i));
                }
            }

            //Input:0 - Output:1, default=0
            result[2] = 0x00;
            for (byte i = 0; i < node.GetBaseConfiguration().NumPins; i++)
            {
                p = ShieldNode.GetPinPort(port, i);
                if (p != null && p.DefaultValueD == true)
                {
                    result[2] = (byte)(result[2] | (0x01 << i));
                }
            }

            //ChangetypeD None:00 Rising:10, Fall:01, Both:11
            UInt16 ctd = 0x00; //change type digital
            for (byte i = 0; i < node.GetBaseConfiguration().NumPins; i++)
            {
                p = ShieldNode.GetPinPort(port, i);
                if (p != null) ctd = (byte)(ctd | ((byte)p.changeTypeD) << (i * 2));
            }

            result[3] = ctd.Uint16ToByte(node.GetBaseConfiguration().LittleEndian)[0];
            result[4] = ctd.Uint16ToByte(node.GetBaseConfiguration().LittleEndian)[1];

            return result;
        }

        private Byte[] PWMConfig()
        {
            List<Byte> result = new List<Byte>();

            foreach (String pin in node.GetBaseConfiguration().PWMPorts)
            {
                if (ShieldNode.GetPinPort(pin[0], Byte.Parse(pin[1].ToString())) != null)
                    result.Add(ShieldNode.GetPinPort(pin[0], Byte.Parse(pin[1].ToString())).DefaultValueA);
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
            PinPort p = null;

            foreach (String pin in node.GetBaseConfiguration().AnalogPorts)
            {
                p = ShieldNode.GetPinPort(pin[0], Byte.Parse(pin[1].ToString()));
                if (p != null)
                {
                    //Analog Input To Binary
                    result.Add(p.Increment);
                    result.Add(p.Threshold);
                }
                else
                {
                    result.Add(0x00); //Increment
                    result.Add(0x00); //Threshold
                }
            }

            return result.ToArray();
        }

        //Este metodo genera toda la parte dinamica y parten de el todas las zonas de memorias restantes
        private Byte[] Dinamic()
        {
            List<Byte> result = new List<Byte>();

            result.AddRange(TableEventList());
            UInt16 PortEventTimeRestrictionPointer = (UInt16)(result.Count - 4); //4 posiciones de memoria que tenemos en 0x00
            UInt16 FreeRegionPointer = (UInt16)(result.Count - 2);

            List<Byte> resultPortEvents = new List<byte>();

            resultPortEvents.AddRange(PortsEvents());
            resultPortEvents.AddRange(TimeEvents());

            //port event time restriction
            result[PortEventTimeRestrictionPointer] = resultPortEvents.Count.Uint16ToByte(node.GetBaseConfiguration().LittleEndian)[0];
            result[PortEventTimeRestrictionPointer + 1] = resultPortEvents.Count.Uint16ToByte(node.GetBaseConfiguration().LittleEndian)[1];

            resultPortEvents.AddRange(PortEventTimeRestriction());

            //free region
            result[FreeRegionPointer] = resultPortEvents.Count.Uint16ToByte(node.GetBaseConfiguration().LittleEndian)[0];
            result[FreeRegionPointer + 1] = resultPortEvents.Count.Uint16ToByte(node.GetBaseConfiguration().LittleEndian)[1];

            result.AddRange(resultPortEvents);

            return result.ToArray();
        }



        private Byte[] TableEventList()
        {
            List<Byte> result = new List<Byte>();
            UInt16 pointer = 0;
            PinPort p = null;

            for (byte i = 0; i < node.GetBaseConfiguration().NumPorts; i++)
            {
                for (byte j = 0; j < node.GetBaseConfiguration().NumPins; j++)
                {
                    result.AddRange(pointer.Uint16ToByte(node.GetBaseConfiguration().LittleEndian));
                    p = ShieldNode.GetPinPort(i, j);
                    if (p != null)
                    {
                        pointer += SizePinEvents(p);
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

        private Byte[] PortsEvents()
        {
            List<Byte> result = new List<Byte>();
            Connector c = null;
            for (byte i = 0; i < node.GetBaseConfiguration().NumPorts; i++)
            {
                for (byte j = 0; j < node.GetBaseConfiguration().NumPins; j++)
                {
                    c = ShieldNode.GetConector(i, j);
                    if (c != null)
                    {
                        foreach (PinPort pin in c.Directions)
                        {
                            foreach (BasicEvent e in pin.PinEvents)
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

        private Byte[] ToBinaryEvent(Event e, bool littleEndian)
        {
            List<Byte> result = new List<byte>();

            result.AddRange(node.Address.Uint16ToByte(littleEndian));

            result.Add(e.OPCode);

            result.AddRange(e.Args);

            return result.ToArray();
        }

        private UInt16 SizePinEvents(PinPort pin)
        {
            UInt16 size = 0;
            foreach (BasicEvent pe in pin.PinEvents)
            {
                size += (UInt16)ToBinaryEvent(pe.Event, true).Length;
            }
            return size;
        }

        private Byte[] TimeEvents()
        {
            List<Byte> result = new List<Byte>();
            //Organizamos por tiempo
            ShieldNode.TimeEvents.Sort();
            foreach (TimeEvent te in ShieldNode.TimeEvents)
            {
                result.AddRange(te.Time.ToBinaryTime());
                result.AddRange(ToBinaryEvent(te.Event, node.GetBaseConfiguration().LittleEndian));
            }

            return result.ToArray();
        }

        //private Byte[] ToBinaryTimeEventRestriction(TimeRestriction tr)
        //{
        //    List<Byte> result = new List<Byte>();
        //    result.AddRange(tr.Start.ToBinaryTime());
        //    result.AddRange(tr.End.ToBinaryTime());
        //    return result.ToArray();
        //}

        private Byte[] PortEventTimeRestriction()
        {
            List<Byte> result = new List<Byte>();

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

    }
}
