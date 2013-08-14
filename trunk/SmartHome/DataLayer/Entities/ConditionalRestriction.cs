#region Using Statements
using DataLayer.Entities.Enums;
using DataLayer.Entities.HomeDevices;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; 
#endregion

namespace DataLayer.Entities
{
    public enum ConditionalOperations
    {

    }

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
