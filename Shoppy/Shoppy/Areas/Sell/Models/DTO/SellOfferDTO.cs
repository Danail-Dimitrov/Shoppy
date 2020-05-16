using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shoppy.Areas.Sell.Models.DTO
{
    public class SellOfferDTO
    {
        public SellOfferDTO()
        {

        }

        public SellOfferDTO(int id, string productTitle, decimal productPrice, decimal totalPrice, bool priceIsNegotiable, bool canReciveBuyOffers, int quantity, string tags)
        {
            this.Id = id;
            this.ProductTitle = productTitle;
            this.ProductPrice = productPrice;
            this.TotalPrice = totalPrice;
            this.PriceIsNegotiable = priceIsNegotiable;
            this.CanReciveBuyOffers = canReciveBuyOffers;
            this.Quantity = quantity;
            this.Tags = tags;
        }

        public SellOfferDTO(string productTitle, decimal productPrice, decimal totalPrice, bool priceIsNegotiable, bool canReciveBuyOffers, int quantity, string tags)
        {
            this.ProductTitle = productTitle;
            this.ProductPrice = productPrice;
            this.TotalPrice = totalPrice;
            this.PriceIsNegotiable = priceIsNegotiable;
            this.CanReciveBuyOffers = canReciveBuyOffers;
            this.Quantity = quantity;
            this.Tags = tags;
        }

        public int Id { get; set; }
        public string ProductTitle { get; set; }
        public decimal ProductPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public bool PriceIsNegotiable { get; set; }
        public bool CanReciveBuyOffers { get; set; }
        public int Quantity { get; set; }
        public string Tags { get; set; }
    }
}
