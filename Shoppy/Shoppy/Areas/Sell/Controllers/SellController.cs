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
using Shoppy.Models.DTO;
using Shoppy.Areas.Sell.Services;
using Shoppy.Areas.Sell.Services.Contracts;
using Shoppy.Models.DBEntities;
using Shoppy.Areas.Sell.Models.DTO;
using Shoppy.Exceptions;

namespace Shoppy.Areas.Sell.Controllers
{
    /// <summary>
    /// This is the Controller for all actions with Sell Offers
    /// </summary>
    public class SellController : Controller
    {
        private readonly ISellService _sellService;

        public SellController(SellService sellService)
        {
            this._sellService = sellService;
        }

        /// <summary>
        /// Calls the SellService to get all SellOffers that the logged in user has and passes them to the View, using SellIndexDTO.
        /// </summary>
        /// <returns>Index View with SellIndexDTO</returns>
        // GET: Sell
        [Authorize]
        [HttpGet]
        [Area("Sell")]
        public IActionResult Index()
        {
            try
            {
                int userId = int.Parse(this.User.FindFirstValue(ClaimTypes.NameIdentifier));

                List<SellOfferDTO> sellOfferDTOs = _sellService.GetSellOffersFromUser(userId);

                return View(sellOfferDTOs);
            }
            catch(ArgumentException ex)
            {
                TempData["ExceptionMessege"] = ex.Message;
                TempData["ErrorMessege"] = "Could not get the data from the database";
                return RedirectToAction("GettingDataFromDbError", "Error", new { area = "Error" });
            }
            catch(UserIsDeletedException ex)
            {
                TempData["Messege"] = ex.Message;
                return RedirectToAction("GettingDataFromDbError", "Error", new { area = "Error" });
            }
        }

        [Authorize]
        [HttpGet]
        [Area("Sell")]
        public ActionResult Details(int? id)
        {
            try
            {
                SellOfferDTO sellOfferDTO = this._sellService.GetSellOfferById(id);

                return View(sellOfferDTO);
            }
            catch(ArgumentException ex)
            {
                TempData["ExceptionMessege"] = ex.Message;
                TempData["ErrorMessege"] = "Could not get the data from the database";
                return RedirectToAction("GettingDataFromDbError", "Error", new { area = "Error" });
            }
            catch(UserIsDeletedException ex)
            {
                TempData["Messege"] = ex.Message;
                return RedirectToAction("GettingDataFromDbError", "Error", new { area = "Error" });
            }
        }

        /// <summary>
        /// Returns the View for the creating of a new SellOffer
        /// </summary>
        /// <returns>Create View</returns>
        // GET: Sell/Create
        [Authorize]
        [Area("Sell")]
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Gets the data from the Create View, calls the SellService class to validate it, than to add it to the data base, if the data is valid and increases the SuperUser score for the user that created the SellOffer. If the data is invalid ( either the builed in ModelState.IsValid or the validation in SellService has not passed) calls Error Controller so the user can be shown an error page.
        /// </summary>
        /// <param name="sellOfferDTO">The DTO containing the information, inputed by the user in the Create view, for the new SellOffer</param>
        /// <returns>Redirects to Index</returns>
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

