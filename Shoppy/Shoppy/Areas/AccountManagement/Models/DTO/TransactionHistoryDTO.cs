using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Shoppy.Areas.AccountManagement.Models.DTO
{
    /// <summary>
    ///  DTO used to transfer information about a TransactionHistory
    /// </summary>
    public class TransactionHistoryDTO
    {
        [Display(Name = "Transaction Date")]
        public DateTime Date { get; set; }
        [Display(Name = "Product Title")]
        public string ProductTitle { get; set; }
        public bool IsProvit { get; set; }
        [Column(TypeName = "decimal(10,2)")]
        public decimal MoneyAmaount { get; set; }
    }
}
