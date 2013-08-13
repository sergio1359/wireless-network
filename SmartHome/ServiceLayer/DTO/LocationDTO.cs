using System.Runtime.Serialization;

namespace ServiceLayer.DTO
{
    [DataContract]
    public class LocationDTO
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public int IdView { get; set; }

        [DataMember]
        public float X { get; set; }

        [DataMember]
        public float Y { get; set; }

        [DataMember]
        public bool Mobile { get; set; }
    }
}
