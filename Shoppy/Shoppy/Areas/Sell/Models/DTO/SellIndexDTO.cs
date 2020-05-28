using System;
using System.Collections.Generic;
using Shoppy.Models.DTO;

namespace Shoppy.Areas.Sell.Models.DTO
{
    /// <summary>
    /// DTO carring a List of SellOfferDTOs for the Index View
    /// </summary>
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
