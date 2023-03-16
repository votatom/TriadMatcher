using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Triad_Matcher
{
    public class WrongWPFElementException : Exception
    {
        public WrongWPFElementException() : base() { }
        public WrongWPFElementException(string message) : base(message) { }
    }
}
