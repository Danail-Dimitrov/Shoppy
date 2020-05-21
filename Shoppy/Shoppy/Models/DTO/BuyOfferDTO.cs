using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Shoppy.Models.DTO
{
    public class BuyOfferDTO
    {
        public BuyOfferDTO()
        {

        }

        public BuyOfferDTO(int id, decimal offeredMoney, int sellOfferId)
        {
            this.Id = id;
            this.OfferedMoney = offeredMoney;
            this.SellOfferId = sellOfferId;
        }


        public int Id { get; set; }
        [Display(Name = "Money Offered")]
        [Range(0.01, 9999999999, ErrorMessage = "The amout of money offered must be betwen 0.01 and 9999999999")]
        public decimal OfferedMoney { get; set; }
        [Display(Name = "Id of the sell offer")]
        public int SellOfferId { get; set; }
    }
}
