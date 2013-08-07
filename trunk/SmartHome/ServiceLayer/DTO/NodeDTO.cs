using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.DTO
{
    public class NodeDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Base { get; set; }
        public string Shield { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
