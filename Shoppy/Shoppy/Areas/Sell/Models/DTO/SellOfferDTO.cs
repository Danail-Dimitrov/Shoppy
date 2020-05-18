using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        [StringLength(999999, ErrorMessage = "Products Title must be betwen 999999 and 3 in length", MinimumLength =3)]
        [Display(Name = "Product Title")]
        public string ProductTitle { get; set; }
        [Display(Name = "Product Description")]
        public string ProductDescription { get; set; }
        [Display(Name = "Product Price")]
        [Range(0.01, 9999999999, ErrorMessage = "The price of a product must be betwen 0.01 and 9999999999")]
        public decimal ProductPrice { get; set; }
        [Display(Name = "Total Price")]
        public decimal TotalPrice { get; set; }
        [Display(Name = "Is the price Negotiable")]
        public bool PriceIsNegotiable { get; set; }
        [Display(Name = "Can buy offers be recived")]
        public bool CanReciveBuyOffers { get; set; }
        [Display(Name = "Quantity")]
        [Range(1, 999999, ErrorMessage = "The quantity of a product must be betwen 1 and 999999")]
        public int Quantity { get; set; }
        public string TagsStr { get; set; }
        public List<string> Tags { get; set; }
    }
}
