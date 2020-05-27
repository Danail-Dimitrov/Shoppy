using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shoppy.Exceptions
{
    public class BuyerHasInsufficientFundsException : Exception
    {
        public BuyerHasInsufficientFundsException(string messege) : base(messege)
        {

        }
    }
}
