using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Shoppy.Models.DBEntities
{
    public class ProductTagSellOffer
    {
        [Required]
        public int ProductTagId { get; set; }
        public ProductTag ProductTag { get; set; }
        [Required]
        public int SellOfferId { get; set; }
        public SellOffer SellOffer { get; set; }
    }
}
