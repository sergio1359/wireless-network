using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Extensions;
using DomoticNetwork.NetworkModel;

namespace DomoticNetwork.EEPROM
{
    //firm version 1.0
    class Generator
    {
        public Shield ShieldNode { get; set; }
        public UInt16 NodeAddress { set; get; }
        public Byte Channel { set; get; }
        public UInt16 PanId { set; get; }
        public Byte[] SecurityKey { set; get; }
        private Dictionary<UInt16, BasicEvent> PortEventTimeRestrictionDictionary;

        public Generator(Network Network, UInt16 Address)
        {
            ShieldNode = Network.GetNode(Address).NodeShield;
            NodeAddress = Address;
            Channel = Network.Channel;
            PanId = Network.PanId;
            //SecurityKey = Network.SecurityKey.ToArray<Byte>();
            PortEventTimeRestrictionDictionary = new Dictionary<ushort, BasicEvent>();
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

            memory[1] = sizeMemory.Uint16ToByte(ShieldNode.ShieldBase.LittleEndian)[0];
            memory[2] = sizeMemory.Uint16ToByte(ShieldNode.ShieldBase.LittleEndian)[1];

            //Generar el CRC
            Byte[] crc = new Crc16().ComputeChecksumBytes(memory, ShieldNode.ShieldBase.LittleEndian);

            //sustituimos el valor de CRC que esta en la posicion de memoria 0x02 0x03, no hace falta contar con endianidad
            memory[3] = crc[0];
            memory[4] = crc[1];

            return memory;
        }

        private Byte[] DeviceInfo()
        {
            Byte[] result = new Byte[5];

            result[0] = (byte)ShieldNode.Type;

            //Default Lenght (unknow at the moment)
            result[1] = 0x00;
            result[2] = 0x00;

            //Default CRC16 to 00 00
            result[3] = 0x00;
            result[4] = 0x00;

            return result;
        }

        private Byte[] NetworkConfig()
        {
            List<Byte> result = new List<Byte>();
            result.AddRange(NodeAddress.Uint16ToByte(ShieldNode.ShieldBase.LittleEndian));

            result.Add(Channel);

            result.AddRange(PanId.Uint16ToByte(ShieldNode.ShieldBase.LittleEndian));

            result.AddRange(SecurityKey);

            return result.ToArray();
        }

        private Byte[] PortsIOConfig()
        {
            List<Byte> result = new List<Byte>();

            for (Byte i = 0; i < ShieldNode.ShieldBase.NumPorts; i++)
            {
                result.AddRange(PinIOConfig(i));
            }

            return result.ToArray();
        }

        private Byte[] PinIOConfig(Byte port)
        {
            //Si no esta definido suponemos Entrada digital
            Byte[] result = new Byte[5];
            Connector c = null;
            //Input:0 - Output:1, default=0
            result[0] = 0x00;
            for (byte i = 0; i < ShieldNode.ShieldBase.NumPins; i++)
            {
                c = ShieldNode.GetConector(port, i);
                if (c != null && c.Output == true)
                {
                    result[0] = (byte)(result[0] | (0x01 << i));
                }
            }

            //Analog:0 - Digital:1 default: 1
            result[1] = 0xFF;
            for (byte i = 0; i < ShieldNode.ShieldBase.NumPins; i++)
            {
                c = ShieldNode.GetConector(port, i);
                if (c != null && c.Digital == false)
                {
                    result[1] = (byte)(result[1] & ~(0x01 << i));
                }
            }

            //Input:0 - Output:1, default=0
            result[2] = 0x00;
            for (byte i = 0; i < ShieldNode.ShieldBase.NumPins; i++)
            {
                c = ShieldNode.GetConector(port, i);
                if (c != null && c.DefaultValueD == true)
                {
                    result[2] = (byte)(result[2] | (0x01 << i));
                }
            }

            //ChangetypeD None:00 Rising:10, Fall:01, Both:11
            UInt16 ctd = 0x00; //change type digital
            for (byte i = 0; i < ShieldNode.ShieldBase.NumPins; i++)
            {
                c = ShieldNode.GetConector(port, i);
                if (c != null) ctd = (byte)(ctd | ((byte)c.changeTypeD) << (i * 2));
            }

            result[3] = ctd.Uint16ToByte(ShieldNode.ShieldBase.LittleEndian)[0];
            result[4] = ctd.Uint16ToByte(ShieldNode.ShieldBase.LittleEndian)[1];

            return result;
        }

