using Shoppy.Models.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Shoppy.Areas.Sell.Models.DTO
{
    /// <summary>
    /// BuyOfferDTO that also shows the Price asked for a product
    /// </summary>
    public class ShowBuyOffersDTO : BuyOfferDTO
    {
        public ShowBuyOffersDTO(int id, decimal offeredMoney, int sellOfferId, decimal askingPrice) : base (id, offeredMoney, sellOfferId)
        {
            this.AskingPrice = askingPrice;
        }

        [Display(Name = "Asking Price")]
        [Range(0.01, 9999999999, ErrorMessage = "The amout of money offered must be betwen 0.01 and 9999999999")]
        public decimal AskingPrice { get; set; }
    }
}
