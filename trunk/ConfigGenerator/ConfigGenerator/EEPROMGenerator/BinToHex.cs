using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

//http://www.ht-lab.com/freeutils/bin2hex/bin2hex.html

namespace Bin2Hexample
{
    class Program
    {
        static void Main(string[] args)
        {
            byte[] ex = { 0x43, 0x50, 0x55, 0x38, 0x38, 0x20, 0x42, 0x6f, 0x6f, 0x74, 0x6c, 0x6f, 0x61, 0x64, 0x65, 0x72,
                          0x20, 0x52, 0x65, 0x61, 0x64, 0x79, 0x3e, 0x0a, 0x0d, 0x00, 0x8c, 0xc8, 0x8e, 0xd8, 0x33, 0xc0,
                          0x8e, 0xc0, 0x06, 0xbf, 0x00, 0x01, 0x57, 0xcb, 0x52, 0x50, 0xba, 0xf9, 0x02, 0xec, 0x24, 0x02,
                          0x74, 0xfb, 0xba, 0xf8, 0x02, 0x58, 0xee, 0x5a, 0xc3, 0x52, 0xba, 0xf9, 0x02, 0xec, 0x24, 0x01};

            File.WriteAllBytes("ex.bin", ex);
            string hexstr = Bin2Hex(ex);
            File.WriteAllText("ex.hex", hexstr);
            Console.WriteLine(hexstr);
            Console.ReadLine();
        }

        static string Bin2Hex(byte[] input)
        {
            //Start Char ':'    |   Length(1)   |   Address(2)  |   Rec Type(1)  |   Data(n)  |   Checksum(1)

            /*
             * 00 Data Record
             * 01 End of File Record
             * 02 Extended Segment Address Record
             * 03 Start Segment Address Record
             * 04 Extended Linear Address Record
             * 05 Start Linear Address Record
             * */

            StringBuilder result = new StringBuilder();
            string stringArray = string.Join(string.Empty, Array.ConvertAll(input, b => b.ToString("X2")));
            for (int i = 0; i < input.Length; i += 32)
            {
                int lineLength = Math.Min(32, input.Length - i);
                result.Append(":" + lineLength.ToString("X2") + i.ToString("X4") + "00");

                byte checksum = (byte)input.Skip(i).Take(lineLength).Select(x => (int)x).Sum();
                checksum += (byte)(lineLength + i);
                checksum = (byte)((checksum ^ 0xFF) + 1);//two's complement

                result.Append(stringArray, i * 2, lineLength * 2);
                result.Append(checksum.ToString("X2"));
                result.AppendLine();
            }
            result.AppendLine(":00000001FF");

            return result.ToString();
        }
    }
}