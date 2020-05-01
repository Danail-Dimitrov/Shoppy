using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Shoppy.Models.DBEntities.Contracts
{
    public interface IProductOrder
    { 
        [Key]
        int Id { get; set; }
        int Quantity { get; set; }
        int ProductId { get; set; }
        int OrderId { get; set; }

        Order Order { get; set; }
        Product Product { get; set; }
    }
}
