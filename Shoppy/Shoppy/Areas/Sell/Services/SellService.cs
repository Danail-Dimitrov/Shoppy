using Shoppy.Data;
using Shoppy.Models.DBEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shoppy.Areas.Sell.Services
{
    public class SellService
    {
        private readonly ApplicationDbContext dbContext;

        public SellService()
        {

        }

        public SellService(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
     
        public List<SellOffer> GetOffersFromUser(int userId)
        {            
           List<SellOffer> sellOffers = dbContext.SellOffers
                       .Where(s => s.UserId == userId)
                       .ToList();            

            return sellOffers;
        }
    }
}
