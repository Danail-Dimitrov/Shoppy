using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Shoppy.Models.DBEntities.Contracts
{
    public interface ISellOffer
    {
        [Key]
        [Required]
        int Id { get; set; }

        [Required]
        string ProductTitle { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        [DisplayName("Price")]
        decimal ProductPrice { get; set; }

        [Required]
        bool PriceIsNegotiable { get; set; }

        bool CanReciveBuyOffers { get; set; }

        int Quantity { get; set; }

        [Required]
        int UserId { get; set; }
  
        IList<BuyOffer> BuyOffers { get; set; }
    }
}
