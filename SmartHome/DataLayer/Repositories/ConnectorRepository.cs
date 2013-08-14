#region Using Statements
using DataLayer.Entities;
using DataLayer.Entities.HomeDevices;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 
#endregion

namespace DataLayer
{
    public class ConnectorRepository: Repository<Connector>
    {
        public ConnectorRepository(SmartHomeDBContext context) : base(context) { }

    }
}
