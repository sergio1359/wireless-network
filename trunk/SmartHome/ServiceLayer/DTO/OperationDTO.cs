using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.DTO
{
    [DataContract]
    public class OperationDTO
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public OperationParams[] Params { get; set; }

        [DataMember]
        public TimeRestrictionDTO[] TimeRestrictions { get; set; }


        public override string ToString()
        {
            return Name;
        }
    }

    [DataContract]
    public class TimeRestrictionDTO
    {
        
    }

    public class OperationParams
    {
        public string Type { get; set; }
        public string Name { get; set; }
        public object Value { get; set; }
    }
}
