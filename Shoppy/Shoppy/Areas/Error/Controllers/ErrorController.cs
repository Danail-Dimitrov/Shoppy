using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

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
        public IActionResult CRUDError()
        {
            return View();
        }
    }
}