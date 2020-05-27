using Shoppy.Models.DTO;
using Shoppy.Areas.Sell.Services.Contracts;
using Shoppy.Data;
using Shoppy.Models.DBEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Shoppy.Areas.Sell.Models.DTO;
using Shoppy.Exceptions;

namespace Shoppy.Areas.Sell.Services
{
    public class SellService : ISellService
    {
        private const int ScoreForAddingOffer = 5;
        private readonly ApplicationDbContext _dbContext;

        public SellService(ApplicationDbContext dbContext)
        {
            this._dbContext = dbContext;
        }

        public List<SellOfferDTO> GetSellOffersFromUser(int? userId)
        {
            CheckIdIsValid(userId);
            CheckUserIsDeleted(userId);


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

        public void CreateSellOffer(SellOfferDTO sellOfferDTO, int? userId)
        {
            CheckIdIsValid(userId);
            CheckUserIsDeleted(userId);

            SellOffer sellOffer = new SellOffer
            {
                PriceIsNegotiable = sellOfferDTO.PriceIsNegotiable,
                BuyOffers = null,
                CanReciveBuyOffers = true,
                HasAcceptedBuyOffer = false,
                ProductPrice = sellOfferDTO.ProductPrice,
                ProductTitle = sellOfferDTO.ProductTitle,
                ProductDescription = sellOfferDTO.ProductDescription,
                Quantity = sellOfferDTO.Quantity
            };
            sellOffer.TotalPrice = sellOffer.ProductPrice * sellOffer.Quantity;
            sellOffer.UserId = (int)(userId);
            sellOffer.ProductTagSellOffers = null;

            this._dbContext.SellOffers.Add(sellOffer);

            this._dbContext.SaveChanges();
        }

        public void EditSellOffer(SellOfferDTO sellOfferDTO, int? userId)
        {
            CheckIdIsValid(userId);
            CheckUserIsDeleted(userId);

            SellOffer oldSellOffer = this._dbContext
                .SellOffers
                .FirstOrDefault(x => x.Id == sellOfferDTO.Id);

            if(oldSellOffer.UserId == userId)
            {
                oldSellOffer.ProductTitle = sellOfferDTO.ProductTitle;
                oldSellOffer.ProductDescription = sellOfferDTO.ProductDescription;
                oldSellOffer.ProductPrice = sellOfferDTO.ProductPrice;
                oldSellOffer.Quantity = sellOfferDTO.Quantity;
                oldSellOffer.PriceIsNegotiable = sellOfferDTO.PriceIsNegotiable;
                oldSellOffer.CanReciveBuyOffers = sellOfferDTO.CanReciveBuyOffers;
                oldSellOffer.TotalPrice = oldSellOffer.ProductPrice * oldSellOffer.Quantity;

                this._dbContext.SaveChanges();
            }
            else
            {
                throw new InvalidOperationException("You can not edit offers that are not yours");
            }            
        }

        public SellOfferDTO GetSellOfferById(int? id)
        {
            CheckIdIsValid(id);       

            SellOffer sellOffer = this._dbContext.SellOffers.FirstOrDefault(x => x.Id == id);

            CheckUserIsDeleted(sellOffer.UserId);

            SellOfferDTO sellOfferDTO = ConvertSellOfferToDTO(sellOffer);

            return sellOfferDTO;
        }

        public void Delete(int? id, int userId)
        {
            CheckIdIsValid(id);
            CheckIdIsValid(userId);

            SellOffer sellOffer = _dbContext.SellOffers.Find(id);
            CheckUserIsDeleted(userId);

            if(sellOffer.UserId == userId)
            {
                this._dbContext.SellOffers.Remove(sellOffer);
                this._dbContext.SaveChanges();

                RemoveAssociatedBuyOffers(id);
            }
            else
            {
                throw new InvalidOperationException("You can not delete offers that are not yours");
            }
            
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
            if(sellOfferDTO.Quantity <= 0 || sellOfferDTO.Quantity > 99)
            {
                throw new ArgumentException("The quantity of products can not be zero or negative or greter than 99");
            }
        }

        public void IncreaseUserScore(int? userId)
        {
            CheckIdIsValid(userId);
            CheckUserIsDeleted(userId);

            User user = this._dbContext.Users.Find(userId);
            user.SuperUserScore += ScoreForAddingOffer;
            this._dbContext.SaveChanges();
        }

        public List<ShowBuyOffersDTO> GetBuyOffers(int? sellOfferId)
        {
            CheckIdIsValid(sellOfferId);

            List<BuyOffer> buyOffers = this._dbContext.BuyOffers.Where(b => b.SellOfferId == sellOfferId).ToList();

            List<ShowBuyOffersDTO> showBuyOffersDTOs = new List<ShowBuyOffersDTO>(buyOffers.Count);

            decimal askingPriceOfTheSellOffer = this._dbContext.SellOffers.Find(sellOfferId).ProductPrice;

            foreach(BuyOffer buyOffer in buyOffers)
            {
                bool userIsDeleted = this._dbContext.Users.Find(buyOffer.UserId).IsDeleted;
                if(!userIsDeleted)
                {
                    showBuyOffersDTOs.Add(ConvertBuyOfferToDTO(buyOffer, askingPriceOfTheSellOffer));
                }

            }

            return showBuyOffersDTOs;
        }

        public void AcceptBuyOffer(int? buyOfferId)
        {
            CheckIdIsValid(buyOfferId);

            BuyOffer buyOffer = this._dbContext.BuyOffers.Find(buyOfferId);
            SellOffer sellOffer = this._dbContext.SellOffers.Where(x => x.Id == buyOffer.SellOfferId).FirstOrDefault();

            sellOffer.HasAcceptedBuyOffer = true;
            sellOffer.AcceptedBuyOfferId = (int)(buyOfferId);

            this._dbContext.SaveChanges();
        }

        public void FinishSale(int? sellOfferId, int? currentUserId)
        {
            CheckIdIsValid(currentUserId);
            CheckUserIsDeleted(currentUserId);

            SellOffer sellOffer = this._dbContext.SellOffers.Find(sellOfferId);
            BuyOffer buyOffer = this._dbContext.BuyOffers.Find(sellOffer.AcceptedBuyOfferId);
            User seller = this._dbContext.Users.Find(currentUserId);
            User buyer = this._dbContext.Users.Find(buyOffer.UserId);

            decimal moneyOfferd = buyOffer.OfferedMoney;
            if(buyer.Money < moneyOfferd)
            {
                throw new BuyerHasInsufficientFundsException("Buyer has insufficient funds!");
            }
            seller.Money += moneyOfferd;
            buyer.Money -= moneyOfferd; 

            TransactionHistory sellerTransactionHistory = new TransactionHistory(DateTime.Now, sellOffer.ProductTitle, true, buyOffer.OfferedMoney, (int)(currentUserId));
            TransactionHistory buyerTransactionHistory = new TransactionHistory(DateTime.Now, sellOffer.ProductTitle, false, buyOffer.OfferedMoney, buyer.Id);
            this._dbContext.TransactionHistories.Add(sellerTransactionHistory);
            this._dbContext.TransactionHistories.Add(buyerTransactionHistory);

            List<BuyOffer> buyOffers = this._dbContext.BuyOffers.Where(x => x.SellOfferId == sellOffer.Id).ToList();
            this._dbContext.RemoveRange(buyOffers);
            this._dbContext.SellOffers.Remove(sellOffer);

            this._dbContext.SaveChanges();
        }

        private ShowBuyOffersDTO ConvertBuyOfferToDTO(BuyOffer buyOffer, decimal askedPrice)
        {
            ShowBuyOffersDTO buyOfferDTO = new ShowBuyOffersDTO(buyOffer.Id, buyOffer.OfferedMoney, buyOffer.SellOfferId, askedPrice);

            return buyOfferDTO;
        }

        private SellOfferDTO ConvertSellOfferToDTO(SellOffer sellOffer)
        {
            SellOfferDTO sellOfferDTO = new SellOfferDTO(sellOffer.Id, sellOffer.ProductTitle,sellOffer.ProductDescription, sellOffer.ProductPrice, sellOffer.TotalPrice, sellOffer.PriceIsNegotiable, sellOffer.CanReciveBuyOffers, sellOffer.Quantity, null, sellOffer.HasAcceptedBuyOffer);

            return sellOfferDTO;
        }

        private void RemoveAssociatedBuyOffers(int? sellOfferId) 
        {
            this._dbContext.BuyOffers.RemoveRange(this._dbContext.BuyOffers.Where(x => x.SellOfferId == sellOfferId));
            this._dbContext.SaveChanges();
        }

        private void CheckIdIsValid(int? id)
        {
            if(id == null)
            {
                throw new ArgumentException("User id can not be null");
            }
            if(id <= 0)
            {
                throw new ArgumentException("User id can not be less than zero");
            }
        }

        private void CheckUserIsDeleted(int? userId)
        {
            bool userIsDeleted = this._dbContext.Users.Find(userId).IsDeleted;
            if(userIsDeleted)
            {
                throw new UserIsDeletedException("The user is deleted");
            }
        }
    }
}
