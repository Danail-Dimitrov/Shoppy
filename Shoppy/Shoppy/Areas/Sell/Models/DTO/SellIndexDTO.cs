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

        public SellIndexDTO(List<SellOfferDTO> SellOfferDTOs)
        {
            this.SellOfferDTOs = SellOfferDTOs;
        }

        public List<SellOfferDTO> SellOfferDTOs { get; set; }
    }
}
