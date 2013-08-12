#region Using Statements
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer.Entities.Enums;
using DataLayer.Entities.HomeDevices; 
#endregion

namespace DataLayer.Entities
{
    [NotMapped]
    public class ConditionalRestriction
    {
        [Key]
        public int Id { get; set; }

        public object ValueMine { get; set; }

        public ConditionalOperations Operation { get; set; }

        /// <summary>
        /// Property Name of the HomeDeviceValue
        /// </summary>
        public string NameProperty { get; set; }

        public virtual HomeDevice HomeDeviceValue { get; set; }
    }
}
