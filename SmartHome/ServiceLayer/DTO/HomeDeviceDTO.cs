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

        [DataMember]
        public StateHomeDeviceDTO[] State { get; set; }

        public override string ToString()
        {
            return "ID: " + Id + "  " + Name + "(" + Type + ")";
        }

        public class StateHomeDeviceDTO
        {
            public string NamePropierty { get; set; }

            public string Type { get; set; }

            public string Value { get; set; }
        }
    }
}
