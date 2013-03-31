using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Extensions.Enums;
using SmartHome.Network.HomeDevices;

namespace SmartHome.Network
{
    public abstract class ActionAbstract
    {
        public int ID { get; set; }
        public String Name { get; set; }
        public HomeDevice ToHomeDevice { get; set; }
        public Enums.OPCode OPCode { get; set; }
        public Object Args { get; set; }
        public Boolean Enable { get; set; }

        public void Execute(){}

        public override string ToString()
        {
 	            return base.ToString();
        }


    }
}
