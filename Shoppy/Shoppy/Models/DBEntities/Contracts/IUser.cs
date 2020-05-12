using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Shoppy.Models.DBEntities.Contracts
{
    public interface IUser
    {
        [Required]
        string FirstName { get; set; }
        string LastName { get; set; }
        bool IsDeleted { get; set; }
        [Required]
        [Column(TypeName = "decimal(10,2)")]
        decimal Money { get; set; }
        int SuperUserScore { get; set; }

        ICollection<SellOffer> SellOffers { get; set; }
        ICollection<TransactionHistory> TransactionHistories { get; set; }
        ICollection<BuyOffer> BuyOffers { get; set; }
    }
}
