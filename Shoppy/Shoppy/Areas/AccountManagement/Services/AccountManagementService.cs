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

        public void AddFunds(int? userId, AddFundsDTO addFundsDTO)
        {
            CheckIdIsValid(userId);
            CheckUserIsDeleted(userId);

            User user = this._dbContext.Users.Find(userId);

            CheckUserIsNotNull(user);

            user.Money += addFundsDTO.Money;
            this._dbContext.SaveChanges();
        }

        public AccountInfoDTO GetUserInfo(int? userId)
        {
            CheckIdIsValid(userId);
            CheckUserIsDeleted(userId);

            User user = this._dbContext.Users.Find(userId);

            CheckUserIsNotNull(user);

            AccountInfoDTO accountInfoDTO = new AccountInfoDTO()
            {
                Money = user.Money,
                SuperUserScore = user.SuperUserScore
            };

            return accountInfoDTO;
        }

        public async void Delete(int userId)
        {
            CheckIdIsValid(userId);
            CheckUserIsDeleted(userId);
            User user = this._dbContext.Users.Find(userId);
            CheckUserIsNotNull(user);

            user.IsDeleted = true;
            this._dbContext.SaveChanges();

            await this._signInManager.SignOutAsync();
        }

        public UserDTO GetUserById(int? userId)
        {
            CheckIdIsValid(userId);
            CheckUserIsDeleted(userId);
            User user = this._dbContext.Users.Find(userId);

            CheckUserIsNotNull(user);

            UserDTO userDTO = new UserDTO(user.UserName, user.FirstName, user.LastName, user.Money, user.SuperUserScore);

            return userDTO;
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

        private void CheckUserIsNotNull(User user)
        {
            if(user == null)
            {
                throw new UserIsNullException("User is null");
            }
        }
    }
}
