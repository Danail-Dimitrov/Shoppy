using Shoppy.Models.DBEntities.Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Shoppy.Models.DBEntities
{
    public class ProductTag : IProductTag
    {
        public ProductTag()
        {
        }

        [Key]
        public int Id { get; set; }

        [Required]
        public string Tag { get; set; }

        public  IList<ProductTagSellOffer> ProductTagSellOffers { get; set; }
    }
}
