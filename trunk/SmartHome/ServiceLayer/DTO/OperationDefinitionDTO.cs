using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.DTO
{
    [DataContract]
    public class OperationDefinitionDTO
    {
        [DataMember]
        public string NameOperation { get; set; }

        [DataMember]
        public string ReturnValueType { get; set; }

        [DataMember]
        public ParamDTO[] Args { get; set; }
    }
}
