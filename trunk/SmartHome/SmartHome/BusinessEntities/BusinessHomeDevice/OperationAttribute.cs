#region Using Statements

#endregion

namespace SmartHome.BusinessEntities.BusinessHomeDevice
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
