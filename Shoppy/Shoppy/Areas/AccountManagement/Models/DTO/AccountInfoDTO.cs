using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Shoppy.Areas.AccountManagement.Models.DTO
{
    /// <summary>
    /// DTO used to transfer information about the user's SuperUserScroe and Money
    /// </summary>
    public class AccountInfoDTO
    {
        [Display(Name = "User Score")]
        public int SuperUserScore { get; set; }

        [Display(Name = "Money")]
        [Range(0.01, 9999999999, ErrorMessage = "The amout of added funds must be betwen 0.01 and 9999999999")]
        public decimal Money { get; set; }
    }
}
