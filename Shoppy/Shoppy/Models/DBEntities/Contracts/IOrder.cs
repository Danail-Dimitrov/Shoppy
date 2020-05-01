using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shoppy.Models.DBEntities.Contracts
{
    public interface IOrder
    {
        int Id { get; set; }
        DateTime OrderPlaced { get; set; }
        DateTime? OrderFulfielled { get; set; }
        int UserId { get; set; }

        User User { get; set; }
        ICollection<ProductOrder> ProdcutOrders { get; set; }
    }
}
