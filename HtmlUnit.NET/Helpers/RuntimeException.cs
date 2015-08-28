using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HtmlUnit.Helpers
{
    public class RuntimeException : Exception
    {
        public RuntimeException()
        { 
        }

        public RuntimeException(string message):
            base(message)
        { 
        }
    }
}
