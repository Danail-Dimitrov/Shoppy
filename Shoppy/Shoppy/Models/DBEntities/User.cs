using Microsoft.AspNetCore.Identity;
using Shoppy.Models.DBEntities.Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Shoppy.Models.DBEntities
{
    public class User : IdentityUser<int>, IUser
    {
        [Required]
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool IsDeleted { get; set; }
        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Money { get; set; }

        public ICollection<SellOffer> SellOffers { get; set; }
        public ICollection<TransactionHistory> TransactionHistories { get; set; }
        public ICollection<BuyOffer> BuyOffers { get; set; }
        public int SuperUserScore { get; set; }
    }
}
