using Shoppy.Areas.AccountManagement.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Shoppy.Areas.AccountManagement.Services.Contracts
{
    public interface IAccountManagementService
    {
        void AddFunds(int? userId, AddFundsDTO addFundsDTO);

        AccountInfoDTO GetUserInfo(int? userId);

        UserDTO GetUserById(int? useId);

        void Delete(int userId);
    }
}
