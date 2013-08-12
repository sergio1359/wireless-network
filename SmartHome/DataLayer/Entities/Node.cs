#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using DataLayer.Entities.HomeDevices;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DataLayer.Entities.Enums; 
#endregion

namespace DataLayer.Entities
{
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

        public Location Location { get; set; }

        public BaseTypes Base { get; set; }

        public ShieldTypes Shield { get; set; }

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
