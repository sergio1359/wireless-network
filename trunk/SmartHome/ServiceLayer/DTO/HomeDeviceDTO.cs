#region Using Statements
using System.Collections.Generic;
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
        public IEnumerable<StateHomeDeviceDTO> State { get; set; }

        public HomeDeviceDTO()
        {
            State = new List<StateHomeDeviceDTO>();
        }

        public override string ToString()
        {
            return "ID: " + Id + "  " + Name + "(" + Type + ")";
        }
    }

    [DataContract]
    public class StateHomeDeviceDTO
    {
        [DataMember]
        public string NamePropierty { get; set; }

        [DataMember]
        public string Type { get; set; }

        [DataMember]
        public string Value { get; set; }

        public override string ToString()
        {
            return NamePropierty + ": " + Value;
        }
    }
}
