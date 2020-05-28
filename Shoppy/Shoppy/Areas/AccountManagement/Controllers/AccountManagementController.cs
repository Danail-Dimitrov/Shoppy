using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Shoppy.Areas.AccountManagement.Models.DTO;
using Shoppy.Areas.AccountManagement.Services;
using Shoppy.Areas.AccountManagement.Services.Contracts;
using Shoppy.Exceptions;

namespace Shoppy.Areas.AccountManagement.Controllers
{
    public class AccountManagementController : Controller
    {
        private readonly IAccountManagementService _managementService;

        public AccountManagementController(AccountManagementService managementService)
        {
            _managementService = managementService;

        }

        /// <summary>
        /// Gets the AccountInfo for the current user from AccountManagementService, stores it in AccountInfoDTO and passes it to the IndexView. This is the Index for this controller.
        /// </summary>
        /// <returns>IndexView and gives it AccountInfoDTO, carying the user info. If there is an Exception thrown redirects to ErrorController.</returns>
        [Authorize]
        [HttpGet]
        [Area("AccountManagement")]
        public IActionResult Index()
        {
            try
            {
                int userId = int.Parse(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
                AccountInfoDTO accountInfoDTO = this._managementService.GetUserInfo(userId);

                return View(accountInfoDTO);
            }
            catch(ArgumentException ex)
            {
                TempData["ExceptionMessege"] = ex.Message;
                TempData["TheErrorHappendWhen"] = "adding funds";
                return RedirectToAction("AccountManagementError", "Error", new { area = "Error" });
            }
            catch(InvalidOperationException ex)
            {
                TempData["ExceptionMessege"] = ex.Message;
                TempData["TheErrorHappendWhen"] = "adding funds";
                return RedirectToAction("AccountManagementError", "Error", new { area = "Error" });
            }
            catch(UserIsNullException ex)
            {
                TempData["Messege"] = ex.Message;
                return RedirectToAction("AccountManagementError", "Error", new { area = "Error" });
            }
            catch(UserIsDeletedException ex)
            {
                TempData["Messege"] = ex.Message;
                return RedirectToAction("AccountManagementError", "Error", new { area = "Error" });
            }
        }

        /// <summary>
        /// Shows the AddFundsView, used for adding funds to the account.
        /// </summary>
        /// <returns>AddFundsView</returns>
        [Authorize]
        [HttpGet]
        [Area("AccountManagement")]
        public IActionResult AddFunds()
        {
            return View();
        }

        /// <summary>
        /// Gets AddFundsDTO, that has the info about adding Funds to a users account, passes it to AccountManagementService and redirects to Index. If the Service throws an Exception ErrorController is called.
        /// </summary>
        /// <param name="addFundsDTO">The DTO that stores the information about funds adding</param>
        /// <returns>Redirects to Index. If there is an Exception thrown redirects to ErrorController.</returns>
        [Authorize]
        [HttpPost]
        [Area("AccountManagement")]
        public IActionResult AddFunds(AddFundsDTO addFundsDTO)
        {
            try
            {
                int userId = int.Parse(this.User.FindFirstValue(ClaimTypes.NameIdentifier));

                this._managementService.AddFunds(userId, addFundsDTO);

                return RedirectToAction(nameof(Index));
            }
            catch(ArgumentException ex)
            {
                TempData["ExceptionMessege"] = ex.Message;
                TempData["TheErrorHappendWhen"] = "adding funds";
                return RedirectToAction("AccountManagementError", "Error", new { area = "Error" });
            }
            catch(InvalidOperationException ex)
            {
                TempData["ExceptionMessege"] = ex.Message;
                TempData["TheErrorHappendWhen"] = "adding funds";
                return RedirectToAction("AccountManagementError", "Error", new { area = "Error" });
            }
            catch(UserIsNullException ex)
            {
                TempData["Messege"] = ex.Message;
                return RedirectToAction("AccountManagementError", "Error", new { area = "Error" });
            }
            catch(UserIsDeletedException ex)
            {
                TempData["Messege"] = ex.Message;
                return RedirectToAction("AccountManagementError", "Error", new { area = "Error" });
            }
        }

        /// <summary>
        /// Gets the information about a User from AccountManagementSetvice in the form of UserDTO and passes it to the View. If the service trows an Exception Error Controller is called.
        /// </summary>
        /// <returns>DeleteView and passes it UserDTO. If there is an Exception thrown redirects to ErrorController.</returns>
        [Authorize]
        [HttpGet]
        [Area("AccountManagement")]
        public IActionResult Delete()
        {
            try
            {
                int userId = int.Parse(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
                UserDTO userDTO = this._managementService.GetUserById(userId);
                return View(userDTO);
            }
            catch(ArgumentException ex)
            {
                TempData["ExceptionMessege"] = ex.Message;
                TempData["TheErrorHappendWhen"] = "adding funds";
                return RedirectToAction("AccountManagementError", "Error", new { area = "Error" });
            }
            catch(InvalidOperationException ex)
            {
                TempData["ExceptionMessege"] = ex.Message;
                TempData["TheErrorHappendWhen"] = "adding funds";
                return RedirectToAction("AccountManagementError", "Error", new { area = "Error" });
            }
            catch(UserIsNullException ex)
            {
                TempData["Messege"] = ex.Message;
                return RedirectToAction("AccountManagementError", "Error", new { area = "Error" });
            }
            catch(UserIsDeletedException ex)
            {
                TempData["Messege"] = ex.Message;
                return RedirectToAction("AccountManagementError", "Error", new { area = "Error" });
            }
        }


        /// <summary>
        /// Calls the AccountManagementService to delete the current User and redirects to Index in the HomeController. If the Service trows an Exception ErrorController is called.
        /// </summary>
        /// <returns>Redirects Index in HomeController. If there is an Exception thrown redirects to ErrorController.</returns>
        [Authorize]
        [HttpPost, ActionName("Delete")]
        [Area("AccountManagement")]
        public IActionResult DeleteConfirm()
        {
            try
            {
                int userId = int.Parse(this.User.FindFirstValue(ClaimTypes.NameIdentifier));

                this._managementService.Delete(userId);

                return RedirectToAction("Index", "Home", new { area = "" });
            }
            catch(ArgumentException ex)
            {
                TempData["ExceptionMessege"] = ex.Message;
                TempData["TheErrorHappendWhen"] = "adding funds";
                return RedirectToAction("AccountManagementError", "Error", new { area = "Error" });
            }
            catch(InvalidOperationException ex)
            {
                TempData["ExceptionMessege"] = ex.Message;
                TempData["TheErrorHappendWhen"] = "adding funds";
                return RedirectToAction("AccountManagementError", "Error", new { area = "Error" });
            }
            catch(UserIsNullException ex)
            {
                TempData["Messege"] = ex.Message;
                return RedirectToAction("AccountManagementError", "Error", new { area = "Error" });
            }
            catch(UserIsDeletedException ex)
            {
                TempData["Messege"] = ex.Message;
                return RedirectToAction("AccountManagementError", "Error", new { area = "Error" });
            }
        }

        /// <summary>
        /// Gets List of TransactionHistoryDTOs and the profit from all transactions from AccountManagementService and passes them to ViewTransactionHistoryView. f the Service trows an Exception ErrorController is called.
        /// </summary>
        /// <returns>Returns ViewTransactionHistoryView and passes it the list as well as the total profit. If there is an Exception thrown redirects to ErrorController.</returns>
        [Authorize]
        [HttpGet]
        [Area("AccountManagement")]
        public IActionResult ViewTransactionHistory()
        {
            try
            {
                int userId = int.Parse(this.User.FindFirstValue(ClaimTypes.NameIdentifier));

                List<TransactionHistoryDTO> transactionHistoryDTOs = this._managementService.GetTransactionHistories(userId);

                decimal profit = this._managementService.GetProfit(userId);
                string stringProfit = profit.ToString("0.00");

                TempData["Profit"] = stringProfit;                
                return View(transactionHistoryDTOs);
            }
            catch(ArgumentException ex)
            {
                TempData["ExceptionMessege"] = ex.Message;
                TempData["TheErrorHappendWhen"] = "adding funds";
                return RedirectToAction("AccountManagementError", "Error", new { area = "Error" });
            }
            catch(InvalidOperationException ex)
            {
                TempData["ExceptionMessege"] = ex.Message;
                TempData["TheErrorHappendWhen"] = "adding funds";
                return RedirectToAction("AccountManagementError", "Error", new { area = "Error" });
            }
            catch(UserIsDeletedException ex)
            {
                TempData["Messege"] = ex.Message;
                return RedirectToAction("AccountManagementError", "Error", new { area = "Error" });
            }
        }
    }
}