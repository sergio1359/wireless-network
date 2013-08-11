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

        public string Name { get; set; }

        public Image ImageMap { get; set; }

        public virtual Home Home { get; set; }

        public virtual ICollection<View> Views { get; set; }

        public Zone()
        {
            this.Views = new List<View>();
        }
    }
}
