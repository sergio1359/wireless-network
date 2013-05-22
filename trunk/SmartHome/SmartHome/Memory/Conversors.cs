using SmartHome.Network;
using SmartHome.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Memory
{
    public static class BinaryExtension
    {
        /// <summary>
        /// Transform a DateTime in a Byte[] sort in Hours, Minutes and Seconds
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static byte[] ToBinaryTime(this DateTime time)
        {
            Byte[] result = new Byte[3];
            result[0] = (byte)time.Hour;
            result[1] = (byte)time.Minute;
            result[2] = (byte)time.Second;

            return result;
        }

        public static byte[] ToBinaryDate(this DateTime time, bool littleEndian)
        {

            List<Byte> result = new List<Byte>();

            result.Add((byte)time.Day);
            result.Add((byte)time.Month);
            result.AddRange(time.Year.Uint16ToByte(littleEndian));

            return result.ToArray();
        }

        public static byte[] Uint16ToByte(this UInt16 b, bool litteEndian)
        {
            Byte[] result = new Byte[2];

            if (litteEndian)
            {
                result[0] = (byte)b;
                result[1] = (byte)(b >> 8);
            }
            else
            {
                result[0] = (byte)(b >> 8);
                result[1] = (byte)b;
            }

            return result;
        }

        public static byte[] Uint16ToByte(this int b, bool litteEndian)
        {
            Byte[] result = new Byte[2];

            if (litteEndian)
            {
                result[0] = (byte)b;
                result[1] = (byte)(b >> 8);
            }
            else
            {
                result[0] = (byte)(b >> 8);
                result[1] = (byte)b;
            }

            return result;
        }

        //Some adds to easy the conversion
        public static List<PinPort> GetPinPortList(this Node node)
        {
            var dic = ProductConfiguration.GetShieldDictionary(node.Shield);
            List<PinPort> result = new List<PinPort>();

            foreach (var item in dic.Values)
            {
                result.AddRange(item);
            }

            return result;
        }

        //esto es para generar la memoria. Devuelve NULL si no tiene configuracion
        public static PinPortConfiguration GetPinPortConfiguration(this Node node, PinPort pinport)
        {
            Connector con = node.GetConnector(pinport);
            if (con == null)
            {
                return ProductConfiguration.DefaultPinPortConfiguration();
            }
            else
            {
                if (con.HomeDevice != null)
                {
                    return ProductConfiguration.GetPinPortConfiguration(con.HomeDevice.HomeDeviceType);
                }
            }

            return ProductConfiguration.DefaultPinPortConfiguration();
        }


        /// <summary>
        /// Devuelve el Connector que tiene el pinPort Asociado, si no existe ningun conector entonces null
        /// </summary>
        /// <param name="node"></param>
        /// <param name="pinPort"></param>
        /// <returns></returns>
        public static Connector GetConnector(this Node node, PinPort pinPort)
        {
            if (!node.GetPinPortList().Contains(pinPort))
            {
                return null;
            }
            else
            {
                foreach (Connector connector in node.Connectors)
                {
                    if (connector.GetPinPort().Contains(pinPort))
                        return connector;
                }
            }
            return null;
        }
    }

    public partial class FirmwareUno
    {
        private byte[] PinIOConfig(char port)
        {
            byte[] result = new Byte[5];
            PinPortConfiguration p = null;
            
            result[0] = 0x00;
            for (byte i = 0; i < node.GetBaseConfiguration().NumPins; i++)//Input:0 - Output:1, default=0
            {
                p = node.GetPinPortConfiguration(new PinPort(port, i));
                if (p.Output == true)
                    result[0] = (byte)(result[0] | (0x01 << i));
            }

            
            result[1] = 0xFF;
            for (byte i = 0; i < node.GetBaseConfiguration().NumPins; i++)//Analog:0 - Digital:1 default: 1
            {
                p = node.GetPinPortConfiguration(new PinPort(port, i));
                if (p.Digital == false)
                    result[1] = (byte)(result[1] & ~(0x01 << i));
            }

            result[2] = 0x00;
            for (byte i = 0; i < node.GetBaseConfiguration().NumPins; i++)//Input:0 - Output:1, default=0
            {
                p = node.GetPinPortConfiguration(new PinPort(port, i));
                if (p.DefaultValueD == true)
                    result[2] = (byte)(result[2] | (0x01 << i));
            }

            //ChangetypeD None:00 Rising:10, Fall:01, Both:11
            UInt16 ctd = 0x00; //change type digital
            for (byte i = 0; i < node.GetBaseConfiguration().NumPins; i++)
            {
                p = node.GetPinPortConfiguration(new PinPort(port, i));
                ctd = (byte)(ctd | ((byte)p.ChangeTypeD) << (i * 2));
            }

            result[3] = ctd.Uint16ToByte(node.GetBaseConfiguration().LittleEndian)[0];
            result[4] = ctd.Uint16ToByte(node.GetBaseConfiguration().LittleEndian)[1];

            return result;
        }

        private byte[] ToBinaryEvent(ActionAbstract act, bool littleEndian)
        {
            List<byte> result = new List<byte>();

            result.AddRange(act.ToHomeDevice.Connector.Node.Address.Uint16ToByte(littleEndian));
            result.Add((byte)act.OPCode);
            result.AddRange(act.Args);

            return result.ToArray();
        }

        private UInt16 SizePinEvents(SmartHome.Network.Action[] actions)
        {
            UInt16 size = 0;

            foreach (SmartHome.Network.Action act in actions)
                size += (UInt16)ToBinaryEvent(act, true).Length;

            return size;
        }
    }
}
