#region Using Statements

#endregion

namespace SmartHome.Network.HomeDevices
{
    public class OperationAttribute : System.Attribute 
    {
        public bool Internal
        {
            get;
            set;
        }

        public OperationAttribute()
        {
        }
    }
}
