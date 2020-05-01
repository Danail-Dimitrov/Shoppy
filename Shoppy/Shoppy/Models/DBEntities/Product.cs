using Shoppy.Models.DBEntities.Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Shoppy.Models.DBEntities
{
    public class Product : IProduct
    {
        public Product()
        {

        }

        public Product(int id, string name, decimal price, string model)
        {
            this.Id = id;
            this.Name = name;
            this.Price = price;
            this.Model = model;
        }

        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [Column(TypeName = "decimal(7, 5)")]
        public decimal Price { get; set; }

        public string Model { get; set; }
    }
}
