using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.DTO
{
    public class PlaceDTO
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public Image ImageMap { get; set; }
    }
}
