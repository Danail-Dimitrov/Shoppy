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
using Shoppy.Exceptions;
using Shoppy.Models.DTO;

namespace Shoppy.Areas.Buy.Controllers
{
    public class BuyController : Controller
    {
        /// <summary>
        /// Shows the default value for how many random SellOffers need to be displayed in the ShowRandomOffers method.
        /// </summary>
        private const int DefaultRandomBuyOffersPerPage = 4;
        private readonly IBuyService _buyService;

        public BuyController(BuyService buyService)
        {
            this._buyService = buyService;
        }

        /// <summary>
        /// Calls BuyService to get all the BuyOffers a User has in a list as BuyOfferDTO and them passes them to the IndexView. If an Exception is thrown by the Service calls ErrorController. This is the Index for this Controller.
        /// </summary>
        /// <returns>Index View and passes it the list with BuyOfferDTO. If there is an Exception thrown redirects to ErrorController.</returns>
        // GET: Buy
        [Authorize]
        [HttpGet]
        [Area("Buy")]
        public IActionResult Index()
        {
            try
            {
                int userId = int.Parse(this.User.FindFirstValue(ClaimTypes.NameIdentifier));

                List<BuyOfferWithTitelDTO> buyOfferWithTitelDTOs = this._buyService.GetBuyOffersFromUser(userId);

                return View(buyOfferWithTitelDTOs);
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
            catch(UserIsNullException ex)
            {
                TempData["Messege"] = ex.Message;
                return RedirectToAction("GettingDataFromDbError", "Error", new { area = "Error" });
            }
        }

        /// <summary>
        /// Calls the BuyService to get a set number of random SellOffers, to be shown in the view. If an Exception is thrown by the Service ErrorContrller is called.
        /// </summary>
        /// <param name="numbderOfOffers">The number of random selloffers that are going to be shown on the View. The default value is shown in the constant DefaultRandomBuyOffersPerPage</param>
        /// <returns>ShowOffersView and passes it the SellOfferDTOs. If there is an Exception thrown redirects to ErrorController.</returns>
        [HttpGet]
        [Area("Buy")]
        public IActionResult ShowRandomOffers(int numbderOfOffers = DefaultRandomBuyOffersPerPage)
        {
            try
            {
                int userId = int.Parse(this.User.FindFirstValue(ClaimTypes.NameIdentifier));

                List<SellOfferDTO> sellOfferDTOs = this._buyService.GetRandomSellOffers(numbderOfOffers, userId);

                return View("ShowOffers", sellOfferDTOs);
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
            catch(UserIsNullException ex)
            {
                TempData["Messege"] = ex.Message;
                return RedirectToAction("GettingDataFromDbError", "Error", new { area = "Error" });
            }
        }

        /// <summary>
        /// Calls BuyService to get details abiut a SellOffer stored in SellOfferDTO. If the Service throws and Exception ErrorController is called
        /// </summary>
        /// <param name="id">Id of the SellOffer whoes details are needed.</param>
        /// <returns>DetailsOfSellOfferView and passes it the SellOfferDTO. If there is an Exception thrown redirects to ErrorController.</returns>
        [HttpGet]
        [Area("Buy")]
        [Authorize]
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
            catch(UserIsDeletedException ex)
            {
                TempData["Messege"] = ex.Message;
                return RedirectToAction("GettingDataFromDbError", "Error", new { area = "Error" });
            }
            catch(UserIsNullException ex)
            {
                TempData["Messege"] = ex.Message;
                return RedirectToAction("GettingDataFromDbError", "Error", new { area = "Error" });
            }
        }


        /// <summary>
        /// Creats CreateBuyOfferDTO which stores the id and the Price of the SellOffer, for which the BuyOffer, that is beeing created, is. The Price of the SellOffer is recived from BuyService as well as the boolean telling if the price is negotiable. If the Service throws an Exception ErrorController is called.
        /// </summary>
        /// <param name="id">Id of the SellOffer, for which the BuyOffer is beeing created.</param>
        /// <returns>CreateView and passes it the CreateBuyOfferDTO. If there is an Exception thrown redirects to ErrorController.</returns>
        // GET: Buy/Create
        [Authorize]
        [Area("Buy")]
        public IActionResult Create(int? id)
        {
            try
            {
                CreateBuyOfferDTO createBuyOfferDTO = new CreateBuyOfferDTO();
                createBuyOfferDTO.AskedMoney = this._buyService.GetAskedMoney(id);
                createBuyOfferDTO.IsThePriceNegotiable = this._buyService.GetIsThePriceNegotiable(id);
                createBuyOfferDTO.Id = (int)(id);
                return View(createBuyOfferDTO);

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
            catch(UserIsNullException ex)
            {
                TempData["Messege"] = ex.Message;
                return RedirectToAction("GettingDataFromDbError", "Error", new { area = "Error" });
            }
        }

        /// <summary>
        /// Gets the CreateBuyOfferDTO, Validates that the model state is valid, if not throws and Exception. Creates BuyOfferDTO and passes it to BuyService to create BuyOdder. Calls BuyService to increase the user's SuperUserScore. If the Service throws an Exception ErrorControllrt iscalled.
        /// </summary>
        /// <param name="createBuyOfferDTO">The DTO storing the information about the BuyOffer that is going to be created.</param>
        /// <returns>Redirects to Index. If there is an Exception thrown redirects to ErrorController.</returns>
        // POST: Buy/Create
        [HttpPost]
        [Area("Buy")]
        [ValidateAntiForgeryToken]
        [Authorize]
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
            catch(UserIsDeletedException ex)
            {
                TempData["Messege"] = ex.Message;
                return RedirectToAction("GettingDataFromDbError", "Error", new { area = "Error" });
            }
            catch(UserIsNullException ex)
            {
                TempData["Messege"] = ex.Message;
                return RedirectToAction("GettingDataFromDbError", "Error", new { area = "Error" });
            }
        }

        /// <summary>
        /// Gets the id of the BuyOffer that needs to be edited, gets its information from SellService, Converts it to EditBuyOfferDTO and passes it to the View. If the Service throws an Exception ErrorController is called.
        /// </summary>
        /// <param name="id">Id of the BuyOffer that needs to be Edited</param>
        /// <returns>EditView and passes it the EditBuyOfferDTO. If there is an Exception thrown redirects to ErrorController.</returns>
        // GET: Buy/Edit/5
        [Area("Buy")]
        [Authorize]
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
            catch(UserIsDeletedException ex)
            {
                TempData["Messege"] = ex.Message;
                return RedirectToAction("GettingDataFromDbError", "Error", new { area = "Error" });
            }
            catch(UserIsNullException ex)
            {
                TempData["Messege"] = ex.Message;
                return RedirectToAction("GettingDataFromDbError", "Error", new { area = "Error" });
            }
        }

