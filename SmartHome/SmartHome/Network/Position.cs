using SmartHome.HomeModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace SmartHome.Network
{
    public class Position
    {
        public int Id { get; set; }

        public Zone Zone { get; set; }
        public PointF ZoneCoordenates { get; set; }

        public List<Tuple<View, PointF>> Views { get; set; }

        public bool Movil { get; set; }

        public void CalculatePosition()
        {
            
        }
    }
}
