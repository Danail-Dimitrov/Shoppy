using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Renci.SshNet.Messages;
using Shoppy.Areas.Error.Controllers;
using Shoppy.Areas.Sell.Models.DTO;
using Shoppy.Areas.Sell.Services;
using Shoppy.Models.DBEntities;

namespace Shoppy.Areas.Sell.Controllers
{
    public class SellController : Controller
    {
        private readonly SellService _sellService;
        private readonly ErrorController _errorController;

        public SellController(SellService sellService, ErrorController errorController)
        {
            this._sellService = sellService;
            this._errorController = errorController;
        }

        // GET: Sell
        [Authorize]
        [HttpGet]
        [Area("Sell")]
        public IActionResult Index()
        {
            int userId = int.Parse(this.User.FindFirstValue(ClaimTypes.NameIdentifier));

            List<SellOfferDTO> sellOfferDTOs = _sellService.GetOffersFromUser(userId);
            SellIndexDTO selOffersDTO = new SellIndexDTO(sellOfferDTOs);

            return View(selOffersDTO);
        }

        // GET: Sell/Create
        [Authorize]
        [Area("Sell")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Sell/Create
        [Authorize]
        [Area("Sell")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(SellOfferDTO sellOfferDTO)
        {
            try
            {
                if(ModelState.IsValid)
                {
                    int userId = int.Parse(this.User.FindFirstValue(ClaimTypes.NameIdentifier));

                    this._sellService.ValidateSellOfferDTO(sellOfferDTO);

                    this._sellService.CreateSellOffer(sellOfferDTO, userId);
                }

                return RedirectToAction("Index");
            }
            catch (ArgumentException ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("CreatingSellOfferError", "Error", new { area = "Error" });
            }
        }

        // GET: Sell/Edit/5
        [Authorize]
        [Area("Sell")]
        [HttpGet]
        public IActionResult Edit(int id)
        {
            SellOfferDTO sellOfferDTO = this._sellService.GetSellOfferById(id);

            return View(sellOfferDTO);
        }

        // POST: Sell/Edit/5
        [Authorize]
        [Area("Sell")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(SellOfferDTO sellOfferDTO)
        {
            try
            {
                if(ModelState.IsValid)
                {
                    this._sellService.ValidateSellOfferDTO(sellOfferDTO);

                    this._sellService.EditSellOffer(sellOfferDTO);
                }
                
                return RedirectToAction("Index");
            }
            catch (ArgumentException ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("EditingSellOfferError", "Error", new { area = "Error" });                
            }
        }

        // GET: Sell/Delete/5
        [Authorize]
        [Area("Sell")]
        public IActionResult Delete(int id)
        {
            SellOfferDTO sellOfferDTO = this._sellService.GetSellOfferById(id);

            return View(sellOfferDTO);
        }

        // POST: Sell/Delete/5
        [Authorize]
        [HttpPost]
        [Area("Sell")]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(SellOfferDTO sellOfferDTO)
        {
            try
            {
                this._sellService.ValidateSellOfferDTO(sellOfferDTO);

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}