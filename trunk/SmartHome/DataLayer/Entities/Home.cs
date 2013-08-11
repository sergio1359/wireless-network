#region Using Statements
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;
#endregion

namespace SmartHome.DataLayer
{
    [Table("Home")]
    public class Home
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public Coordenate Location { get; set; }

        public Security Security = new Security();

        public virtual ICollection<Zone> Zones { get; set; }

        public Home()
        {
            this.Zones = new List<Zone>();
        }
    }

    [Table("Home")]
    public class Coordenate
    {
        public float Longitude { get; set; }

        public float Latitude { get; set; }
    }
}