        private Byte[] PWMConfig()
        {
            List<Byte> result = new List<Byte>();

            foreach (String pin in ShieldNode.ShieldBase.PWMPorts)
            {
                if (ShieldNode.GetConector(pin[0], Convert.ToByte(pin[1])) != null)
                    result.Add(ShieldNode.GetConector(pin[0], Convert.ToByte(pin[1])).DefaultValueA);
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
            Connector c = null;

            foreach (String pin in ShieldNode.ShieldBase.AnalogPorts)
            {
                c = ShieldNode.GetConector(pin[0], Convert.ToByte(pin[1]));
                if (c != null)
                {
                    //Analog Input To Binary
                    result.Add(c.Increment);
                    result.Add(c.Threshold);
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
            UInt16 TableEnableEventsFlagPointer = (UInt16)(result.Count - 2);

            result.AddRange(PortsEvents());

            result.AddRange(TimeEvents());

            result[PortEventTimeRestrictionPointer] = result.Count.Uint16ToByte(ShieldNode.ShieldBase.LittleEndian)[0];
            result[PortEventTimeRestrictionPointer + 1] = result.Count.Uint16ToByte(ShieldNode.ShieldBase.LittleEndian)[1];

            result.AddRange(PortEventTimeRestriction());

            result[TableEnableEventsFlagPointer] = result.Count.Uint16ToByte(ShieldNode.ShieldBase.LittleEndian)[0];
            result[TableEnableEventsFlagPointer + 1] = result.Count.Uint16ToByte(ShieldNode.ShieldBase.LittleEndian)[1];
            
            return result.ToArray();
        }



        private Byte[] TableEventList()
        {
            List<Byte> result = new List<Byte>();
            UInt16 pointer = 0;
            Connector c = null;

            //añadimos la primera direccion, que siempre será 0x00
            result.Add(0x00);

            for (byte i = 0; i < ShieldNode.ShieldBase.NumPorts; i++)
            {
                for (byte j = 0; j < ShieldNode.ShieldBase.NumPins; j++)
                {
                    c = ShieldNode.GetConector(i, j);
                    if (c != null)
                    {
                        pointer += c.SizePortEvents();
                    }
                    result.AddRange(pointer.Uint16ToByte(ShieldNode.ShieldBase.LittleEndian));
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
            Connector c = null;
            for (byte i = 0; i < ShieldNode.ShieldBase.NumPorts; i++)
            {
                for (byte j = 0; j < ShieldNode.ShieldBase.NumPins; j++)
                {
                    c = ShieldNode.GetConector(i, j);
                    if (c != null)
                    {
                        foreach (BasicEvent pe in c.ConnectorEvent)
                        {
                            if (pe.TimeRestriction != null)
                            {
                                PortEventTimeRestrictionDictionary.Add((UInt16)result.Count, pe); //rellenamos el diccionario para el metodo port event time restriction
                            }
                            result.AddRange(pe.Event.ToBinary(ShieldNode.ShieldBase.LittleEndian));
                        }
                    }
                }
            }

            return result.ToArray();
        }

        //hay relacionarlo con el metodo de arriba, tambien tenemos que tener el size
        public Byte[] ToBinaryEvent(bool littleEndian)
        {
            List<Byte> result = new List<byte>();

            result.AddRange(Address.Uint16ToByte(littleEndian));

            result.Add(OPCode);

            result.AddRange(Args);

            return result.ToArray();
        }

        private Byte[] TimeEvents()
        {
            List<Byte> result = new List<Byte>();
            //Organizamos por tiempo
            ShieldNode.TimeEvents.Sort();
            foreach (TimeEvent te in ShieldNode.TimeEvents)
            {
                result.AddRange(te.Time.ToBinaryEEPROM());
                result.AddRange(te.Event.ToBinary(ShieldNode.ShieldBase.LittleEndian));
            }

            return result.ToArray();
        }

        //Hay que ponerlo en el codigo tambien
        public Byte[] ToBinaryTimeEventRestriction()
        {
            List<Byte> result = new List<Byte>();
            result.AddRange(Start.ToBinaryEEPROM());
            result.AddRange(End.ToBinaryEEPROM());
            return result.ToArray();
        }

        private Byte[] PortEventTimeRestriction()
        {
            List<Byte> result = new List<Byte>();

            //for (int i = 0; i < PortEventTimeRestrictionDictionary.Count; i++)
            //{
            //    result.AddRange(PortEventTimeRestrictionDictionary.Keys
            //}

            foreach (KeyValuePair<UInt16, BasicEvent> pe in PortEventTimeRestrictionDictionary)
            {
                result.AddRange(pe.Key.Uint16ToByte(ShieldNode.ShieldBase.LittleEndian));
                result.AddRange(pe.Value.TimeRestriction.Start.ToBinaryEEPROM());
                result.AddRange(pe.Value.TimeRestriction.End.ToBinaryEEPROM());
            }
            return result.ToArray();
        }

        //private Byte[] TableEnableEventsFlags()
        //{
        //    List<Byte> result = new List<Byte>();
        //    byte pointer = 0x00;
        //    Connector c = null;

        //    result.Add(pointer); //el primero siempre es 00

        //    for (byte i = 0; i < ShieldNode.ShieldBase.NumPorts; i++)
        //    {
        //        for (byte j = 0; j < ShieldNode.ShieldBase.NumPins; j++)
        //        {
        //            c = ShieldNode.GetConector(i, j);
        //            if (c != null)
        //            {
        //                pointer += (byte)(c.ConnectorEvent.Count / 8);
        //            }
        //            else
        //            {
        //                pointer++;
        //                result.Add(pointer);
        //            }
        //        }
        //    }

        //    //Add free region
        //    pointer += (byte)(ShieldNode.TimeEvents.Count / 8);
        //    result.Add(pointer);

        //    return result.ToArray();
        //}

        ////OJO REVISAR
        //private Byte[] TableEventsFlags()
        //{
        //    List<Byte> result = new List<Byte>();
        //    Connector c = null;
        //    for (byte i = 0; i < ShieldNode.ShieldBase.NumPorts; i++)
        //    {
        //        for (byte j = 0; j < ShieldNode.ShieldBase.NumPins; j++)
        //        {
        //            c = ShieldNode.GetConector(i, j);
        //            if (c != null)
        //            {
        //                byte toResult = 0x00; //el byte que añadiremos a la memoria

        //                for (int k = 0; k < c.ConnectorEvent.Count; k++)
        //                {

        //                    if (c.ConnectorEvent[k].Enable == true)
        //                    {
        //                        toResult = (byte)(toResult | (0x01 << Math.Abs((k % 8) - 8)));  //NOTA MENTAL: Math.Abs((k % 8) - 8)) es la posicion que ocupa el enable en ese momento
        //                    }

        //                    if (Math.Abs((k % 8) - 8) == 0 || k + 1 == c.ConnectorEvent.Count) // Si hemos agotado la capacidad del byte o estamos en la ultima iteracion del for
        //                    {
        //                        result.Add(toResult);
        //                        toResult = 0x00;
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                result.Add(0x00);
        //            }
        //        }
        //    }

        //    for (int i = 0; i < ShieldNode.TimeEvents.Count; i++)
        //    {
        //        byte toResult = 0x00; //el byte que añadiremos a la memoria

        //        if (ShieldNode.TimeEvents[i].Enable == true)
        //        {
        //            toResult = (byte)(toResult | (0x01 << Math.Abs((i % 8) - 8)));  //NOTA MENTAL: Math.Abs((i % 8) - 8)) es la posicion que ocupa el enable en ese momento
        //        }

        //        if (Math.Abs((i % 8) - 8) == 0 || i + 1 == ShieldNode.TimeEvents.Count) // Si hemos agotado la capacidad del byte o estamos en la ultima iteracion del for
        //        {
        //            result.Add(toResult);
        //            toResult = 0x00;
        //        }
        //    }
        //    return result.ToArray();
        //}
    }
}
