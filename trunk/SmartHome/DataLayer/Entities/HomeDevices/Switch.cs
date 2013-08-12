#region Using Statements
using DataLayer.Entities.Enums;
using System.Collections.Generic;

#endregion

namespace DataLayer.Entities.HomeDevices
{
    public class Switch : HomeDevice
    {
        public bool Open { get; set; }

        public Switch()
            : base()
        {
            base.Operations = new List<Operation>();
            base.ConnectorCapable = ConnectorTypes.LogicInput;
        }
    }
}
