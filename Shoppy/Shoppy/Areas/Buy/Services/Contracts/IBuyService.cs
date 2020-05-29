using Shoppy.Areas.Buy.Models.DTO;
using Shoppy.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shoppy.Areas.Buy.Services.Contracts
{
    public interface IBuyService
    {
        List<BuyOfferWithTitelDTO> GetBuyOffersFromUser(int? userId);

        List<SellOfferDTO> GetRandomSellOffers(int numberOfSellOffers, int ?currentUserId);

        SellOfferDTO GetSellOfferById(int? id);

        void ValidateBuyOfferDTO(BuyOfferDTO buyOfferDTO);

        void CreateBuyOffer(BuyOfferDTO buyOfferDTO, int? userId);

        BuyOfferDTO GetBuyOfferById(int? id);

        void IncreaseUserScore(int? userId);

        decimal GetAskedMoney(int? id);

        void EditBuyOffer(EditBuyOfferDTO editBuyOfferDTO, int? userId);

        BuyOfferWithTitelDTO GetBuyOfferWithTitelByIndex(int? id);

        void DeleteBuyOffer(int? id, int? userId);

        List<SellOfferDTO> GetSellOffersByName(GetSellOfferByNameDTO getSellOfferByNameDTO, int userId);
        bool GetIsThePriceNegotiable(int? sellOfferId);
    }
}
