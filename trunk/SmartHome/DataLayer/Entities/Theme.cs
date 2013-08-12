using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DataLayer.Entities
{
    public class Theme
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public List<Operation> Operations { get; set; }
    }
}
