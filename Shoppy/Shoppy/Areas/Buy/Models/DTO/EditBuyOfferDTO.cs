using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Shoppy.Areas.Buy.Models.DTO
{
    /// <summary>
    ///  DTO used to transfer information when a BuyOffer is beeing edited
    /// </summary>
    public class EditBuyOfferDTO
    {
        public int Id { get; set; }
        [Display(Name = "Money Offered")]
        [Range(0.01, 9999999999, ErrorMessage = "The amout of money offered must be betwen 0.01 and 9999999999")]
        public decimal MoneyOffered { get; set; }
    }
}
