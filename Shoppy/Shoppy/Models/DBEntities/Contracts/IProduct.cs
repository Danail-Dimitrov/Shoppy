using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shoppy.Models.DBEntities.Contracts
{
    public interface IProduct
    {
        int Id { get; set; }       
        string Name { get; set; }      
        decimal Price { get; set; }
        string Model { get; set; }
    }
}
