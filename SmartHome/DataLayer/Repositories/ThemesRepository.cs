using DataLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer
{
    public class ThemesRepository : Repository<Theme>
    {
        public ThemesRepository(SmartHomeDBContext context) : base(context) { }



    }
}
