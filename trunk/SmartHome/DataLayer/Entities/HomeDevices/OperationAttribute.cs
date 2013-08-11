#region Using Statements

#endregion

namespace DataLayer.Entities.HomeDevices
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
