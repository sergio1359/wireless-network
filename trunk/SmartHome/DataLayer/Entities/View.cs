#region Using Statements
using System.ComponentModel.DataAnnotations;

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
        public virtual Zone Zone { get; set; }
    }
}
