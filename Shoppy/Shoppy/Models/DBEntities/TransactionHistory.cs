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

        public TransactionHistory(DateTime date, string text, bool isProvit, decimal moneyAmaount)
        {
            this.Date = date;
            this.Text = text;
            this.IsProvit = isProvit;
            this.MoneyAmaount = moneyAmaount;
        }

        [Key]
        public int Id { get; set; }
        [Required]
        public DateTime Date { get; set; }
        [Required]
        public string Text { get; set; }
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
