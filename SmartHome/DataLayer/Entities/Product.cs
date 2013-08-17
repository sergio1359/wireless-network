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

        /// <summary>
        /// Type of product build
        /// </summary>
        public string TypeProduct { get; set; }

        /// <summary>
        /// Name of the concrete product
        /// </summary>
        public string NameProduct { get; set; }
    }
}
