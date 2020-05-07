using Shoppy.Areas.Seller.Services.Contracts;
using Shoppy.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shoppy.Areas.Seller.Services
{
    public class SellerProductService : ISellerProductService
    {
        private ApplicationDbContext dbContext;

        public SellerProductService(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }


    }
}
