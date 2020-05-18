using Shoppy.Areas.Sell.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shoppy.Areas.Sell.Services.Contracts
{
    public interface ISellService
    {
        List<SellOfferDTO> GetOffersFromUser(int userId);

        void CreateSellOffer(SellOfferDTO sellOfferDTO, int userId);

        void EditSellOffer(SellOfferDTO sellOfferDTO);

        SellOfferDTO GetSellOfferById(int? id);

        void Delete(int? id);

        void ValidateSellOfferDTO(SellOfferDTO sellOfferDTO);

        void IncreaseUserScore(int? userId);
    }
}
