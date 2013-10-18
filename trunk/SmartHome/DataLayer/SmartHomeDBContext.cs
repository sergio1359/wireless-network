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
        public DbSet<Operation> Operations { get; set; }
        public DbSet<Log> Logs { get; set; }
        public DbSet<View> Views { get; set; }
        

        public SmartHomeDBContext()
            : base("SmartHomeDB")
        {
            Database.SetInitializer<SmartHomeDBContext>(new SmartHomeDBInitializer());
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //Haarcascade Remove
            //modelBuilder.Entity<Node>().HasMany(c => c.Connectors).WithRequired().WillCascadeOnDelete();


            base.OnModelCreating(modelBuilder);
        }

    }
}
