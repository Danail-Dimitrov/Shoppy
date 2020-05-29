using Microsoft.AspNetCore.Identity;
using Shoppy.Areas.AccountManagement.Models.DTO;
using Shoppy.Areas.AccountManagement.Services.Contracts;
using Shoppy.Data;
using Shoppy.Exceptions;
using Shoppy.Models.DBEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shoppy.Areas.AccountManagement.Services
{
    public class AccountManagementService : IAccountManagementService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly SignInManager<User> _signInManager;

        public AccountManagementService(ApplicationDbContext applicationDbContext, SignInManager<User> signInManager)
        {
            this._dbContext = applicationDbContext;
            this._signInManager = signInManager;
        }

        /// <summary>
        /// Adds funds to a givenUser. Gets the id o the current user and AddFundsDTO, carying information about the funds. Gets the User from the database, checks if he is null, adds the funds to him and saves changes.
        /// </summary>
        /// <param name="userId">Id of the user to whom the funds must be added</param>
        /// <param name="addFundsDTO">AddFundsDTO, storing information about the funds, that need to be added</param>
        public void AddFunds(int? userId, AddFundsDTO addFundsDTO)
        {
            CheckIdIsValid(userId);
            CheckUserIsDeleted(userId);
            if(addFundsDTO.Money <= 0)
            {
                throw new ArgumentException("The Amount of money that you add can not be less than or equal to zero");
            }
            User user = this._dbContext.Users.Find(userId);

            user.Money += addFundsDTO.Money;
            this._dbContext.SaveChanges();
        }

        /// <summary>
        /// Gets information about the Money and SuperUserScore of a user. Gets id of a user, checks it, checks if user is deleted, gets user from data base, checks that he is not null, saves the data in AccountInfoDTO and returns it.
        /// </summary>
        /// <param name="userId">Id of the user whoes information must be returned</param>
        /// <returns>AccountInfoDTO containing the information about the user</returns>
        public AccountInfoDTO GetUserInfo(int? userId)
        {
            CheckIdIsValid(userId);
            CheckUserIsDeleted(userId);

            User user = this._dbContext.Users.Find(userId);            

            AccountInfoDTO accountInfoDTO = new AccountInfoDTO()
            {
                Money = user.Money,
                SuperUserScore = user.SuperUserScore
            };

            return accountInfoDTO;
        }

        /// <summary>
        /// Deletes a given User. Gets id of a user, checks it, checks if user is deleted, gets user from data base, checks that he is not null, calls method to Remove his Orders, deletes the user, saves the changes to the database and signs the user out
        /// </summary>
        /// <param name="userId">Id of the User that needs to be deleted</param>
        public async void Delete(int? userId)
        {
            CheckIdIsValid(userId);
            CheckUserIsDeleted(userId);
            User user = this._dbContext.Users.Find(userId);

            RemoveAllOrders(userId);

            user.IsDeleted = true;
            this._dbContext.SaveChanges();

            await this._signInManager.SignOutAsync();
        }

        /// <summary>
        /// Gets a given User in form og UserDTO. Gets id of a user, checks it, checks if user is deleted, gets user from data base, checks that he is not null, converts the User to UserDTO and returns it
        /// </summary>
        /// <param name="userId">Id of a user whoes information must be returned</param>
        /// <returns>UserDTO containing the information about a given User</returns>
        public UserDTO GetUserById(int? userId)
        {
            CheckIdIsValid(userId);
            CheckUserIsDeleted(userId);
            User user = this._dbContext.Users.Find(userId);

            UserDTO userDTO = new UserDTO(user.UserName, user.FirstName, user.LastName, user.Money, user.SuperUserScore);

            return userDTO;
        }

        /// <summary>
        /// Removes all Offers from a given User. Gets the id of a User, checks if it is valid, checks if user is deleted, gets all SellOffers and BuyOffers from him, removes them and saves the changes to the database.
        /// </summary>
        /// <param name="userId">Id of the User whoes Offers must be removed</param>
        public void RemoveAllOrders(int? userId)
        {
            CheckIdIsValid(userId);
            CheckUserIsDeleted(userId);

            List<BuyOffer> buyOffers = this._dbContext.BuyOffers.Where(x => x.UserId == userId).ToList();
            List<SellOffer> sellOffers = this._dbContext.SellOffers.Where(x => x.UserId == userId).ToList();

            this._dbContext.RemoveRange(buyOffers);
            this._dbContext.RemoveRange(sellOffers);
            this._dbContext.SaveChanges();
        }

        /// <summary>
        /// Returns all TransactionHistories from a given User. Gets the id of a User, checks if it is valid, checks if user is deleted, gets the TransactionHistories, converts them to DTOs and returns them.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>List of TransactionHistoryDTOs containing information about the TransactionHistories of the user</returns>
        public List<TransactionHistoryDTO> GetTransactionHistories(int? userId)
        {
            CheckIdIsValid(userId);
            CheckUserIsDeleted(userId);

            List<TransactionHistory> transactionHistories = this._dbContext.TransactionHistories.Where(x => x.UserId == userId).ToList();
            List<TransactionHistoryDTO> transactionHistoryDTOs = new List<TransactionHistoryDTO>(transactionHistories.Count);

            foreach(var transactionHistory in transactionHistories)
            {
                transactionHistoryDTOs.Add(ConverTransactionHistoryToDTO(transactionHistory));
            }

            return transactionHistoryDTOs;
        }

        /// <summary>
        /// Gets the total profi of a User from all his transactions.  Gets the id of a User, checks if it is valid, checks if user is deleted, gets all the TransactionHistory and calculates the total profit. Then returns it
        /// </summary>
        /// <param name="userId">Id of the User whoes profit needs to be calculated</param>
        /// <returns>The profit of a given User</returns>
        public decimal GetProfit(int? userId)
        {
            CheckIdIsValid(userId);
            CheckUserIsDeleted(userId);

            List<TransactionHistory> transactionHistories = this._dbContext.TransactionHistories.Where(x => x.UserId == userId).ToList();
            decimal profit = 0m;

            foreach(var transactionHistory in transactionHistories)
            {
                if(transactionHistory.IsProvit)
                {
                    profit += transactionHistory.MoneyAmaount;
                }
                else
                {
                    profit -= transactionHistory.MoneyAmaount;
                }
            }

            return profit;
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
                throw new ArgumentException("User id can not be less than zero");
            }
        }

        /// <summary>
        /// Checks if a user is deleted. Gets user id and checks the database if this user is deleted. Throws an Exception if he is.
        /// </summary>
        /// <param name="userId">Id of the user that needs to be checked if he is deleted</param>
        /// <exception cref="UserIsDeletedException">UserIsDeletedException is thrown if a user is deleted</exception>
        private void CheckUserIsDeleted(int? userId)
        {
            CheckUserIsNotNull((int)(userId));
            bool userIsDeleted = this._dbContext.Users.Find(userId).IsDeleted;
            if(userIsDeleted)
            {
                throw new UserIsDeletedException("The user is deleted");
            }
        }

        /// <summary>
        /// Checks if a user is null. Gets user and if he is null an Exception is thrown.
        /// </summary>
        /// <param name="user">User that is going to be checked</param>
        /// <exception cref="UserIsNullException">UserIsNullException if the user is null</exception>
        private void CheckUserIsNotNull(int userId)
        {
            User user = this._dbContext.Users.Find(userId);
            if(user == null)
            {
                throw new UserIsNullException("User is null");
            }
        }

        /// <summary>
        /// Transforms TransactionHistory to DTO.
        /// </summary>
        /// <param name="transactionHistory">TransactionHistory that needs to be converted</param>
        /// <returns>TransactionHistoryDTO with the information from the TransactionHistory</returns>
        private TransactionHistoryDTO ConverTransactionHistoryToDTO(TransactionHistory transactionHistory)
        {
            TransactionHistoryDTO transactionHistoryDTO = new TransactionHistoryDTO
            {
                Date = transactionHistory.Date,
                IsProvit = transactionHistory.IsProvit,
                MoneyAmaount = transactionHistory.MoneyAmaount,
                ProductTitle = transactionHistory.ProductTitle
            };

            return transactionHistoryDTO;
        }
    }
}
