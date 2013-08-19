#region Using Statements
using System.Runtime.Serialization;
#endregion

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
            return "ID: " + Id + "  " + Name + "(" + Type + ")";
        }
    }
}
