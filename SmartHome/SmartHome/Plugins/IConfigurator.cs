
namespace SmartHome.Plugins
{
    public interface IConfigurator
    {
        void GetParameters();
        void GetType();
        void SaveChanges();
        void SetValues();
    }
}
