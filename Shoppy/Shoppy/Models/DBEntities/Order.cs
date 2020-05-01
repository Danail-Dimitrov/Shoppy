using Shoppy.Models.DBEntities.Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Shoppy.Models.DBEntities
{
    public class Order : IOrder
    {
        public Order()
        {
        }

        public Order(int id, DateTime orderPlaced, DateTime? orderFulfielled, int userId, User user, ICollection<ProductOrder> prodcutOrders)
        {
            this.Id = id;
            this.OrderPlaced = orderPlaced;
            this.OrderFulfielled = orderFulfielled;
            this.UserId = userId;
            this.User = user;
            this.ProdcutOrders = prodcutOrders;
        }

        [Key]
        public int Id { get; set; }
        public DateTime OrderPlaced { get; set; }
        public DateTime? OrderFulfielled { get; set; }
        [Required]
        public int UserId { get; set; }

        public User User { get; set; }
        public ICollection<ProductOrder> ProdcutOrders { get; set; }
    }
}