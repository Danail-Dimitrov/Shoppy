using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Shoppy.Areas.AccountManagement.Models.DTO
{
    /// <summary>
    ///  DTO used to transfer information about the user
    /// </summary>
    public class UserDTO
    {
        public UserDTO(string userName, string firstName, string lastName, decimal money, int superUserScore)
        {
            this.UserName = userName;
            this.FirstName = firstName;
            this.LastName = lastName;
            this.Money = money;
            this.SuperUserScore = superUserScore;
        }

        [Display(Name = "User Name")]
        public string UserName { get; set; }
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        [Required]
        [Display(Name = "Money")]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Money { get; set; }
        [Display(Name = "User Score")]
        public int SuperUserScore { get; set; }
    }
}
