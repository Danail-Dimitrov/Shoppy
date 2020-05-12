using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Shoppy.Areas.Error.Models.DTO;

namespace Shoppy.Areas.Error.Controllers
{
    public class ErrorController : Controller
    {
        [Area("Error")]
        public IActionResult Index()
        {
            return View();
        }

        [Area("Error")]
        public IActionResult CreatingSellOfferError(ErrorDTO errorDTO)
        {
            return View(errorDTO);
        }
    }
}