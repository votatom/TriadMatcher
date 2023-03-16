using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Triad_Matcher
{
    public class InsufficientDataException : Exception
    {
        public InsufficientDataException() : base() { }
        public InsufficientDataException(string message) : base(message) { }
    }
}
