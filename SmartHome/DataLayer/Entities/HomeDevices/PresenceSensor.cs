#region Using Statements
using System.Collections.Generic;

#endregion

namespace DataLayer.Entities.HomeDevices
{
    public class PresenceSensor : HomeDevice
    {
        public byte Sensibility { get; set; }
        public const int DEFAULT_SENSIBILITY = 10;

        public PresenceSensor()
            : base()
        {
            base.Operations = new List<Operation>();
        }
    }
}
