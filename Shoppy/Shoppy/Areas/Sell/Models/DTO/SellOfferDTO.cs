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

        public SellOfferDTO(string productTitle, string productDescription, decimal productPrice, decimal totalPrice, bool priceIsNegotiable, bool canReciveBuyOffers, int quantity, string tagsStr)
        {
            this.ProductTitle = productTitle;
            this.ProductDescription = productDescription;
            this.ProductPrice = productPrice;
            this.TotalPrice = totalPrice;
            this.PriceIsNegotiable = priceIsNegotiable;
            this.CanReciveBuyOffers = canReciveBuyOffers;
            this.Quantity = quantity;
            this.TagsStr = tagsStr;
        }

        public SellOfferDTO(int id, string productTitle, string productDescription, decimal productPrice, decimal totalPrice, bool priceIsNegotiable, bool canReciveBuyOffers, int quantity, string tagsStr)
        {
            this.Id = id;
            this.ProductTitle = productTitle;
            this.ProductDescription = productDescription;
            this.ProductPrice = productPrice;
            this.TotalPrice = totalPrice;
            this.PriceIsNegotiable = priceIsNegotiable;
            this.CanReciveBuyOffers = canReciveBuyOffers;
            this.Quantity = quantity;
            this.TagsStr = tagsStr;
        }

        public int Id { get; set; }
        public string ProductTitle { get; set; }
        public string ProductDescription { get; set; }
        public decimal ProductPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public bool PriceIsNegotiable { get; set; }
        public bool CanReciveBuyOffers { get; set; }
        public int Quantity { get; set; }
        public string TagsStr { get; set; }
        public List<string> Tags { get; set; }
    }
}
