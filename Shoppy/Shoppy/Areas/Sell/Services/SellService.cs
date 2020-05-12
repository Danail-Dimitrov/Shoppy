using Shoppy.Areas.Sell.Models.DTO;
using Shoppy.Data;
using Shoppy.Models.DBEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shoppy.Areas.Sell.Services
{
    public class SellService
    {
        private readonly ApplicationDbContext dbContext;

        public SellService()
        {

        }

        public SellService(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
     
        public List<SellOffer> GetOffersFromUser(int userId)
        {            
           List<SellOffer> sellOffers = dbContext.SellOffers
                       .Where(s => s.UserId == userId)
                       .ToList();            

            return sellOffers;
        }

        public void CreateSellOffer(CreateSellOfferDTO sellOfferDTO, int userId)
        {
            SellOffer sellOffer = new SellOffer();
            sellOffer.PriceIsNegotiable = sellOfferDTO.PriceIsNegotiable;
            sellOffer.BuyOffers = null;
            sellOffer.CanReciveBuyOffers = true;
            sellOffer.ProductPrice = sellOfferDTO.ProductPrice;
            sellOffer.ProductTitle = sellOfferDTO.ProductTitle;
            sellOffer.Quantity = sellOfferDTO.Quantity;
            sellOffer.TotalPrice = sellOffer.ProductPrice * sellOffer.Quantity;
            sellOffer.UserId = userId;
            sellOffer.ProductTagSellOffers = null;

            this.dbContext.SellOffers.Add(sellOffer);

            this.dbContext.SaveChanges();
        }

        public void ValidateCreatSellOfferDTO(CreateSellOfferDTO sellOfferDTO)
        {
            if(String.IsNullOrEmpty(sellOfferDTO.ProductTitle))
            {
                throw new ArgumentException("The Titel of a product can not be null or empty");
            }
            if(sellOfferDTO.ProductPrice <= 0)
            {
                throw new ArgumentException("The price for a single product can not be zero or negative");
            }
            if(sellOfferDTO.Quantity <= 0)
            {
                throw new ArgumentException("The quantity of products can not be zero or negative");
            }
        }

    }
}
