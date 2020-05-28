using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Shoppy.Areas.Buy.Models.DTO
{
    /// <summary>
    /// DTO used to transfer information, when gettind a SellOffer based on a certain criteria.
    /// </summary>
    public class GetSellOfferByNameDTO
    {
        [StringLength(999999, ErrorMessage = "Products Title must be betwen 999999 and 3 in length", MinimumLength = 3)]
        [Display(Name = "Product Title")]
        public string ProductTitle { get; set; }
    }
}
