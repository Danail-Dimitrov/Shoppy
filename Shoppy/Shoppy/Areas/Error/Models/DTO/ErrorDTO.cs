using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shoppy.Areas.Error.Models.DTO
{
    public class ErrorDTO
    {
        public ErrorDTO()
        {

        }

        public ErrorDTO(string errorMessege)
        {
            this.ErrorMessege = errorMessege;
        }

        public string ErrorMessege { get; set; }
    }
}
