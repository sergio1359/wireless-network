#region Using Statements
using DataLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion

namespace DataLayer
{
    public class LocationRepository : Repository<Location>
    {
        public LocationRepository(SmartHomeDBContext context) : base(context) { }



    }
}
