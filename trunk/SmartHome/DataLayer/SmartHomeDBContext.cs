#region Using Statements
using DataLayer.Entities;
using DataLayer.Entities.HomeDevices;
using System.Data.Entity;
#endregion

namespace DataLayer
{
    public class SmartHomeDBContext : DbContext
    {
        public DbSet<Node> Nodes { get; set; }
        public DbSet<HomeDevice> HomeDevices { get; set; }
        public DbSet<Connector> Connectors { get; set; }
        public DbSet<Zone> Zones { get; set; }
        public DbSet<Home> Home { get; set; }
        public DbSet<Theme> Themes { get; set; }
        public DbSet<TimeOperation> TimeOperations { get; set; }
        

        public SmartHomeDBContext()
            : base("SmartHomeDB")
        {
            Database.SetInitializer<SmartHomeDBContext>(new DropCreateDatabaseIfModelChanges<SmartHomeDBContext>());
        }

    }
}
