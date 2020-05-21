using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shoppy.Areas.Account.Models.DTO;
using Shoppy.Areas.Account.Services;

namespace Shoppy.Areas.Account.Controllers
{
    public class AccountController : Controller
    {
        private readonly AccountService _accountService;

        public AccountController(AccountService accountService)
        {
            this._accountService = accountService;
        }

        [Authorize]
        [HttpGet]
        [Area("Account")]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        [Area("Account")]
        public IActionResult AddFunds()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        [Area("Account")]
        public IActionResult AddFunds(AddFundsDTO addFundsDTO)
        {
            try
            {
                int userId = int.Parse(this.User.FindFirstValue(ClaimTypes.NameIdentifier));

                this._accountService.AddFundsToUser(userId, addFundsDTO);

                return RedirectToAction(nameof(Index));
            }
            catch(ArgumentException ex)
            {
                TempData["ExceptionMessege"] = ex.Message;
                TempData["ErrorMessege"] = "Could not get the data from the database";
                return RedirectToAction("GettingDataFromDbError", "Error", new { area = "Error" });
            }           
        }
    }
}