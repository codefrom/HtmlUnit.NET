using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HtmlUnit.Helpers
{
    public class IllegalStateException: Exception
    {
        public IllegalStateException(): base()
        {}

        public IllegalStateException(string message) : base(message)
        {
        }
    }
}
