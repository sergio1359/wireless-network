#region Using Statements
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations; 
#endregion

namespace DataLayer.Entities
{
    public class Zone
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public View MainView { get; set; }

        [Required]
        public virtual Home Home { get; set; }

        public virtual ICollection<View> Views { get; set; }

        public Zone()
        {
            this.Views = new List<View>();
        }
    }
}
