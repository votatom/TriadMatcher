using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Triad_Matcher
{
    public class InvalidWPFElementException : Exception
    {
        public InvalidWPFElementException() : base() { }
        public InvalidWPFElementException(string message) : base(message) { }
    }
}
