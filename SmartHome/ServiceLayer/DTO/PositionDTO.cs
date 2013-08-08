using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.DTO
{
    [DataContract]
    public class PositionDTO
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public AxisDTO Zone { get; set; }

        [DataMember]
        public AxisDTO[] Views { get; set; }
    }

    [DataContract]
    public class AxisDTO
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public float X { get; set; }

        [DataMember]
        public float Y { get; set; }
    }
}
