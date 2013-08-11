#region Using Statements
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer.Entities.HomeDevices; 
#endregion

namespace DataLayer.Entities
{
    [NotMapped]
    public class ConditionalRestriction
    {
        [Key]
        public int Id { get; set; }

        public string NameProperty;

        public object Value;

        public Operations Operation;

        public virtual HomeDevice HomeDeviceValue;

        public enum Operations
        {

        }
    }
}
