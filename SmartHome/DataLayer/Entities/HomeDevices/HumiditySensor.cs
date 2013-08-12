#region Using Statements

#endregion

namespace DataLayer.Entities.HomeDevices
{
    public class HumiditySensor : HomeDevice
    {
        public int Humidity { get; set; }

        public HumiditySensor()
            : base()
        {

        }
    }
}
