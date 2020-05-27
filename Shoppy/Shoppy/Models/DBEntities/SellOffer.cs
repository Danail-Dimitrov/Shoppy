using Shoppy.Models.DBEntities.Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Shoppy.Models.DBEntities
{
    public class SellOffer : ISellOffer
    {
        public SellOffer()
        {

        }

        [Key]
        public int Id { get; set; }
        [DisplayName("Product")]
        [Required]
        public string ProductTitle { get; set; }
        public string ProductDescription { get; set; }
        [Column(TypeName = "decimal(10,2)")]
        [DisplayName("Price")]
        [Required]
        public decimal ProductPrice { get; set; }
        [Column(TypeName = "decimal(10,2)")]
        [DisplayName("Total Price")]
        public decimal TotalPrice { get; set; }
        [Required]
        public bool PriceIsNegotiable { get; set; }
        public bool CanReciveBuyOffers { get; set; }
        [Required]
        public int Quantity { get; set; }
        [Required]
        [ForeignKey("UserId")]
        public int UserId { get; set; }
        public bool HasAcceptedBuyOffer { get; set; }
        public int AcceptedBuyOfferId { get; set; }


        public IList<ProductTagSellOffer> ProductTagSellOffers { get; set; }        
        public IList<BuyOffer> BuyOffers { get; set; }
    }
}
