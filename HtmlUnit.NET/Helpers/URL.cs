/*
 * Copyright (c) 2015 3D
 *
 * C# fork v0.1
 * 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HtmlUnit.Helpers
{
    public class URL
    {
        private string _host;
        private string _protocol;

        public string Host
        {
            get
            {
                return _host;
            }
        }

        public string Protocol
        {
            get
            {
                return _protocol;
            }
        }

        public string ToExternalForm()
        {
            // TODO : IMPLEMENT !
            throw new NotImplementedException();
        }

        public URL(String protocol, String host)
        {
            _protocol = protocol;
            _host = host;
        }
    }
}
