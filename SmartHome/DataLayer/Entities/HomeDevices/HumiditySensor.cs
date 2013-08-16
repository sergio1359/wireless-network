#region Using Statements

#endregion

using System.ComponentModel.DataAnnotations.Schema;
namespace DataLayer.Entities.HomeDevices
{
    public class HumiditySensor : HomeDevice
    {
        [NotMapped]
        public int? Humidity { get; set; }

        public HumiditySensor()
            : base()
        {
            this.ConnectorCapable = Enums.ConnectorTypes.LogicInput;
        }
    }
}
