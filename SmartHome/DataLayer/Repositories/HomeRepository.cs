using DataLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer
{
    public class HomeRepository : Repository<Home>
    {
        public HomeRepository(SmartHomeDBContext context) : base(context) { }

        public Home GetHome()
        {
            return _Collection.First();
        }

    }
}
