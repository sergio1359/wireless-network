using System.Runtime.Serialization;

namespace ServiceLayer.DTO
{
    [DataContract]
    public class NodeDTO
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public uint Address { get; set; }

        [DataMember]
        public string Base { get; set; }

        [DataMember]
        public string Shield { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
