#region Using Statements
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#endregion

namespace DataLayer.Entities.HomeDevices
{
    [Table("PresenceSensor")]
    public class PresenceSensor : HomeDevice
    {
        public const int DEFAULT_SENSIBILITY = 10;

        [Range(0, 255)]
        public int Sensibility { get; set; }

        public PresenceSensor()
            : base()
        {
            base.Operations = new List<Operation>();
        }
    }
}
