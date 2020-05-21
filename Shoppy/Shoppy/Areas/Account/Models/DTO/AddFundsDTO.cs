using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Shoppy.Areas.Account.Models.DTO
{
    public class AddFundsDTO
    {
        [Display(Name = "Add Funds")]
        [Range(0.01, 9999999999, ErrorMessage = "The amout of money offered must be betwen 0.01 and 9999999999")]
        public decimal Money { get; set; }
    }
}
