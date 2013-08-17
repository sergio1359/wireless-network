using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.DTO
{
    [DataContract]
    public class TimeOperationDTO
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public byte WeekDays { get; set; }

        [DataMember]
        public TimeSpan Time { get; set; }

        [DataMember]
        public OperationDTO[] Operation { get; set; }
    }
}
