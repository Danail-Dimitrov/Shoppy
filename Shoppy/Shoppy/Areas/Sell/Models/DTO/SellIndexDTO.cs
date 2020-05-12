using Shoppy.Models.DBEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shoppy.Areas.Sell.Models.DTO
{
    public class SellIndexDTO
    {
        public SellIndexDTO()
        {

        }

        public SellIndexDTO(List<SellOffer> sellOffers)
        {
            this.SellOffers = sellOffers;
        }

        public List<SellOffer> SellOffers { get; set; }
    }
}
