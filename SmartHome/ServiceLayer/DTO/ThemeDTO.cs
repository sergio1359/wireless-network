
using System;
using System.Runtime.Serialization;
namespace ServiceLayer.DTO
{
    [DataContract]
    public class ThemeDTO
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string name { get; set; }

        public override string ToString()
        {
            return name;
        }
    }
}
