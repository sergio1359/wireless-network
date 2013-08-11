#region Using Statements
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Linq;
using System.Text; 
#endregion

namespace DataLayer.Entities
{
    public class View
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public byte[] ImageMap { get; set; }

        [Required]
        public virtual Zone Zone;
    }
}
