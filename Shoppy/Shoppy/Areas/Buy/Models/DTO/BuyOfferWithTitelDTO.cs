﻿using Shoppy.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shoppy.Areas.Buy.Models.DTO
{
    /// <summary>
    /// Extends BuyOfferDTO by also storing the title of the prodcut from the SellOffer.
    /// </summary>
    public class BuyOfferWithTitelDTO : BuyOfferDTO
    {
        public BuyOfferWithTitelDTO(int id, decimal offeredMoney, int sellOfferId, string productTitle) : base (id, offeredMoney, sellOfferId)
        {
            this.ProductTitle = productTitle;
        }

        public string ProductTitle { get;set; }
    }
}
