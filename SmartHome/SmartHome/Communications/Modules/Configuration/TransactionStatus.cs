using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Communications.Modules.Config
{
    public class TransactionStatus
    {
        public ushort NodeAddress { get; set; }

        public float Percentage { get; set; }
    }
}
