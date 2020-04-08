using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

namespace Shoppy.Areas.Seller.Controllers
{
    public class SellerProductController : Controller
    {
        [Area("Seller")]
        public IActionResult Index()
        {
            return View();
         
        }

    }
}