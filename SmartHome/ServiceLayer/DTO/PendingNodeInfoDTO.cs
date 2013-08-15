using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer.Entities;

namespace ServiceLayer.DTO
{
    public class PendingNodeInfoDTO
    {
        public string MacAddress { get; set; }

        public ShieldTypes ShieldType { get; set; }

        public BaseTypes BaseType { get; set; }

        public override string ToString()
        {
            return MacAddress + " " + BaseType + " " + ShieldType;
        }
    }
}
