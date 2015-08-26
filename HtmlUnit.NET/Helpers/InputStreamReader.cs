using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace HtmlUnit.Helpers
{
    public class InputStreamReader : StreamReader
    {
        public InputStreamReader(InputStream stream, Encoding encoding) :
            base(stream, encoding)
        { 
        }

        public InputStreamReader(InputStream stream):
            base(stream)
        { 
        }
    }
}
