
namespace SmartHome.Plugins
{
    public interface IConfiguration
    {
        public void GetParameters();
        public void GetType();
        public void SaveChanges();
        public void SetValues();
    }
}
