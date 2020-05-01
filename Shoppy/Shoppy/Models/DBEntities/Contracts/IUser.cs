using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shoppy.Models.DBEntities.Contracts
{
    public interface IUser
    {
        decimal? Money { get; set; }
        ICollection<Order> Orders { get; set; }
    }
}
