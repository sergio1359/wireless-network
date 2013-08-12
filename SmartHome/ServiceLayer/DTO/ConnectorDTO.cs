using System.Runtime.Serialization;

namespace ServiceLayer.DTO
{
    [DataContract]
    public class ConnectorDTO
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string ConnectorType { get; set; }

        [DataMember]
        public bool InUse { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
