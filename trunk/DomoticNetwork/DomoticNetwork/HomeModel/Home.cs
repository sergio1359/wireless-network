using System;
using System.Collections.Generic;

namespace DomoticNetwork.HomeModel
{
    class Home
    {
        public String Name { get; set; }
        public List<Zone> Zones { get; set; }
    }

    class Zone
    {
        public String NameZone { get; set; }
        public int Height { get; set; }
        public int Weight { get; set; }
        //public Image map  { get; set; }
    }
}
