
using System;
using System.Runtime.Serialization;
namespace ServiceLayer.DTO
{
    [DataContract]
    public class LogDTO
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public DateTime Date { get; set; }

        [DataMember]
        public string Category { get; set; }

        [DataMember]
        public string Text { get; set; }
    }
}
