#region Using Statements
using DataLayer.Entities.HomeDevices;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations; 
#endregion

namespace DataLayer.Entities
{
    public class Operation
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        [Required]
        public string OperationName { get; set; }

        public object[] Args { get; set; }

        public virtual HomeDevice SourceHomeDevice { get; set; }

        public virtual HomeDevice DestionationHomeDevice { get; set; }

        public virtual ICollection<TimeRestriction> TimeRestrictions { get; set; }

        public virtual ICollection<ConditionalRestriction> ConditionalRestriction { get; set; }

        public Operation()
        {
            this.TimeRestrictions = new List<TimeRestriction>();
            this.ConditionalRestriction = new List<ConditionalRestriction>();
        }
    }
}