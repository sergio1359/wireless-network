#region Using Statements
using DataLayer.Entities.Enums;
using DataLayer.Entities.HomeDevices;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq; 
#endregion

namespace DataLayer.Entities
{
    public enum BaseTypes : byte
    {
        ATMega128RFA1_V1 = 0,
        ATMega128RFA1_V2 = 1,
    }

    public enum ShieldTypes
    {
        Debug,
    }

    public class Node
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(6), MinLength(6, ErrorMessage = "Mac must have 6 characters length")]
        public string Mac { get; set; }

        public string Name { get; set; }

        [Range(0, 30)]
        public int NetworkRetries { get; set; }

        public ushort Address { get; set; }

        public BaseTypes Base { get; set; }

        public ShieldTypes Shield { get; set; }

        public virtual Location Location { get; set; }

        public virtual ICollection<Connector> Connectors { get; set; }

        [NotMapped]
        public IEnumerable<HomeDevice> HomeDevices
        {
            get
            {
                return this.Connectors.SelectMany(c => c.HomeDevices);
            }
        }

        public Node()
        {
            this.Connectors = new List<Connector>();
        }
    }
}
