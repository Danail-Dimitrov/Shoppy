using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shoppy.Exceptions
{
    public class UserIsNullException : Exception
    {
        public UserIsNullException(string messege) : base(messege)
        {

        }
    }
}
