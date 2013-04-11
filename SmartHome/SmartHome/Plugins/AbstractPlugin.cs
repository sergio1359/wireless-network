
namespace SmartHome.Plugins
{
    public abstract class AbstractPlug
    {
        public string Name;
        public string Author;
        public string Description;
        public double Version;

        public void Finish(){}

        public void GetFunctions(){}

        public void Inicialize(){}

        public void Invoke(){}

        public void Refresh(){}


    }
}
