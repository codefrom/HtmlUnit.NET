/*
 * Copyright (c) 2002-2015 Gargoyle Software Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
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
using HtmlUnit.com.gargoylesoftware.htmlunit.util;
using HtmlUnit.Helpers;

namespace HtmlUnit.com.gargoylesoftware.htmlunit
{
    /// <summary>
    /// A simple WebResponse created from a string. Content is assumed to be of type <tt>text/html</tt>.
    /// @version $Revision: 9868 $
    /// @author <a href="mailto:mbowler@GargoyleSoftware.com">Mike Bowler</a>
    /// @author Marc Guillemot
    /// @author Brad Clarke
    /// @author Ahmed Ashour
    /// @author Ronald Brill
    /// @author Carsten Steul
    /// </summary>
    public class StringWebResponse : WebResponse
    {
        private bool fromJavascript_;

        /// <summary>
        /// Creates an instance associated with the specified originating URL.
        /// </summary>
        /// <param name="content">the content to return</param>
        /// <param name="originatingURL">the URL that this should be associated with</param>
        public StringWebResponse(String content, URL originatingURL) :
            // use UTF-8 here to be sure, all chars in the string are part of the charset
            this(content, "UTF-8", originatingURL)
        {
        }

        /// <summary>
        /// Creates an instance associated with the specified originating URL.
        /// </summary>
        /// <param name="content">the content to return</param>
        /// <param name="charset">the charset used to convert the content</param>
        /// <param name="originatingURL">the URL that this should be associated with</param>
        public StringWebResponse(String content, String charset, URL originatingURL) :
            base(GetWebResponseData(content, charset), BuildWebRequest(originatingURL, charset), 0)
        {
        }

        /// <summary>
        /// Helper method for constructors. Converts the specified string into {@link WebResponseData}
        /// with other defaults specified.
        /// </summary>
        /// <param name="contentString">the string to be converted to a <tt>WebResponseData</tt></param>
        /// <param name="charset"></param>
        /// <returns>a simple <tt>WebResponseData</tt> with defaults specified</returns>
        private static WebResponseData GetWebResponseData(String contentString, String charset)
        {
            byte[] content = TextUtil.StringToByteArray(contentString, charset);
            List<NameValuePair> compiledHeaders = new List<NameValuePair>();
            compiledHeaders.Add(new NameValuePair("Content-Type", "text/html; charset=" + charset));
            return new WebResponseData(content, HttpStatus.SC_OK, "OK", compiledHeaders);
        }

        private static WebRequest BuildWebRequest(URL originatingURL, String charset)
        {
            WebRequest webRequest = new WebRequest(originatingURL, HttpMethod.GET);
            webRequest.Charset = charset;
            return webRequest;
        }

        /// <summary>
        /// Returns the fromJavascript property. This is true, if the response was created
        /// from javascript (usually document.write).
        /// </summary>
        public bool IsFromJavascript
        {
            get
            {
                return fromJavascript_;
            }
            set
            {
                fromJavascript_ = value;
            }
        }
    }
}
