using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Extensions
{
    public static class Extension
    {
        /// <summary>
        /// Transform a DateTime in a Byte[] sort in Hours, Minutes and Seconds
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static Byte[] ToBinaryEEPROM(this DateTime time)
        {
            Byte[] result = new Byte[3];
            result[0] = (byte)time.Hour;
            result[1] = (byte)time.Minute;
            result[2] = (byte)time.Second;

            return result;
        }
    }
}