                    this._sellService.IncreaseUserScore(userId);

                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["TheErrorHappendWhen"] = "creating";
                    return RedirectToAction("SellOfferError", "Error", new { area = "Error" });
                }             
            }
            catch (ArgumentException ex)
            {
                TempData["ExceptionMessege"] = ex.Message;
                TempData["TheErrorHappendWhen"] = "creating";
                return RedirectToAction("SellOfferError", "Error", new { area = "Error" });
            }
            catch(UserIsDeletedException ex)
            {
                TempData["Messege"] = ex.Message;
                return RedirectToAction("GettingDataFromDbError", "Error", new { area = "Error" });
            }
        }

        /// <summary>
        /// Shows Edit View and passes it SellOfferDTO
        /// </summary>
        /// <param name="id">The id of the Sell Offer that needs to be edited</param>
        /// <returns>Edit View</returns>
        // GET: Sell/Edit/5
        [Authorize]
        [Area("Sell")]
        [HttpGet]
        public IActionResult Edit(int? id)
        {
            try
            {
                SellOfferDTO sellOfferDTO = this._sellService.GetSellOfferById(id);

                return View(sellOfferDTO);
            }
            catch(ArgumentException ex)
            {
                TempData["ExceptionMessege"] = ex.Message;
                TempData["ErrorMessege"] = "Could not get the data from the database";
                return RedirectToAction("GettingDataFromDbError", "Error", new { area = "Error" });
            }
            catch(UserIsDeletedException ex)
            {
                TempData["Messege"] = ex.Message;
                return RedirectToAction("GettingDataFromDbError", "Error", new { area = "Error" });
            }
        }

        /// <summary>
        /// Gets the data from the Edit View, calls the SellService class to validate it, than to update it in the data base. If the data is invalid ( either the builed in ModelState.IsValid or the validation in SellService has not passed) calls Error Controller so the user can be shown an error page.
        /// </summary>
        /// <param name="sellOfferDTO">The DTO containing the data entered from the user about how the SellOffer needs to be changed</param>
        /// <returns>Redirects to Index</returns>
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
                    int userId = int.Parse(this.User.FindFirstValue(ClaimTypes.NameIdentifier));

                    this._sellService.ValidateSellOfferDTO(sellOfferDTO);

                    this._sellService.EditSellOffer(sellOfferDTO, userId);

                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["TheErrorHappendWhen"] = "editing";
                    return RedirectToAction("SellOfferError", "Error", new { area = "Error" });
                }
            }
            catch (ArgumentException ex)
            {
                TempData["ExceptionMessege"] = ex.Message;
                TempData["TheErrorHappendWhen"] = "editing";
                return RedirectToAction("SellOfferError", "Error", new { area = "Error" });                
            }
            catch(InvalidOperationException ex)
            {
                TempData["ExceptionMessege"] = ex.Message;
                TempData["TheErrorHappendWhen"] = "editing";
                return RedirectToAction("SellOfferError", "Error", new { area = "Error" });
            }
            catch(UserIsDeletedException ex)
            {
                TempData["Messege"] = ex.Message;
                return RedirectToAction("GettingDataFromDbError", "Error", new { area = "Error" });
            }
        }

        /// <summary>
        /// Shows a Confirm Delete page
        /// </summary>
        /// <param name="id">The id of the SellOffer that needs to be deleted</param>
        /// <returns>Delete View and passes it DTO with the data of the SellOffer</returns>
        // GET: Sell/Delete/5
        [Authorize]
        [Area("Sell")]
        public IActionResult Delete(int? id)
        {
            try
            {
                SellOfferDTO sellOfferDTO = this._sellService.GetSellOfferById(id);

                return View(sellOfferDTO);
            }
            catch(ArgumentException ex)
            {
                TempData["ExceptionMessege"] = ex.Message;
                TempData["ErrorMessege"] = "Could not get the data from the database";
                return RedirectToAction("GettingDataFromDbError", "Error", new { area = "Error" });
            }
            catch(UserIsDeletedException ex)
            {
                TempData["Messege"] = ex.Message;
                return RedirectToAction("GettingDataFromDbError", "Error", new { area = "Error" });
            }
        }

        /// <summary>
        /// Telss the service provider to delete the SellOffer chosen by the User.  If the id is invalid (the validation in SellService has not passed) calls Error Controller so the user can be shown an error page.
        /// </summary>
        /// <param name="id">the id of the SellOffer that needs to be deletet</param>
        /// <returns>Redirects to the Index Page</returns>
        // POST: Sell/Delete/5
        [Authorize]
        [HttpPost, ActionName("Delete")]
        [Area("Sell")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int? id)
        {
            try
            {
                int userId = int.Parse(this.User.FindFirstValue(ClaimTypes.NameIdentifier));

                this._sellService.Delete(id, userId);
               
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
            catch(UserIsDeletedException ex)
            {
                TempData["Messege"] = ex.Message;
                return RedirectToAction("GettingDataFromDbError", "Error", new { area = "Error" });
            }
        }

        /// <summary>
        /// Calls the SellService class to get all the BuyOffers related to the selected SellOffer
        /// </summary>
        /// <param name="id">The id of the SellOffer the Buy Offers of witch need to be shown</param>
        /// <returns>The View showing all the buy offers</returns>
        [Authorize]
        [HttpGet]
        [Area("Sell")]
        public IActionResult BuyOffers (int? id)
        {
            try
            {
                List<ShowBuyOffersDTO> showBuyOffersDTOs = this._sellService.GetBuyOffers(id);

                return View(showBuyOffersDTOs);
            }
            catch(ArgumentException ex)
            {
                TempData["ExceptionMessege"] = ex.Message;
                TempData["ErrorMessege"] = "Could not get the data from the database";
                return RedirectToAction("GettingDataFromDbError", "Error", new { area = "Error" });
            }
            catch(UserIsDeletedException ex)
            {
                TempData["Messege"] = ex.Message;
                return RedirectToAction("GettingDataFromDbError", "Error", new { area = "Error" });
            }
        }
    }
}