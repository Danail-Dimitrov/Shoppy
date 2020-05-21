using Shoppy.Areas.Account.Models.DTO;
using Shoppy.Areas.Account.Services.Contracts;
using Shoppy.Data;
using Shoppy.Models.DBEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shoppy.Areas.Account.Services
{
    public class AccountService : IAccountService
    {
        private readonly ApplicationDbContext _dbContext;

        public AccountService(ApplicationDbContext dbContext)
        {
            this._dbContext = dbContext;
        }

        public void AddFundsToUser(int? userId, AddFundsDTO addFundsDTO)
        {
            CheckIdIsValid(userId);

            User user = this._dbContext.Users.Find(userId);

            user.Money += addFundsDTO.Money;

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
    }
}
