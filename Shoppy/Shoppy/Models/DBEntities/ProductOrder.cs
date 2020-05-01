using Shoppy.Models.DBEntities.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shoppy.Models.DBEntities
{
    public class ProductOrder : IProductOrder
    {
        public ProductOrder()
        {
                
        }

        public ProductOrder(int id, int quantity, int productId, int orderId, Order order, Product product)
        {
            this.Id = id;
            this.Quantity = quantity;
            this.ProductId = productId;
            this.OrderId = orderId;
            this.Order = order;
            this.Product = product;
        }

        public int Id { get; set; }
        public int Quantity { get; set; }
        public int ProductId { get; set; }
        public int OrderId { get; set; }

        public Order Order { get; set; }
        public Product Product { get; set; }
    }
}
