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
    /// <summary>
    /// SellService handels all the buisnes logic for making a sell offer
    /// </summary>
    public class SellService : ISellService
    {
        /// <summary>
        /// The minimum score a user needs to have to be a SuperUser
        /// </summary>
        private const int SupperUserMinScore = 100;
        /// <summary>
        /// The ammount by wich a User's SupperUserScore is increasted after posting a SellOffer
        /// </summary>
        private const int ScoreForAddingOffer = 5;
        private readonly ApplicationDbContext _dbContext;

        public SellService(ApplicationDbContext dbContext)
        {
            this._dbContext = dbContext;
        }

        /// <summary>
        /// Calls methodes to validate, that userId is valid and the User is not deleted. Gets all the SellOffers the user has and calls methodes to convert them to SellOfferDTOs.
        /// </summary>
        /// <param name="userId">Id of the User whoes offers need to be shown.</param>
        /// <returns>List of SellOfferDTO containing all the SellOffers this user has.</returns>
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

        /// <summary>
        /// Calls methodes to validate, that userId is valid and the User is not deleted and that the SellOfferDTO is valid. Creats SellOffer with the data from the dto and adds it to the database.
        /// </summary>
        /// <param name="sellOfferDTO">SellOfferDTO stores the information about the SellOffer that needs to be created.</param>
        /// <param name="userId">The id of the user that creats the SellOffer.</param>
        public void CreateSellOffer(SellOfferDTO sellOfferDTO, int? userId)
        {
            CheckIdIsValid(userId);
            CheckUserIsDeleted(userId);
            ValidateSellOfferDTO(sellOfferDTO);

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

        /// <summary>
        /// Calls methodes to validate, that userId is valid and the User is not deleted and that the SellOfferDTO is valid. Gets the SellOffer that needs to be edited, using the SellOfferDTO id, if the SellOffer belongs to the user that tries to edit it, it gets updated and saved to the database. If not an Exception is thrown 
        /// </summary>
        /// <param name="sellOfferDTO">SellOfferDTO stores the information about the SellOffer that needs to be edited.</param>
        /// <param name="userId">The id of the user that edits the SellOffer.</param>
        /// <exception cref="InvalidOperationException">InvalidOperationException is thrown if a user tries to edit SellOffer that is not his</exception>
        public void EditSellOffer(SellOfferDTO sellOfferDTO, int? userId)
        {
            CheckIdIsValid(userId);
            CheckUserIsDeleted(userId);
            ValidateSellOfferDTO(sellOfferDTO);

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

        /// <summary>
        /// Calls methodes to validate, that the id of the SellOffer is valid. Gets the SellOffer from the database, checks if its user is deleted, converts the SellOffer to DTO.
        /// </summary>
        /// <param name="id">The id of the SellOffer, that needs to be returned</param>
        /// <returns>SellOfferDTO cotaining the information about the SellOffer with the coresponding id</returns>
        public SellOfferDTO GetSellOfferById(int? id)
        {
            CheckIdIsValid(id);

            SellOffer sellOffer = this._dbContext.SellOffers.FirstOrDefault(x => x.Id == id);

            CheckUserIsDeleted(sellOffer.UserId);

            SellOfferDTO sellOfferDTO = ConvertSellOfferToDTO(sellOffer);

            return sellOfferDTO;
        }

        /// <summary>
        /// Calls methodes to validate, that userId is valid and the User is not deleted. Gets the SellOffer that needs to be deleted from the database, if the user owns it removes it and saves the changes to the database.If not throws Exception. Finally calls method to remove the BuyOffers made for this SellOffer. 
        /// </summary>
        /// <param name="id">Id of the SellOffer that needs to be deleted</param>
        /// <param name="userId">Id of the User that wants to delete the SellOffer</param>
        /// <exception cref="InvalidOperationException">InvalidOperationException is thrown if a userer tries to delet SellOffer that is not his</exception>
        public void Delete(int? id, int userId)
        {
            CheckIdIsValid(id);
            CheckIdIsValid(userId);

            SellOffer sellOffer = _dbContext.SellOffers.Find(id);

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

        /// <summary>
        /// Validates that the data inside the SellOfferDTo. If it is incorect a coresponding Exception is thrown.
        /// </summary>
        /// <param name="sellOfferDTO">The SellOfferDTO that needs to be validated</param>
        /// <exception cref="ArgumentException">ArgumentException is thrown if the Id of a product is less than zero</exception>
        /// <exception cref="ArgumentException">ArgumentException is thrown if the Titel of a product is null or empty</exception>
        /// <exception cref="ArgumentException">ArgumentException is thrown if the Titel of a product is shorter than 3 charecters or grater than 999999</exception>
        /// <exception cref="ArgumentException">ArgumentException is thrown if the price for a single product is zero or negative or greter than 9999999999</exception>
        /// <exception cref="ArgumentException">ArgumentException is thrown if the quantity of products is zero or negative or greter than 99</exception>
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
            if(sellOfferDTO.ProductTitle.Length < 3 || sellOfferDTO.ProductTitle.Length > 999999)
            {
                throw new ArgumentException("The Titel of a product can not shorter than 3 charecters or grater than 999999");
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

        /// <summary>
        /// Calls methodes to validate, that userId is valid and the User is not deleted. Gets the User fromt the database, increases his SuperUserScore and saves the changes to the database.
        /// </summary>
        /// <param name="userId">The id of the user that is going to have his score increased</param>
        public void IncreaseUserScore(int? userId)
        {
            CheckIdIsValid(userId);
            CheckUserIsDeleted(userId);

            User user = this._dbContext.Users.Find(userId);
            CheckUserIsNull(user);

            user.SuperUserScore += ScoreForAddingOffer;
            this._dbContext.SaveChanges();
        }

        /// <summary>
        /// Validates the id of the SellOffer. Gets all BuyOffers made for it transfers them to DTOs .
        /// </summary>
        /// <param name="sellOfferId">Id of the SellOffer the BuyOffers for which will be taken</param>
        /// <returns>List of BuyOfferDTOs.</returns>
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

        /// <summary>
        /// Calls methodes to validate, that userId is valid and the User is not deleted. Validates that BuyOfferId is valid. Gets The buyOffer, bassed on its Id, than Gets the SellOffer that it is for, makes sre the user owns it, if not an Exception is thrown, and Marks that the sellOffer has AcceptedBuyOffer. Saves changes to the database.
        /// </summary>
        /// <param name="buyOfferId">Id off the BuyOffer that is beeing marked a accepted</param>
        /// <param name="userId">Id of the users that tries to accept it</param>
        /// <exception cref="ArgumentException">ArgumentException is thrown if a user tries to accept a BuyOffer for a SellOffer that is not his</exception>
        public void AcceptBuyOffer(int? buyOfferId, int? userId)
        {
            CheckIdIsValid(buyOfferId);
            CheckIdIsValid(userId);

            BuyOffer buyOffer = this._dbContext.BuyOffers.Find(buyOfferId);
            SellOffer sellOffer = this._dbContext.SellOffers.Where(x => x.Id == buyOffer.SellOfferId).FirstOrDefault();

            if(sellOffer.UserId != userId)
            {
                throw new InvalidOperationException("You can not accept offers for SellOffers you do not own");
            }

            sellOffer.HasAcceptedBuyOffer = true;
            sellOffer.AcceptedBuyOfferId = (int)(buyOfferId);

            this._dbContext.SaveChanges();
        }

        /// <summary>
        /// Checks the id, that have been given to the method, if the buyer has insufficient funds an Exception is thrown ,compleats the transaction, calls method to change Users balances acordingly and deletes the offers
        /// </summary>
        /// <param name="sellOfferId">Id of the SellOffer the BuyOffer of witch will be accepted</param>
        /// <param name="currentUserId">Id of the User that wants to finish the order</param>
        /// <exception cref="BuyerHasInsufficientFundsException">BuyerHasInsufficientFundsException is thrown if the buyer has insufficient funds</exception>
        public void FinishSale(int? sellOfferId, int? currentUserId)
        {
            CheckIdIsValid(currentUserId);
            CheckUserIsDeleted(currentUserId);

            SellOffer sellOffer = this._dbContext.SellOffers.Find(sellOfferId);
            BuyOffer buyOffer = this._dbContext.BuyOffers.Find(sellOffer.AcceptedBuyOfferId);
            User seller = this._dbContext.Users.Find(currentUserId);
            User buyer = this._dbContext.Users.Find(buyOffer.UserId);

            CheckUserIsNull(seller);
            CheckUserIsNull(buyer);

            decimal moneyOfferd = buyOffer.OfferedMoney;
            if(buyer.Money < moneyOfferd)
            {
                throw new BuyerHasInsufficientFundsException("Buyer has insufficient funds!");
            }

            ChangeUserBalance(seller, false, moneyOfferd);
            ChangeUserBalance(buyer, true, moneyOfferd);


            TransactionHistory sellerTransactionHistory = new TransactionHistory(DateTime.Now, sellOffer.ProductTitle, true, buyOffer.OfferedMoney, (int)(currentUserId));
            TransactionHistory buyerTransactionHistory = new TransactionHistory(DateTime.Now, sellOffer.ProductTitle, false, buyOffer.OfferedMoney, buyer.Id);
            this._dbContext.TransactionHistories.Add(sellerTransactionHistory);
            this._dbContext.TransactionHistories.Add(buyerTransactionHistory);

            List<BuyOffer> buyOffers = this._dbContext.BuyOffers.Where(x => x.SellOfferId == sellOffer.Id).ToList();
            this._dbContext.RemoveRange(buyOffers);
            this._dbContext.SellOffers.Remove(sellOffer);

            this._dbContext.SaveChanges();
        }

        /// <summary>
        /// Removes the accepted BuyOffer, after checking if the ids are valid and asuring that the user who tryes to Unaccept a BuyOffer owns the SellOffer, if not throws an Exception
        /// </summary>
        /// <param name="sellOferId">Id of the Selloffer that will be unaccepting the buyOffer</param>
        /// <param name="userId">Id of the User that wants to unaccept the buyOffer</param>
        /// <exception cref="ArgumentException">ArgumentException is thrown is a user tries to unaccerpt a BuyOffer for somone's SellOffer</exception>
        public void UnaccpetBuyOffer(int? sellOferId, int? userId)
        {
            CheckIdIsValid(sellOferId);
            CheckIdIsValid(userId);
            SellOffer sellOffer = this._dbContext.SellOffers.Find(sellOferId);
            if(sellOffer.UserId != userId)
            {
                throw new InvalidOperationException("You can not unaccept offers for SellOffers you do not own");
            }

            sellOffer.HasAcceptedBuyOffer = false;
            sellOffer.AcceptedBuyOfferId = 0;

            this._dbContext.SaveChanges();
        }

        /// <summary>
        /// Converts the BuyOffer to ShowBuyOffersDTO. This DTO stores information about the offered money and the asked money. {Used in the View showing buy offers}
        /// </summary>
        /// <param name="buyOffer">BuyOffer that needs to be converted to DTO</param>
        /// <param name="askedPrice">The price of a product in the SellOffer</param>
        /// <returns>ShowBuyOffersDTO with the information of the given BuyOffer</returns>
        private ShowBuyOffersDTO ConvertBuyOfferToDTO(BuyOffer buyOffer, decimal askedPrice)
        {
            ShowBuyOffersDTO buyOfferDTO = new ShowBuyOffersDTO(buyOffer.Id, buyOffer.OfferedMoney, buyOffer.SellOfferId, askedPrice);

            return buyOfferDTO;
        }

        /// <summary>
        /// Coverts a SellOffer to SellOfferDTO, containing all the information about the SellOffer.
        /// </summary>
        /// <param name="sellOffer">SellOffer that needs to be converted</param>
        /// <returns>SellOfferDTO with the information about the given SellOffer</returns>
        private SellOfferDTO ConvertSellOfferToDTO(SellOffer sellOffer)
        {
            SellOfferDTO sellOfferDTO = new SellOfferDTO(sellOffer.Id, sellOffer.ProductTitle, sellOffer.ProductDescription, sellOffer.ProductPrice, sellOffer.TotalPrice, sellOffer.PriceIsNegotiable, sellOffer.CanReciveBuyOffers, sellOffer.Quantity, null, sellOffer.HasAcceptedBuyOffer);

            return sellOfferDTO;
        }

        /// <summary>
        /// Removes all the BuyOffers Associated with a given SellOffer
        /// </summary>
        /// <param name="sellOfferId">Id of the SellOffer the BuysOffers of wich will be removed</param>
        private void RemoveAssociatedBuyOffers(int? sellOfferId)
        {
            this._dbContext.BuyOffers.RemoveRange(this._dbContext.BuyOffers.Where(x => x.SellOfferId == sellOfferId));
            this._dbContext.SaveChanges();
        }

        /// <summary>
        /// Checks if id is valid and if not throws Exception.
        /// </summary>
        /// <param name="id">The Id that needs to e validated</param>
        /// <exception cref="ArgumentException">ArgumentException is beeing thrown if the id is null</exception>
        /// <exception cref="ArgumentException">ArgumentException is beeing thrown if the id is less than or equal to zero</exception>
        private void CheckIdIsValid(int? id)
        {
            if(id == null)
            {
                throw new ArgumentException("Id can not be null");
            }
            if(id <= 0)
            {
                throw new ArgumentException("Id can not be less than or equal to zero");
            }
        }

        /// <summary>
        /// Checks if the a given User is deleted , if yes throws Exception
        /// </summary>
        /// <param name="userId">The id of the User that is going to be checked</param>
        /// <exception cref="UserIsDeletedException">UserIsDeletedException is thrown if the user is deleted</exception>
        private void CheckUserIsDeleted(int? userId)
        {
            User user = this._dbContext.Users.Find(userId);
            CheckUserIsNull(user);
            bool userIsDeleted = user.IsDeleted;
            if(userIsDeleted)
            {
                throw new UserIsDeletedException("The user is deleted");
            }
        }

        /// <summary>
        /// Changes the balance to a user. If he is a buyer his balance is decreased, if not - increased.
        /// </summary>
        /// <param name="user">The user that needs to have his balance changed</param>
        /// <param name="isBuyer">Boolean that shows if a user is a Buyer</param>
        /// <param name="money">The ammount that needs to be added or subtracted from a User balance</param>
        private void ChangeUserBalance(User user, bool isBuyer, decimal money)
        {
            if(isBuyer)
            {
                if(CheckIfUserIsSupper(user))
                {
                    decimal superUserBonus = money * 10 / 100;
                    money -= superUserBonus;
                }
                user.Money -= money;
            }
            else
            {
                if(CheckIfUserIsSupper(user))
                {
                    decimal superUserBonus = money * 10 / 100;
                    money += superUserBonus;
                }
                user.Money += money;
            }

            this._dbContext.SaveChanges();
        }

        /// <summary>
        /// Checks if a User is a SupperUser
        /// </summary>
        /// <param name="user">A user that needs to be checked if it is a SuperUser</param>
        /// <returns>Bollean Showing if a User is a SuperUser</returns>
        private bool CheckIfUserIsSupper(User user)
        {
            return user.SuperUserScore >= SupperUserMinScore;
        }

        /// <summary>
        /// Checks if a given user is null. If he is Exceprio is thrown.
        /// </summary>
        /// <param name="user">The user that needs to be checked</param>
        /// <exception cref="UserIsNullException">UserIsNullException is trown if the user is null</exception>
        private void CheckUserIsNull(User user)
        {
            if(user == null)
            {
                throw new UserIsNullException("The user is null");
            }
        }
    }
}
