using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Shoppy.Models.DBEntities.Contracts
{
    public interface IProductTag
    {
        [Key]
        int Id { get; set; }

        [Required]
        string Tag { get; set; }

        //IList<SellOffer> SellOffers { get; set; }
    }
}
