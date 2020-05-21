using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Shoppy.Models.DBEntities.Contracts
{
    public interface IBuyOffer
    {
        int Id { get; set; }
        decimal OfferedMoney { get; set; }
        int SellOfferId { get; set; }
        int UserId { get; set; }

        SellOffer SellOffer { get; set; }
    }
}
