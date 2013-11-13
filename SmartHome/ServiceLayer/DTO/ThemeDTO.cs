
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
        public string Name { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
