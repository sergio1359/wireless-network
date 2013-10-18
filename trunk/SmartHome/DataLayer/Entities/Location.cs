#region Using Statements
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DataLayer.Entities.HomeDevices; 
#endregion

namespace DataLayer.Entities
{
    public class Location
    {
        [Key]
        public int Id { get; set; }

        public float X { get; set; }

        public float Y { get; set; }

        public bool Mobile { get; set; }

        [Required]
        public virtual View View { get; set; }
    }
}
