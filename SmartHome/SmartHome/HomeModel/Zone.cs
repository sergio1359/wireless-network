using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.ComponentModel.DataAnnotations;

namespace SmartHome.HomeModel
{
    public class Zone
    {
        [Key]
        public int Id { get; set; }

        public string NameZone { get; set; }

        public List<View> Views { get; set; }

        public Image ImageMap  { get; set; }
    }
}
