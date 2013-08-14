using DataLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer
{
    public class TimeOperationRepository : Repository<TimeOperation>
    {
        public TimeOperationRepository(SmartHomeDBContext context) : base(context) { }



    }
}
