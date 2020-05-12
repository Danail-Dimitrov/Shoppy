using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shoppy.Models.DBEntities
{
    public class ProductTagSellOffer
    {
        public int ProductTagId { get; set; }
        public ProductTag ProductTag { get; set; }
        public int SellOfferId { get; set; }
        public SellOffer SellOffer { get; set; }
    }
}
