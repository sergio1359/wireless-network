using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigGenerator.DeviceModel
{
    class NetworkConfig
    {
        //aunque cada uno tiene una direccion asociada despues hay cosas que si que no cambian por ejemplo ¿todos los dispositivos trabajan en el mismo canal?
        //bajo la misma contraseña?
        //esto hay que sacarlo fuera
        public UInt16 DeviceAddress { set; get; }
        public Byte Channel { set; get; }
        public UInt16 PanId { set; get; }
        public Byte[] SecurityKey { set; get; }

        public void GenerateKey(String key)
        {
        }


        public Byte[] ToBinary(bool littleEndian)
        {
            List<Byte> result = new List<Byte>();
            if (littleEndian)
            {
                result.Add((byte)DeviceAddress);
                result.Add((byte)(DeviceAddress >> 8));
            }
            else
            {
                result.Add((byte)(DeviceAddress >> 8));
                result.Add((byte)DeviceAddress);
            }

            result.Add(Channel);

            if (littleEndian)
            {
                result.Add((byte)PanId);
                result.Add((byte)(PanId >> 8));
            }
            else
            {
                result.Add((byte)(PanId >> 8));
                result.Add((byte)PanId);
            }

            result.AddRange(SecurityKey);

            return result.ToArray();
        }
    }
}
