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
        private readonly ApplicationDbContext _dbContext;

        public SellService()
        {

        }

        public SellService(ApplicationDbContext dbContext)
        {
            this._dbContext = dbContext;
        }
     
        public List<SellOfferDTO> GetOffersFromUser(int userId)
        {            
           List<SellOffer> sellOffers = _dbContext.SellOffers
                       .Where(s => s.UserId == userId)
                       .ToList();

            List<SellOfferDTO> sellOfferDTOs = new List<SellOfferDTO>(sellOffers.Count);

            foreach(SellOffer item in sellOffers)
            {
                sellOfferDTOs.Add(ConvertSellOfferToDTO(item));
            }

            return sellOfferDTOs;
        }

        public void CreateSellOffer(SellOfferDTO sellOfferDTO, int userId)
        {
            SellOffer sellOffer = new SellOffer
            {
                PriceIsNegotiable = sellOfferDTO.PriceIsNegotiable,
                BuyOffers = null,
                CanReciveBuyOffers = true,
                ProductPrice = sellOfferDTO.ProductPrice,
                ProductTitle = sellOfferDTO.ProductTitle,
                Quantity = sellOfferDTO.Quantity
            };
            sellOffer.TotalPrice = sellOffer.ProductPrice * sellOffer.Quantity;
            sellOffer.UserId = userId;
            sellOffer.ProductTagSellOffers = null;

            this._dbContext.SellOffers.Add(sellOffer);

            this._dbContext.SaveChanges();
        }

        public void EditSellOffer (SellOfferDTO sellOfferDTO)
        {
            SellOffer oldSellOffer = this._dbContext
                .SellOffers
                .FirstOrDefault(x => x.Id == sellOfferDTO.Id);

            oldSellOffer.ProductTitle = sellOfferDTO.ProductTitle;
            oldSellOffer.ProductPrice = sellOfferDTO.ProductPrice;
            oldSellOffer.Quantity = sellOfferDTO.Quantity;
            oldSellOffer.PriceIsNegotiable = sellOfferDTO.PriceIsNegotiable;
            oldSellOffer.CanReciveBuyOffers = sellOfferDTO.CanReciveBuyOffers;
            oldSellOffer.TotalPrice = oldSellOffer.ProductPrice * oldSellOffer.Quantity;

            this._dbContext.SaveChanges();
        }
     
        public SellOfferDTO GetSellOfferById (int id)
        {
            SellOffer sellOffer = this._dbContext.SellOffers.FirstOrDefault(x => x.Id == id);

            SellOfferDTO sellOfferDTO = ConvertSellOfferToDTO(sellOffer);

            return sellOfferDTO;
        }

        private SellOfferDTO ConvertSellOfferToDTO (SellOffer sellOffer)
        {
            SellOfferDTO sellOfferDTO = new SellOfferDTO(sellOffer.Id, sellOffer.ProductTitle, sellOffer.ProductPrice, sellOffer.TotalPrice, sellOffer.PriceIsNegotiable, sellOffer.CanReciveBuyOffers, sellOffer.Quantity, null);

            return sellOfferDTO;
        }

        public void ValidateSellOfferDTO(SellOfferDTO sellOfferDTO)
        {
            if(sellOfferDTO.Id < 0)
            {
                throw new ArgumentException("The Id of a product can not less than zero");
            }
            if(String.IsNullOrEmpty(sellOfferDTO.ProductTitle))
            {
                throw new ArgumentException("The Titel of a product can not be null or empty");
            }
            if(sellOfferDTO.ProductTitle.Length < 3)
            {
                throw new ArgumentException("The Titel of a product can not shorter than 3 charecters");
            }
            if(sellOfferDTO.ProductPrice <= 0 || sellOfferDTO.ProductPrice > 9999999999)
            {
                throw new ArgumentException("The price for a single product can not be zero or negative or greter than 9999999999");
            }
            if(sellOfferDTO.Quantity <= 0 || sellOfferDTO.Quantity > 999999)
            {
                throw new ArgumentException("The quantity of products can not be zero or negative or greter than 999999");
            }
        }
    }
}
