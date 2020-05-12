using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shoppy.Areas.Sell.Models.DTO
{
    public class CreateSellOfferDTO
    {
        public CreateSellOfferDTO()
        {

        }

        public CreateSellOfferDTO(string productTitle, decimal productPrice, bool priceIsNegotiable, int quantity, string tags)
        {
            this.ProductTitle = productTitle;
            this.ProductPrice = productPrice;
            this.PriceIsNegotiable = priceIsNegotiable;
            this.Quantity = quantity;
            this.Tags = tags;
        }

        public string ProductTitle { get; set; }
        public decimal ProductPrice { get; set; }
        public bool PriceIsNegotiable { get; set; }
        public int Quantity { get; set; }
        public string Tags { get; set; }
    }
}
