using System;
using System.Collections.Generic;
using System.Drawing;

namespace SmartHome.HomeModel
{
    public class Home
    {
        public string Name { get; set; }
        public List<Zone> Zones { get; set; }
        public Coordenate Location { get; set; }
    }

    public class Coordenate
    {
        public float Longitude { get; set; }
        public float Latitude { get; set; }
    }
}
