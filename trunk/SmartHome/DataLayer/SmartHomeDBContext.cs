#region Using Statements
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer.Entities;
using DataLayer.Entities.HomeDevices;
#endregion

namespace DataLayer
{
    public class SmartHomeDBContext : DbContext
    {
        DbSet<Node> Nodes { get; set; }
        DbSet<HomeDevice> HomeDevices { get; set; }
        DbSet<Connector> Connectors { get; set; }
        DbSet<Zone> Zones { get; set; }
        DbSet<Home> Home { get; set; }
        DbSet<Theme> Themes { get; set; }
        

        public SmartHomeDBContext()
            : base("SmartHomeDB")
        {
            Database.SetInitializer<SmartHomeDBContext>(new DropCreateDatabaseIfModelChanges<SmartHomeDBContext>());
        }

    }
}
