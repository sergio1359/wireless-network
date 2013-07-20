using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace SmartHome.HomeModel
{
    public class Zone
    {
        public int Id { get; set; }
        public string NameZone { get; set; }
        public List<View> Views { get; set; }
        public Image Map  { get; set; }
    }
}
