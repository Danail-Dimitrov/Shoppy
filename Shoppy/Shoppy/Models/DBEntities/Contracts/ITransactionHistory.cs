using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Shoppy.Models.DBEntities.Contracts
{
    public interface ITransactionHistory
    {
        [Key]
        int Id { get; set; }
        [Required]
        DateTime Date { get; set; }
        [Required]
        string Text { get; set; }
        [Required]
        bool IsProvit { get; set; }
        [Required]
        [Column(TypeName = "decimal(10,2)")]
        decimal MoneyAmaount { get; set; }
        [Required]
        [ForeignKey("UserId")]
        int UserId { get; set; }
    }
}
