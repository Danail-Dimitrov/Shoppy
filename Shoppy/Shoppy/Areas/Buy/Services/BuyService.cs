using Shoppy.Areas.Buy.Models.DTO;
using Shoppy.Areas.Buy.Services.Contracts;
using Shoppy.Data;
using Shoppy.Exceptions;
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
        /// <summary>
        /// The ammount by wich a User's SupperUserScore is increasted after posting a BuyOffer
        /// </summary>
        private const int ScoreForAddingBuyOffer = 1;

        private readonly ApplicationDbContext _dbContext;

        public BuyService(ApplicationDbContext dbContext)
        {
            this._dbContext = dbContext;
        }

        /// <summary>
        /// Gets userId and calls methods validates it, gets all the BuyOffers the user has, converts them to DTOs and returns them.
        /// </summary>
        /// <param name="userId">The id of the user whoes BuyOffers need to be returned</param>
        /// <returns>List of BuyOfferDTO that contains all buy offers the given user has</returns>
        public List<BuyOfferWithTitelDTO> GetBuyOffersFromUser(int? userId)
        {
            CheckIdIsValid(userId);

            CheckUserIsDeleted(userId);

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

        /// <summary>
        /// Gets a given number of random SellOffers. Gets the number of SellOffers per page and the id of the current user than validates them. Calls method to get the random number of database entries to skip, gets the random SellOffers, convirts them to DTOs and returns them.
        /// </summary>
        /// <param name="numberOfSellOffers">The umber of SellOffers that need to be taken</param>
        /// <param name="currentUserId">The id of the current user. Needed to ensure that he will not be shown his own SellOffers</param>
        /// <returns>List of SellOfferDTOs that contains the given number of Random SellOffers.</returns>
        public List<SellOfferDTO>  GetRandomSellOffers(int numberOfSellOffersPerPage, int? currentUserId)
        {
            CheckIdIsValid(currentUserId);
            CheckUserIsDeleted(currentUserId);

            if(numberOfSellOffersPerPage <= 0)
            {
                throw new ArgumentException("The number of sell offers per page can not be less than or eaqual to zero");
            }

            int numberOfEntriesToSkip = NumberOfEntriesToSkip(numberOfSellOffersPerPage, (int)(currentUserId));

            List<SellOffer> sellOffers = this._dbContext.SellOffers.Where(x => x.CanReciveBuyOffers == true && x.UserId != currentUserId && x.HasAcceptedBuyOffer == false).Skip(numberOfEntriesToSkip).Take(numberOfSellOffersPerPage).ToList();

            List<SellOfferDTO> sellOfferDTOs = new List<SellOfferDTO>(sellOffers.Count);

            foreach(SellOffer item in sellOffers)
            {
                SellOfferDTO sellOfferDTO = ConvertSellOfferToDTO(item);
                sellOfferDTOs.Add(sellOfferDTO);
            }

            return sellOfferDTOs;
        }

        /// <summary>
        /// Gets the SellOffer id, validates it and gets the SellOffer from the database. Checks if the user who owns it is deleted. Than converts it to DTO and returns it.
        /// </summary>
        /// <param name="id">The id of the SellOffer that needs to be returned</param>
        /// <returns>SellOfferDTO, that contains the data about the given SellOffer.</returns>
        public SellOfferDTO GetSellOfferById(int? id)
        {
            CheckIdIsValid(id);      

            SellOffer sellOffer = this._dbContext.SellOffers.Find(id);

            CheckUserIsDeleted(sellOffer.UserId);

            SellOfferDTO sellOfferDTO = ConvertSellOfferToDTO(sellOffer);

            return sellOfferDTO;
        }

        /// <summary>
        /// Gets BuyOfferDTO and validates if its SellOffer is existing and that the OfferdMoney Amount is valid. If not trows an Exception.d
        /// </summary>
        /// <param name="buyOfferDTO">The BuyOfferDTO that needs to be tested.</param>
        /// <exception cref="ArgumentException">ArgumentException is thrown if the SellOffer that the DTO points to is not existing</exception>
        /// <exception cref="ArgumentException">ArgumentException is thrown if BuoyOfferDTO's OfferMoney amount is less than 0.01 or more than 9999999999</exception>
        public void ValidateBuyOfferDTO(BuyOfferDTO buyOfferDTO)
        {
            SellOffer sellOffer = this._dbContext.SellOffers.FirstOrDefault(x => x.Id == buyOfferDTO.SellOfferId);

            if(sellOffer == null)
            {
                throw new ArgumentException("The id of the sell offer is invalid");
            }
            if(buyOfferDTO.OfferedMoney < 0.01m || buyOfferDTO.OfferedMoney > 9999999999)
            {
                throw new ArgumentException("The amout of money offered must be betwen 0.01 and 9999999999");
            }
        }

        /// <summary>
        /// Creates a BuyOffer and assigns it to a User. Gets the data for the BuyOffer from a BuyOfferDTO and the userId. Validates them, creats BuyOffer bassed on the data from the DTO and saves it to the database.
        /// </summary>
        /// <param name="buyOfferDTO">The BuyOfferDTO that caries the data about the BuyOffer that needs to be created.</param>
        /// <param name="userId"></param>
        public void CreateBuyOffer(BuyOfferDTO buyOfferDTO, int? userId)
        {
            CheckIdIsValid(userId);
            CheckUserIsDeleted(userId);
            ValidateBuyOfferDTO(buyOfferDTO);

            BuyOffer buyOffer = new BuyOffer
            {
                SellOfferId = buyOfferDTO.SellOfferId,
                UserId = (int)(userId),
                OfferedMoney = buyOfferDTO.OfferedMoney
            };

            this._dbContext.BuyOffers.Add(buyOffer);
            this._dbContext.SaveChanges();
        }

        /// <summary>
        /// Gets the id of a user, validates it, gets the user the user form the database and icreases his score by ScoreForAddingBuyOffer. Than saves the changes to the database. 
        /// </summary>
        /// <param name="userId">Id of the User that needs to have his score increased.</param>
        public void IncreaseUserScore(int? userId)
        {
            CheckIdIsValid(userId);
            CheckUserIsDeleted(userId);
            User user = this._dbContext.Users.Find(userId);
            CheckUserIsNull(user);
            user.SuperUserScore += ScoreForAddingBuyOffer;
            this._dbContext.SaveChanges();
        }

        /// <summary>
        /// Gets a BuyOffer bassed on its id. Gets the id. Validates it, gets the BuyOffer, checks if the user who owns it is deleted, converts it to DTO and returns it.
        /// </summary>
        /// <param name="buyOfferId">Id of the BuyOffer that needs to be return</param>
        /// <returns>BuyOfferDTO, that caries the information about the BuyOffer</returns>
        public BuyOfferDTO GetBuyOfferById(int? buyOfferId)
        {
            CheckIdIsValid(buyOfferId);

            BuyOffer buyOffer = this._dbContext.BuyOffers.Find(buyOfferId);
            CheckUserIsDeleted(buyOffer.UserId);

            BuyOfferDTO buyOfferDTO = ConvertBuyOfferToDTO(buyOffer);

            return buyOfferDTO;
        }
         
        /// <summary>
        /// Gets the Total Price for a SellOffer. Gets the Id of the SellOffer, validates it, gets the total price from the database and returns it. 
        /// </summary>
        /// <param name="id">the id of the SellOffer, the Price of which needs to be returned</param>
        /// <returns>Returns the TotalPrice of a given SellOffer</returns>
        public decimal GetAskedMoney(int? id)
        {
            CheckIdIsValid(id);
            return this._dbContext.SellOffers.Find(id).TotalPrice;
        }

        /// <summary>
        /// Updates the data for a given BuyOffer. Gets the EditBuyOfferDTO, containing the inforation about the SellOffer and the id of the user who tries to edit it, than validates them. Gets the BuyOffer from the database and if the user, trying to edit the BuyOffer owns it, upadates the data and saves the changes to the database.
        /// </summary>
        /// <param name="editBuyOfferDTO">The DTO containing the data for the BuyOffer</param>
        /// <param name="userId">Id of the user who tryes to edit the BuyOffer</param>
        /// <exception cref="ArgumentException">ArgumentException is thrown if the amout of money offered is betwen 0.01 and 9999999999</exception>
        /// <exception cref="InvalidOperationException">InvalidOperationException if the User who tries to edit the Offer does not own it</exception>
        public void EditBuyOffer(EditBuyOfferDTO editBuyOfferDTO, int? userId)
        {
            CheckIdIsValid(userId);
            CheckUserIsDeleted(userId);
            if(editBuyOfferDTO.MoneyOffered < 0.01m || editBuyOfferDTO.MoneyOffered > 9999999999)
            {
                throw new ArgumentException("The amout of money offered must be betwen 0.01 and 9999999999");
            }

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

        /// <summary>
        /// Gets BuyOffer in form of BuyOfferDTO. Gets the id of the BuyOffer that needs to be returned, validates it, checks if the user who owns it is deleted than converts the BuyOffer to DTO and returns it.
        /// </summary>
        /// <param name="id">Id of the BuyOffer that needs to be returned</param>
        /// <returns>BuyOfferDTO that has the data for the given BuyOffer</returns>
        public BuyOfferWithTitelDTO GetBuyOfferWithTitelByIndex(int? id)
        {
            CheckIdIsValid(id);

            BuyOffer buyOffer = this._dbContext.BuyOffers.Find(id);

            CheckUserIsDeleted(buyOffer.UserId);

            string priductTitel = this._dbContext.SellOffers.Find(buyOffer.SellOfferId).ProductTitle;
            BuyOfferWithTitelDTO BuyOfferDTO = new BuyOfferWithTitelDTO(buyOffer.Id, buyOffer.OfferedMoney, buyOffer.SellOfferId, priductTitel);

            return BuyOfferDTO;
        }

        /// <summary>
        /// Deletes a given BuyOffer. Gets the id of the Offer and the id of the user who wants to delete it. Validats them and checks if the user is deleted. Gets the BuyOffer from the database. Checks if it has been accepted. Checks if the User trying to delete the Offer owns it, it gets deleted and the change is saved to the database. 
        /// </summary>
        /// <param name="id">The id of the BuyOffer that needs to be deleted.</param>
        /// <param name="userId">The id of the User who tries to delete it.</param>
        /// <exception cref="InvalidOperationException">InvalidOperationException is thrown if the User who tries to delete the BuyOffer dies not own it</exception>
        public void DeleteBuyOffer(int? id, int? userId)
        {
            CheckIdIsValid(id);
            CheckIdIsValid(userId);

            CheckUserIsDeleted(userId);

            BuyOffer buyOffer = this._dbContext.BuyOffers.Find(id);

            UnacceptOffer(buyOffer);

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

        /// <summary>
        /// Gets a given SellOffer in the form of SellOfferDTO. Recives GetSellOfferByNameDTO that contains the criteria, based on witch SellOffers need to be returned, and the id of the current user. Checks if id is valid and if user is deleted. Gets all SellOffer that match the criteria, except thoes owned by the user, so he wont be shown his own SellOffer. Converts them to DTOs and returns them.
        /// </summary>
        /// <param name="getSellOfferByNameDTO">DTO that stores the criteria for the selection of the SellOffers</param>
        /// <param name="userId">Id of the current user</param>
        /// <returns>List of SellOfferDTOs that contain the information about the SellOffer that meet the criteria.</returns>
        public List<SellOfferDTO> GetSellOffersByName(GetSellOfferByNameDTO getSellOfferByNameDTO, int userId)
        {
            CheckIdIsValid(userId);
            CheckUserIsDeleted(userId);

            List<SellOffer> sellOffers = this._dbContext.SellOffers.Where(x => x.ProductTitle == getSellOfferByNameDTO.ProductTitle && x.UserId != userId && x.HasAcceptedBuyOffer == false && x.CanReciveBuyOffers == true).ToList();

            List<SellOfferDTO> sellOfferDTOs = new List<SellOfferDTO>(sellOffers.Count);

            foreach(SellOffer item in sellOffers)
            {
                SellOfferDTO sellOfferDTO = ConvertSellOfferToDTO(item);
                sellOfferDTOs.Add(sellOfferDTO);
            }

            return sellOfferDTOs;
        }

        /// <summary>
        /// Checks if the id of the SellOffer is valid and returns a boolean telling us if the price of a SellOffer is negotiale
        /// </summary>
        /// <param name="sellOfferId">Id of the SellOffer about which we are getting the information</param>
        /// <returns>Boolean showing if the price of the SellOffer is negotiable</returns>
        public bool GetIsThePriceNegotiable(int? sellOfferId)
        {
            CheckIdIsValid(sellOfferId);
            return this._dbContext.SellOffers.Find(sellOfferId).PriceIsNegotiable;
        }

        /// <summary>
        /// Converts a BuyOffer to BuyOfferDTO
        /// </summary>
        /// <param name="buyOffer">The BuyOffer that needs to be converted</param>
        /// <param name="productTitle">The product title </param>
        /// <returns>BuyOfferDTO with the information from the BuyOffer</returns>
        private BuyOfferWithTitelDTO ConvertBuyOfferToDTO(BuyOffer buyOffer, string productTitle)
        {
            BuyOfferWithTitelDTO BuyOfferDTO = new BuyOfferWithTitelDTO(buyOffer.Id, buyOffer.OfferedMoney, buyOffer.SellOfferId, productTitle);

            return BuyOfferDTO;
        }

        /// <summary>
        /// Converts a BuyOffer to BuyOfferDTO.
        /// </summary>
        /// <param name="buyOffer">The BuyOffer that needs to be converted</param>
        /// <returns>BuyOfferDTO with the information from the BuyOffer</returns>
        private BuyOfferDTO ConvertBuyOfferToDTO(BuyOffer buyOffer)
        {
            BuyOfferDTO buyOfferDTO = new BuyOfferDTO(buyOffer.Id, buyOffer.OfferedMoney, buyOffer.SellOfferId);

            return buyOfferDTO;
        }

        /// <summary>
        /// Converts a SellOffer to SellOfferDTO.
        /// </summary>
        /// <param name="sellOffer">The SellOffer that needs to be converted</param>
        /// <returns>SellOfferDTO with the information from the SellOffer</returns>
        private SellOfferDTO ConvertSellOfferToDTO(SellOffer sellOffer)
        {
            SellOfferDTO sellOfferDTO = new SellOfferDTO(sellOffer.Id, sellOffer.ProductTitle, sellOffer.ProductDescription, sellOffer.ProductPrice, sellOffer.TotalPrice, sellOffer.PriceIsNegotiable, sellOffer.CanReciveBuyOffers, sellOffer.Quantity, null, sellOffer.HasAcceptedBuyOffer);

            return sellOfferDTO;
        }

        /// <summary>
        /// Checks if a given id is valid. If it is null or less than or eaqual to zero and Exception is thrown.
        /// </summary>
        /// <param name="id">The id that needs to be checked</param>
        /// <exception cref="ArgumentException">ArgumentException is thrown if the id is null</exception>
        /// <exception cref="ArgumentException">ArgumentException is thrown if the id is less than or eaqual to zero </exception>
        private void CheckIdIsValid(int? id)
        {
            if(id == null)
            {
                throw new ArgumentException("User id can not be null");
            }
            if(id <= 0)
            {
                throw new ArgumentException("User id can not be less than or equal zero");
            }
        }

        /// <summary>
        /// Gets the random number of entries that need to be skipped for the GetRandomSellOffers method. Gets the number of SellOffers per page, this ensures that if there are less database entries than this number all available entries will be shown and that the number of skips will not get too big, leading less SellOffers beeing displayed. Also recives the id of the current user, this is needed because a user can not be shown his SellOffers so their count must taken in considuration, because it reduces the count of the available SellOffers.
        /// </summary>
        /// <param name="numberOfSellOffers">The number telling how many SellOffers will be shown on the View.</param>
        /// <param name="userId">The id of the current user</param>
        /// <returns>A random intiger that shows how many database entries need to be skiped</returns>
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

        /// <summary>
        /// Checks if a user is deleted. Gets user id and checks the database if this user is deleted. Throws an Exception if he is.
        /// </summary>
        /// <param name="userId">Id of the user that needs to be checked if he is deleted</param>
        /// <exception cref="UserIsDeletedException">UserIsDeletedException is thrown if a user is deleted</exception>
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
        /// Gets Buyoffer unaccepted, if it has been accepted, after it is deleted. Gets the BuyOffer, checks if it has been accepted. If it is it gets unaccepted.
        /// </summary>
        /// <param name="buyOffer">BuyOffer that needs to be unaccepted</param>
        private void UnacceptOffer(BuyOffer buyOffer)
        {
            SellOffer sellOffer = this._dbContext.SellOffers.Where(x => x.AcceptedBuyOfferId == buyOffer.Id).FirstOrDefault();

            if(sellOffer != null)
            {
                sellOffer.AcceptedBuyOfferId = 0;
                sellOffer.HasAcceptedBuyOffer = false;
                this._dbContext.SaveChanges();
            }
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
