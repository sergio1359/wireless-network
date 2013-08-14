using DataLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer
{
    public class ZoneRepository : Repository<Zone>
    {
        public ZoneRepository(SmartHomeDBContext context) : base(context) { }



    }
}
