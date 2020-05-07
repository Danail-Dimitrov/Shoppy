using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Shoppy.Models.DBEntities
{
    public class User : IdentityUser<int>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [Required]
        [Column(TypeName = "decimal(7, 5)")]
        public decimal Money { get; set; }
        public bool IsDeleted { get; set; }
        public ICollection<Order> Orders { get; set; }
    }
}
