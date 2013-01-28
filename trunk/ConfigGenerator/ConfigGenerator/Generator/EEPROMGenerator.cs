using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigGenerator.EEPROM
{
    class Generator
    {
        public ConfigDevice Device { get; set; }
        public Byte FirmVersion { get; set; }
        

        public Generator(ConfigDevice device)
        {
            Device = device;
        }

        public UInt16 CRCGenetator()
        {

        }

        public Byte[] GenerateEEPROM() 
        {

        }


    }
}
