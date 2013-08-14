using DataLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.BusinessEntities
{
    public static class BusinessTheme
    {
        public static void ExecuteTheme(this Theme theme)
        {
            theme.Operations.ForEach(op => op.Execute());
        }
    }
}
