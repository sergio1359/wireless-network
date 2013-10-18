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
    public class ConditionalRestrictionRepository : Repository<ConditionalRestriction>
    {
        public ConditionalRestrictionRepository(SmartHomeDBContext context) : base(context) { }



    }
}
