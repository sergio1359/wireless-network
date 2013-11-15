using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Entities
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        public string TypeProduct { get; set; }

        public string NameProduct { get; set; }
    }
}
