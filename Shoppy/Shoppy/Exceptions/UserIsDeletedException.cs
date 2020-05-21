using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shoppy.Exceptions
{
    public class UserIsDeletedException : Exception
    {
        public UserIsDeletedException(string messege) : base(messege)
        {

        }
    }
}
