using System;
using System.Runtime.Serialization;

namespace ServiceLayer.DTO
{
    [DataContract]
    public class TimeRestrictionDTO
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public byte MaskWeekDays { get; set; }

        [DataMember]
        public DateTime DateStart { get; set; }

        [DataMember]
        public DateTime DateEnd { get; set; }

        [DataMember]
        public TimeSpan TimeStart { get; set; }

        [DataMember]
        public TimeSpan TimeEnd { get; set; }
    }
}
