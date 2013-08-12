#region Using Statements
using DataLayer.Entities.Enums;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

#endregion

namespace DataLayer.Entities.HomeDevices
{
    public class Switch : HomeDevice
    {
        [NotMapped]
        public bool? Open { get; set; }

        public Switch()
            : base()
        {
            base.Operations = new List<Operation>();
            base.ConnectorCapable = ConnectorTypes.LogicInput;
        }
    }
}
