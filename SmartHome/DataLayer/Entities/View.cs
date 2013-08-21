#region Using Statements
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        [InverseProperty("Views")]
        public virtual Zone Zone { get; set; } //Se generan dos zones (uno para el mainView y el otro para apuntar a la lista de Views)
    }
}
