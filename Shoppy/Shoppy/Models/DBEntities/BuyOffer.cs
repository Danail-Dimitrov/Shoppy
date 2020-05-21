using Shoppy.Models.DBEntities.Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Shoppy.Models.DBEntities
{
    public class BuyOffer : IBuyOffer
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal OfferedMoney { get; set; }
        [ForeignKey("SellOffer")]
        public int SellOfferId { get; set; }
        [ForeignKey("User")]
        public int UserId { get; set; }
        public SellOffer SellOffer { get; set; }
    }
}
