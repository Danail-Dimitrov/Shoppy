using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shoppy.Areas.Buy.Models.DTO;
using Shoppy.Areas.Buy.Services;
using Shoppy.Areas.Buy.Services.Contracts;
using Shoppy.Models.DTO;

namespace Shoppy.Areas.Buy.Controllers
{
    public class BuyController : Controller
    {
        private readonly IBuyService _buyService;

        public BuyController(BuyService buyService)
        {
            this._buyService = buyService;
        }

        // GET: Buy
        [Authorize]
        [HttpGet]
        [Area("Buy")]
        public IActionResult Index()
        {
            try
            {
                int userId = int.Parse(this.User.FindFirstValue(ClaimTypes.NameIdentifier));

                List<BuyOfferWithTitelDTO> BuyOfferWithTitelDTOs = this._buyService.GetBuyOffersFromUser(userId);

                return View(BuyOfferWithTitelDTOs);
            }
            catch(ArgumentException ex)
            {
                TempData["ExceptionMessege"] = ex.Message;
                TempData["ErrorMessege"] = "Could not get the data from the database";
                return RedirectToAction("GettingDataFromDbError", "Error", new { area = "Error" });
            }
        }

        [HttpGet]
        [Area("Buy")]
        public IActionResult ShowRandomOffers(int numbderOfOffers = 4)
        {
            try
            {
                int userId = int.Parse(this.User.FindFirstValue(ClaimTypes.NameIdentifier));

                List<SellOfferDTO> sellOfferDTOs = this._buyService.GetRandomSellOffers(numbderOfOffers, userId);

                return View(sellOfferDTOs);
            }
            catch(ArgumentException ex)
            {
                TempData["ExceptionMessege"] = ex.Message;
                TempData["ErrorMessege"] = "Could not get the data from the database";
                return RedirectToAction("GettingDataFromDbError", "Error", new { area = "Error" });
            }
        }

        [HttpGet]
        [Area("Buy")]
        public IActionResult DetailsOfSellOffer(int? id)
        {
            try
            {
                SellOfferDTO sellOfferDTO = this._buyService.GetSellOfferById(id);

                return View(sellOfferDTO);
            }
            catch(ArgumentException ex)
            {
                TempData["ExceptionMessege"] = ex.Message;
                TempData["ErrorMessege"] = "Could not get the data from the database";
                return RedirectToAction("GettingDataFromDbError", "Error", new { area = "Error" });
            }
        }

        // GET: Buy/Create
        [Authorize]
        [Area("Buy")]
        public IActionResult Create(int? id)
        {
            try
            {
                CreateBuyOfferDTO createBuyOfferDTO = new CreateBuyOfferDTO();
                createBuyOfferDTO.AskedMoney = this._buyService.GetAskedMoneyForProduct(id);
                createBuyOfferDTO.Id = (int)(id);
                return View(createBuyOfferDTO);

            }
            catch(ArgumentException ex)
            {
                TempData["ExceptionMessege"] = ex.Message;
                TempData["ErrorMessege"] = "Could not get the data from the database";
                return RedirectToAction("GettingDataFromDbError", "Error", new { area = "Error" });
            }
        }

        // POST: Buy/Create
        [HttpPost]
        [Area("Buy")]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CreateBuyOfferDTO createBuyOfferDTO)
        {
            try
            {
                if(ModelState.IsValid)
                {
                    BuyOfferDTO buyOfferDTO = new BuyOfferDTO
                    {
                        OfferedMoney = createBuyOfferDTO.MoneyOffered,
                        SellOfferId = createBuyOfferDTO.Id
                    };

                    int userId = int.Parse(this.User.FindFirstValue(ClaimTypes.NameIdentifier));

                    this._buyService.ValidateBuyOfferDTO(buyOfferDTO);

                    this._buyService.CreateBuyOffer(buyOfferDTO, userId);

                    this._buyService.IncreaseUserScore(userId);

                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["TheErrorHappendWhen"] = "creating";
                    return RedirectToAction("BuyOfferError", "Error", new { area = "Error" });
                }
            }
            catch(ArgumentException ex)
            {
                TempData["ExceptionMessege"] = ex.Message;
                TempData["TheErrorHappendWhen"] = "creating";
                return RedirectToAction("BuyOfferError", "Error", new { area = "Error" });
            }
        }

        // GET: Buy/Edit/5
        [Area("Buy")]
        [HttpGet]
        public IActionResult Edit(int? id)
        {
            try
            {
                BuyOfferDTO buyOfferDTO = this._buyService.GetBuyOfferById(id);

                EditBuyOfferDTO editBuyOfferDTO = new EditBuyOfferDTO
                {
                    Id = buyOfferDTO.Id,
                    MoneyOffered = buyOfferDTO.OfferedMoney
                };

                return View(editBuyOfferDTO);
            }
            catch(ArgumentException ex)
            {
                TempData["ExceptionMessege"] = ex.Message;
                TempData["ErrorMessege"] = "Could not get the data from the database";
                return RedirectToAction("GettingDataFromDbError", "Error", new { area = "Error" });
            }
        }

        // POST: Buy/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Area("Buy")]
        public IActionResult Edit(EditBuyOfferDTO editBuyOfferDTO)
        {
            try
            {
                if(ModelState.IsValid)
                {
                    int userId = int.Parse(this.User.FindFirstValue(ClaimTypes.NameIdentifier));

                    this._buyService.EditBuyOffer(editBuyOfferDTO, userId);

                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["TheErrorHappendWhen"] = "editing";
                    return RedirectToAction("BuyOfferError", "Error", new { area = "Error" });
                }
            }
            catch(ArgumentException ex)
            {
                TempData["ExceptionMessege"] = ex.Message;
                TempData["TheErrorHappendWhen"] = "editing";
                return RedirectToAction("BuyOfferError", "Error", new { area = "Error" });
            }
            catch(InvalidOperationException ex)
            {
                TempData["ExceptionMessege"] = ex.Message;
                TempData["TheErrorHappendWhen"] = "editing";
                return RedirectToAction("BuyOfferError", "Error", new { area = "Error" });
            }
        }

        // GET: Buy/Delete/5
        [Area("Buy")]
        [HttpGet]
        public IActionResult Delete(int? id)
        {
            try
            {
                BuyOfferWithTitelDTO buyOfferWithTitel = this._buyService.GetBuyOfferWithTitelByIndex(id);

                return View(buyOfferWithTitel);
            }
            catch(ArgumentException ex)
            {
                TempData["ExceptionMessege"] = ex.Message;
                TempData["ErrorMessege"] = "Could not get the data from the database";
                return RedirectToAction("GettingDataFromDbError", "Error", new { area = "Error" });
            }
        }

        // POST: Buy/Delete/5
        [HttpPost]
        [Area("Buy"), ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirm(int? id)
        {
            try
            {
                int userId = int.Parse(this.User.FindFirstValue(ClaimTypes.NameIdentifier));

                this._buyService.DeleteBuyOffer(id, userId);

                return RedirectToAction(nameof(Index));
            }
            catch(ArgumentException ex)
            {
                TempData["ExceptionMessege"] = ex.Message;
                TempData["TheErrorHappendWhen"] = "deleting";
                return RedirectToAction("SellOfferError", "Error", new { area = "Error" });
            }
            catch(InvalidOperationException ex)
            {
                TempData["ExceptionMessege"] = ex.Message;
                TempData["TheErrorHappendWhen"] = "editing";
                return RedirectToAction("SellOfferError", "Error", new { area = "Error" });
            }
        }
    }
}