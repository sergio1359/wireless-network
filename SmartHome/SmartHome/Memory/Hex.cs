using System;
using System.IO;
using System.Linq;
using System.Text;

//http://www.ht-lab.com/freeutils/bin2hex/bin2hex.html

namespace SmartHome.Memory
{
    static class Hex
    {
        public static void SaveBin2Hex(byte[] input, string fileName)
        {
            string hexstr = Bin2Hex(input);
            File.WriteAllText(fileName + ".hex", hexstr);
        }

        private static string Bin2Hex(byte[] input)
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