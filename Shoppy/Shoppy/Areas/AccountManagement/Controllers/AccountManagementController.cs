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

        [Authorize]
        [HttpGet]
        [Area("AccountManagement")]
        public IActionResult AddFunds()
        {
            return View();
        }

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

        [Authorize]
        [HttpPost, ActionName("Delete")]
        [Area("AccountManagement")]
        public IActionResult DeleteConfirm()
        {
            int userId = int.Parse(this.User.FindFirstValue(ClaimTypes.NameIdentifier));

            this._managementService.Delete(userId);

            return RedirectToAction("Index", "Home", new { area = "" });
        }
    }
}