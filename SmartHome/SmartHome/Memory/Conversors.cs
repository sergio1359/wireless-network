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

            string value = Enum.GetName(typeof(DayOfWeek), time.DayOfWeek);
            result.Add((byte)Enum.Parse(typeof(WeekDays), value));

            result.Add((byte)time.Day);
            result.Add((byte)time.Month);
            result.AddRange(((ushort)time.Year).UshortToByte(littleEndian));

            return result.ToArray();
        }

        public static byte[] UshortToByte(this ushort b, bool litteEndian)
        {
            byte[] result = new Byte[2];

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
                result.AddRange(item.Item2);
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
        private byte GetLogicConfiguration(Connector hd)
        {
            PinPortConfiguration ppc = hd.GetPinPortConfiguration();
            byte res = 0x00;
            if (ppc.Output)
                res |= (byte)(0x01 << 3);
            if (ppc.DefaultValueD)
                res |= (byte)(0x01 << 2);
            res |= (byte)ppc.Threshold;

            return res;
        }

        private byte[] ToBinaryOperation(Operation operation, bool littleEndian)
        {
            List<byte> result = new List<byte>();

            //TODO: SourceAddress, ojo con las request (tendremos que volver aqui)
            result.Add(0x00);
            result.Add(0x00);

            //DestinationAddress
            if (operation.DestionationHomeDevice.Connector != null && operation.DestionationHomeDevice.Connector.Node.Address == node.Address)
            {
                result.Add(0x00);
                result.Add(0x00);
            }
            else
            {
                result.AddRange(operation.DestionationHomeDevice.Connector.Node.Address.UshortToByte(littleEndian));
            }

            result.Add((byte)operation.OPCode);
            result.AddRange(operation.Args);

            return result.ToArray();
        }

        private UInt16 SizePinEvents(Operation[] operations)
        {
            UInt16 size = 0;

            foreach (Operation act in operations)
                size += (UInt16)ToBinaryOperation(act, true).Length;

            return size;
        }
    }
}
