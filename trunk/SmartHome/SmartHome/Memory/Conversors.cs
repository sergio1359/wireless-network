using DataLayer.Entities;
using DataLayer.Entities.HomeDevices;
using SmartHome.BusinessEntities;
using SmartHome.Communications.Messages;
using SmartHome.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmartHome.Memory
{
    public static class BinaryExtension
    {
        public static string ToStrigMac(this byte[] macAddress)
        {
            StringBuilder result = new StringBuilder();

            foreach (byte b in macAddress)
            {
                result.AppendFormat("{0:X2}", b);
            }

            return result.ToString();
        }

        public static byte[] ToBinaryMac(this string macAddress)
        {
            return Enumerable.Range(0, macAddress.Length)
                     .Where(x => x % 2 == 0)
                     .Select(x => Convert.ToByte(macAddress.Substring(x, 2), 16))
                     .ToArray();
        }

        /// <summary>
        /// Converts a TimeSpan in a Byte[] sort in Hours, Minutes and Seconds
        /// </summary>
        public static byte[] ToBinaryTime(this DateTime time)
        {
            byte[] result = new byte[3];
            result[0] = (byte)time.Hour;
            result[1] = (byte)time.Minute;
            result[2] = (byte)time.Second;

            return result;
        }

        public static byte[] ToBinaryTime(this TimeSpan time)
        {
            byte[] result = new byte[3];
            result[0] = (byte)time.Hours;
            result[1] = (byte)time.Minutes;
            result[2] = (byte)time.Seconds;

            return result;
        }

        public static byte[] ToBinaryDate(this DateTime time, bool littleEndian = true)
        {

            List<Byte> result = new List<Byte>();

            //TODO: Corregir!!
            string value = Enum.GetName(typeof(DayOfWeek), time.DayOfWeek);
            result.Add((byte)Enum.Parse(typeof(WeekDays), value));

            result.Add((byte)time.Day);
            result.Add((byte)time.Month);
            result.AddRange(((ushort)time.Year).UshortToByte(littleEndian));

            return result.ToArray();
        }

        public static byte[] UshortToByte(this ushort b, bool litteEndian = true)
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


        /// <returns>If not exist, null</returns>
        public static Connector GetConnector(this Node node, PinPort pinPort)
        {
            if (!node.GetPinPortList().Contains(pinPort))
            {
                return null;
            }

            return node.Connectors.FirstOrDefault(connector => connector.GetPinPort().Contains(pinPort));
        }
    }

    public partial class FirmwareUno
    {
        private byte GetLogicConfiguration(Connector connector, HomeDevice homeDevice)
        {
            PinPortConfiguration ppc = connector.GetPinPortConfiguration(homeDevice);
            byte res = 0x00;
            if (ppc.Output)
                res |= (byte)(0x01 << 3);
            if (ppc.DefaultValueD)
                res |= (byte)(0x01 << 2);
            res |= (byte)ppc.ChangeTypeD;

            return res;
        }
    }
}
