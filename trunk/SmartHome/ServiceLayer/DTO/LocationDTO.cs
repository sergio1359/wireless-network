using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.DTO
{
    [DataContract]
    public class LocationDTO
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string NameZone { get; set; }

        [DataMember]
        public ViewDTO MainView { get; set; }

        [DataMember]
        public ViewDTO[] Views { get; set; }
    }

    [DataContract]
    public class ViewDTO
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public float X { get; set; }

        [DataMember]
        public float Y { get; set; }
    }
}
