using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace SmartHome.HomeModel
{
    class Zone
    {
        public string NameZone { get; set; }
        public int Height { get; set; }
        public int Weight { get; set; }
        public Image Map  { get; set; }
    }
}
