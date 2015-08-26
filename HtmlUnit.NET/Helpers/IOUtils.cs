using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace HtmlUnit.Helpers
{
    public static class IOUtils
    {
        public static String ToString(InputStream stream, String encoding)
        { 
            StreamReader sr = new StreamReader(stream);
            return sr.ReadToEnd();
        }

        public static void CloseQuietly(InputStream stream)
        {
            stream.Close();
        }
    }
}