        /// <summary>
        /// Gets the information about the BuyOffer that needs to be Edited, using EditBuyOfferDTO and calls BuyService to edit it. If the Service throws an Exception ErrorController is called. 
        /// </summary>
        /// <param name="editBuyOfferDTO">The DTO storing the information about the BuyOffer that needs to be edited</param>
        /// <returns>Redirects to Idndex. If there is an Exception thrown redirects to ErrorController.</returns>
        // POST: Buy/Edit/5
        [HttpPost]
        [Authorize]
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
            catch(UserIsDeletedException ex)
            {
                TempData["Messege"] = ex.Message;
                return RedirectToAction("GettingDataFromDbError", "Error", new { area = "Error" });
            }
            catch(UserIsNullException ex)
            {
                TempData["Messege"] = ex.Message;
                return RedirectToAction("GettingDataFromDbError", "Error", new { area = "Error" });
            }
        }

        /// <summary>
        /// Gets the Id of the BuyOffer that needs to be delited, calls BuyService to get the information about it in the form of BuyOfferDTO and passes it to the Delete View. If the Service throws an Exception ErrorController is called.
        /// </summary>
        /// <param name="id">The id of the BuyOffer that eeeds to be deleted</param>
        /// <returns>DeleteView and passes it the BuyOfferDTO. If there is an Exception thrown redirects to ErrorController.</returns>
        // GET: Buy/Delete/5
        [Area("Buy")]
        [Authorize]
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
            catch(UserIsDeletedException ex)
            {
                TempData["Messege"] = ex.Message;
                return RedirectToAction("GettingDataFromDbError", "Error", new { area = "Error" });
            }
            catch(UserIsNullException ex)
            {
                TempData["Messege"] = ex.Message;
                return RedirectToAction("GettingDataFromDbError", "Error", new { area = "Error" });
            }
        }

        /// <summary>
        /// Gets the id of the BuyOffer that needs to be deleted and passes it BuyService. If the Service throws and Exception ErrorController will be called.
        /// </summary>
        /// <param name="id">The id of the BuyOffer that needs to be deleted</param>
        /// <returns>Redirects to Index. If there is an Exception thrown redirects to ErrorController.</returns>
        // POST: Buy/Delete/5
        [HttpPost]
        [Authorize]
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
            catch(UserIsDeletedException ex)
            {
                TempData["Messege"] = ex.Message;
                return RedirectToAction("GettingDataFromDbError", "Error", new { area = "Error" });
            }
            catch(UserIsNullException ex)
            {
                TempData["Messege"] = ex.Message;
                return RedirectToAction("GettingDataFromDbError", "Error", new { area = "Error" });
            }
        }

        /// <summary>
        /// Shows the GetSellOffersByNameView. It shows a searchbar for the user to serach a SellOffer.
        /// </summary>
        /// <returns>GetSellOffersByNameView</returns>
        [HttpGet]
        [Authorize]
        [Area("Buy")]
        public IActionResult GetSellOffersByName()
        {
            return View();
        }

        /// <summary>
        /// Gets the information about the searched SellOffer in the form of GetSellOfferByNameDTO, passes it to the BuyService and gets from it List of SellOfferDTOs that contains all SellOffer who meet the criteria from the GetSellOfferByNameDTO. If the Service throws a Exception ErrorController is called.
        /// </summary>
        /// <param name="getSellOfferByNameDTO">The DTO that contains the criteria for the SellOffers</param>
        /// <returns>ShowOffersView and passes it the list of SellOffers that are going to be shown. If there is an Exception thrown redirects to ErrorController.</returns>
        [Area("Buy")]
        [Authorize]
        public IActionResult GetSellOffersByName(GetSellOfferByNameDTO getSellOfferByNameDTO)
        {
            try
            {
                int userId = int.Parse(this.User.FindFirstValue(ClaimTypes.NameIdentifier));

                List<SellOfferDTO> sellOfferDTOs = this._buyService.GetSellOffersByName(getSellOfferByNameDTO, userId);

                return View("ShowOffers", sellOfferDTOs);
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
            catch(UserIsNullException ex)
            {
                TempData["Messege"] = ex.Message;
                return RedirectToAction("GettingDataFromDbError", "Error", new { area = "Error" });
            }
        }
    }
}