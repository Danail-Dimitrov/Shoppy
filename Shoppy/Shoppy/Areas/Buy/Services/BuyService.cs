using Shoppy.Areas.Buy.Models.DTO;
using Shoppy.Areas.Buy.Services.Contracts;
using Shoppy.Data;
using Shoppy.Models.DBEntities;
using Shoppy.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shoppy.Areas.Buy.Services
{
    public class BuyService : IBuyService
    {
        private const int ScoreForAddingBuyOffer = 1;

        private readonly ApplicationDbContext _dbContext;

        public BuyService(ApplicationDbContext dbContext)
        {
            this._dbContext = dbContext;
        }

        public List<BuyOfferWithTitelDTO> GetBuyOffersFromUser(int? userId)
        {
            CheckIdIsValid(userId);

            List<BuyOffer> buyOffers = this._dbContext.BuyOffers.Where(s => s.UserId == userId).ToList();

            List<BuyOfferWithTitelDTO> buyOfferWithTitelDTOs = new List<BuyOfferWithTitelDTO>(buyOffers.Count);            

            foreach(var buyOffer in buyOffers)
            {
                string sellOfferProductTitel = this._dbContext.SellOffers.Find(buyOffer.SellOfferId).ProductTitle;
                BuyOfferWithTitelDTO buyOfferWithTitelDTO = ConvertBuyOfferToDTO(buyOffer, sellOfferProductTitel);
                buyOfferWithTitelDTOs.Add(buyOfferWithTitelDTO);
            }

            return buyOfferWithTitelDTOs;
        }

        public List<SellOfferDTO>  GetRandomSellOffers(int numberOfSellOffers, int? currentUserId)
        {
            CheckIdIsValid(currentUserId);

            int numberOfEntriesToSkip = NumberOfEntriesToSkip(numberOfSellOffers, (int)(currentUserId));

            List<SellOffer> sellOffers = this._dbContext.SellOffers.Where(x => x.CanReciveBuyOffers == true && x.UserId != currentUserId).Skip(numberOfEntriesToSkip).Take(numberOfSellOffers).ToList();

            List<SellOfferDTO> sellOfferDTOs = new List<SellOfferDTO>(sellOffers.Count);

            foreach(SellOffer item in sellOffers)
            {
                SellOfferDTO sellOfferDTO = ConvertSellOfferToDTO(item);
                User user = this._dbContext.Users.Find(item.UserId);

                if(!user.IsDeleted)
                {
                    sellOfferDTOs.Add(sellOfferDTO);
                }              
            }

            return sellOfferDTOs;
        }

        public SellOfferDTO GetSellOfferById(int? id)
        {
            CheckIdIsValid(id);

            SellOffer sellOffer = this._dbContext.SellOffers.Find(id);

            SellOfferDTO sellOfferDTO = ConvertSellOfferToDTO(sellOffer);

            return sellOfferDTO;
        }

        public void ValidateBuyOfferDTO(BuyOfferDTO buyOfferDTO)
        {
            SellOffer sellOffer = this._dbContext.SellOffers.FirstOrDefault(x => x.Id == buyOfferDTO.SellOfferId);

            if(sellOffer == null)
            {
                throw new ArgumentException("The id of the sell offer is invalid");
            }
            if(buyOfferDTO.OfferedMoney < 0.1m || buyOfferDTO.OfferedMoney > 9999999999)
            {
                throw new ArgumentException("he amout of money offered must be betwen 0.01 and 9999999999");
            }
        }

        public void CreateBuyOffer(BuyOfferDTO buyOfferDTO, int? userId)
        {
            CheckIdIsValid(userId);

            BuyOffer buyOffer = new BuyOffer
            {
                SellOfferId = buyOfferDTO.SellOfferId,
                UserId = (int)(userId),
                OfferedMoney = buyOfferDTO.OfferedMoney
            };

            this._dbContext.BuyOffers.Add(buyOffer);
            this._dbContext.SaveChanges();
        }

        public void IncreaseUserScore(int? userId)
        {
            CheckIdIsValid(userId);
            User user = this._dbContext.Users.Find(userId);
            user.SuperUserScore += ScoreForAddingBuyOffer;
            this._dbContext.SaveChanges();
        }

        public BuyOfferDTO GetBuyOfferById(int? buyOfferId)
        {
            CheckIdIsValid(buyOfferId);

            BuyOffer buyOffer = this._dbContext.BuyOffers.Find(buyOfferId);

            BuyOfferDTO buyOfferDTO = ConvertBuyOfferToDTO(buyOffer);

            return buyOfferDTO;
        }

        public decimal GetAskedMoneyForProduct(int? id)
        {
            return this._dbContext.SellOffers.Find(id).ProductPrice;
        }

        public void EditBuyOffer(EditBuyOfferDTO editBuyOfferDTO, int? userId)
        {
            CheckIdIsValid(userId);

            BuyOffer oldBuyOffer = this._dbContext.BuyOffers.Find(editBuyOfferDTO.Id);

            if(oldBuyOffer.UserId == userId)
            {
                oldBuyOffer.OfferedMoney = editBuyOfferDTO.MoneyOffered;
                this._dbContext.SaveChanges();
            }
            else
            {
                throw new InvalidOperationException("You can not edit offers that are not yours");
            }
        }

        public BuyOfferWithTitelDTO GetBuyOfferWithTitelByIndex(int? id)
        {
            CheckIdIsValid(id);

            BuyOffer buyOffer = this._dbContext.BuyOffers.Find(id);

            string priductTitel = this._dbContext.SellOffers.Find(buyOffer.SellOfferId).ProductTitle;

            BuyOfferWithTitelDTO buyOfferWithTitelDTO = new BuyOfferWithTitelDTO(buyOffer.Id, buyOffer.OfferedMoney, buyOffer.SellOfferId, priductTitel);

            return buyOfferWithTitelDTO;
        }

        public void DeleteBuyOffer(int? id, int? userId)
        {
            CheckIdIsValid(id);
            CheckIdIsValid(userId);

            BuyOffer buyOffer = this._dbContext.BuyOffers.Find(id);

            if(buyOffer.UserId == userId)
            {
                this._dbContext.BuyOffers.Remove(buyOffer);
                this._dbContext.SaveChanges();
            }
            else
            {
                throw new InvalidOperationException("You can not delete offers that are not yours");
            }
        }

        private BuyOfferWithTitelDTO ConvertBuyOfferToDTO(BuyOffer buyOffer, string productTitle)
        {
            BuyOfferWithTitelDTO buyOfferWithTitelDTO = new BuyOfferWithTitelDTO(buyOffer.Id, buyOffer.OfferedMoney, buyOffer.SellOfferId, productTitle);

            return buyOfferWithTitelDTO;
        }

        private BuyOfferDTO ConvertBuyOfferToDTO(BuyOffer buyOffer)
        {
            BuyOfferDTO buyOfferDTO = new BuyOfferDTO(buyOffer.Id, buyOffer.OfferedMoney, buyOffer.SellOfferId);

            return buyOfferDTO;
        }

        private SellOfferDTO ConvertSellOfferToDTO(SellOffer sellOffer)
        {
            SellOfferDTO sellOfferDTO = new SellOfferDTO(sellOffer.Id, sellOffer.ProductTitle, sellOffer.ProductDescription, sellOffer.ProductPrice, sellOffer.TotalPrice, sellOffer.PriceIsNegotiable, sellOffer.CanReciveBuyOffers, sellOffer.Quantity, null);

            return sellOfferDTO;
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

        private int NumberOfEntriesToSkip(int numberOfSellOffers, int userId) 
        {
            int totalEntriesInDb = this._dbContext.SellOffers.Where(x => x.UserId != userId).Count();

            //if the total amout of offers is less than the number of sell offers per page than no entries should be skipped
            int nuberOfEntriesToSkip = 0;

            if(totalEntriesInDb > numberOfSellOffers)
            {
                Random random = new Random();
                nuberOfEntriesToSkip = random.Next(0, totalEntriesInDb - numberOfSellOffers + 1);
            }

            return nuberOfEntriesToSkip;
        }
    }
}
