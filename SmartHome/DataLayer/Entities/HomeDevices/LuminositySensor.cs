#region Using Statements

#endregion

namespace DataLayer.Entities.HomeDevices
{
    public class LuminositySensor : HomeDevice
    {
        public byte Sensibility { get; set; }

        public LuminositySensor()
            : base()
        {

        }
    }
}
