using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Exceptions
{
    public class AdaException : Exception
    {
        public AdaException(string message) : base(message)
        {

        }
    }
}
