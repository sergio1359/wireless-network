#region Using Statements
using System.ComponentModel.DataAnnotations; 
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

        public virtual View View { get; set; }

    }
}
