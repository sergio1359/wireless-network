using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;

namespace SmartHome.HomeModel
{
    public class Home
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public Coordenate Location { get; set; }

        public virtual ICollection<Zone> Zones { get; set; }

        public Home()
        {
            this.Zones = new List<Zone>();
        }
    }

    public class Coordenate
    {
        public float Longitude { get; set; }
        public float Latitude { get; set; }
    }
}
