using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shoppy.Models.DBEntities
{
    public class UserRole : IdentityRole<int>
    {
        public UserRole()
        {

        }
        public UserRole(string role) : base(role)
        {

        }
        public string Description { get; set; }

    }
}
