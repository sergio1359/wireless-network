using System;
using System.Collections.Generic;

namespace SmartHome.HomeModel
{
    public class Home
    {
        public string Name { get; set; }
        public List<Zone> Zones { get; set; }

        public double Longitude { get; set; }
        public double Latitude { get; set; }
    }
}
