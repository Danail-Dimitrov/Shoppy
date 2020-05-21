using Shoppy.Areas.Sell.Models.DTO;
using Shoppy.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shoppy.Areas.Sell.Services.Contracts
{
    public interface ISellService
    {
        List<SellOfferDTO> GetSellOffersFromUser(int? userId);

        void CreateSellOffer(SellOfferDTO sellOfferDTO, int? userId);

        void EditSellOffer(SellOfferDTO sellOfferDTO, int? userId);

        SellOfferDTO GetSellOfferById(int? id);

        void Delete(int? id, int userId);

        void ValidateSellOfferDTO(SellOfferDTO sellOfferDTO);

        void IncreaseUserScore(int? userId);

        List<ShowBuyOffersDTO> GetBuyOffers(int? sellOfferId);
    }
}
