using Shoppy.Models.DBEntities.Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Shoppy.Models.DBEntities
{
    public class TransactionHistory : ITransactionHistory
    {
        public TransactionHistory()
        {

        }

        public TransactionHistory(DateTime date, string productTitle, bool isProvit, decimal moneyAmaount, int userId)
        {
            this.Date = date;
            this.ProductTitle = productTitle;
            this.IsProvit = isProvit;
            this.MoneyAmaount = moneyAmaount;
            this.UserId = userId;
        }

        [Key]
        public int Id { get; set; }
        [Required]
        public DateTime Date { get; set; }
        [Required]
        public string ProductTitle { get; set; }
        [Required]
        public bool IsProvit { get; set; }
        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal MoneyAmaount { get; set; }
        [Required]
        [ForeignKey("UserId")]
        public int UserId { get; set; }
    }
}
