using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace ServiceLayer.DTO
{
    [DataContract]
    public class HomeDeviceDTO
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Type { get; set; }

        [DataMember]
        public bool InUse { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
