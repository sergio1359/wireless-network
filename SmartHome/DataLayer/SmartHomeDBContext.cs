using SmartHome.Network;
using SmartHome.Network.HomeDevices;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer
{
    public class SmartHomeDBContext : DbContext
    {
        DbSet<Connector> Connectors { get; set; }
        DbSet<Node> Nodes { get; set; }
        DbSet<Security> Securities { get; set; }
        DbSet<Position> Positions { get; set; }
        DbSet<SmartHome.Network.HomeDevices.HomeDevice> HomeDevices { get; set; }
        DbSet<Operation> Operations { get; set; }
        DbSet<TimeRestriction> TimeRestrictions { get; set; }

        public SmartHomeDBContext()
            : base("SmartHomeDB")
        {

            Database.SetInitializer<SmartHomeDBContext>(new DropCreateDatabaseIfModelChanges<SmartHomeDBContext>());
        }

    }
}
