using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Entities.HomeDevices
{
    public class PropertyAttribute: System.Attribute
    {
        public bool Internal
        {
            get;
            set;
        }

        public PropertyAttribute() { }
    }
}
