using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Shoppy.Areas.Sell.Models.DTO;
using Shoppy.Areas.Sell.Services;
using Shoppy.Data;
using Shoppy.Models.DBEntities;

namespace Shoppy.Areas.Sell.Controllers
{
    public class SellController : Controller
    {
        private readonly SellService sellService;
        private readonly UserManager<User> userManager;

        public SellController(SellService sellService, UserManager<User> userManager)
        {
            this.sellService = sellService;
            this.userManager = userManager;
        }

        // GET: Sell
        [Authorize]
        [HttpGet]
        [Area("Sell")]
        public IActionResult Index()
        {
            int userId = int.Parse(this.User.FindFirstValue(ClaimTypes.NameIdentifier));

            List<SellOffer> sellOffers = sellService.GetOffersFromUser(userId);
            SellIndexDTO selOffersDTO = new SellIndexDTO(sellOffers);

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
        public IActionResult Create(IFormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Sell/Edit/5
        [Authorize]
        [Area("Sell")]
        public IActionResult Edit(int id)
        {
            return View();
        }

        // POST: Sell/Edit/5
        [Authorize]
        [Area("Sell")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Sell/Delete/5
        [Authorize]
        [Area("Sell")]
        public IActionResult Delete(int id)
        {
            return View();
        }

        // POST: Sell/Delete/5
        [Authorize]
        [HttpPost]
        [Area("Sell")]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}