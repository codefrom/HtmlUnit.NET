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
    /// Parameter object for making web requests.
    /// @version $Revision: 10804 $
    /// @author Brad Clarke
    /// @author Hans Donner
    /// @author Ahmed Ashour
    /// @author Marc Guillemot
    /// @author Rodney Gitzel
    /// @author Ronald Brill
    /// </summary>
    [Serializable]
    public class WebRequest
    {
        private static readonly Pattern DOT_PATTERN = Pattern.compile("/\\./");
        private static readonly Pattern DOT_DOT_PATTERN = Pattern.compile("/(?!\\.\\.)[^/]*/\\.\\./");
        private static readonly Pattern REMOVE_DOTS_PATTERN = Pattern.compile("^/(\\.\\.?/)*");

        private String url_; // String instead of java.net.URL because "about:blank" URLs don't serialize correctly
        private String proxyHost_;
        private int proxyPort_;
        private bool isSocksProxy_;
        private HttpMethod httpMethod_ = HttpMethod.GET;
        private FormEncodingType encodingType_ = FormEncodingType.URL_ENCODED;
        private Dictionary<String, String> additionalHeaders_ = new Dictionary<string, string>();
        private Credentials urlCredentials_;
        private Credentials credentials_;
        private String charset_ = TextUtil.DEFAULT_CHARSET;

        /* These two are mutually exclusive; additionally, requestBody_ should only be set for POST requests. */
        private List<NameValuePair> requestParameters_ = new List<NameValuePair>();
        private String requestBody_;

        /// <summary>
        /// The proxy host to use.
        /// </summary>
        public String ProxyHost
        {
            get
            {
                return proxyHost_;
            }
            set
            {
                proxyHost_ = value;
            }
        }

        /// <summary>
        /// The proxy port to use.
        /// </summary>
        public int ProxyPort
        {
            get
            {
                return proxyPort_;
            }
            set
            {
                proxyPort_ = value;
            }
        }

        /// <summary>
        /// Whether SOCKS proxy or not
        /// </summary>
        public bool IsSocksProxy
        {
            get
            {
                return isSocksProxy_;
            }
            set
            {
                isSocksProxy_ = value;
            }
        }

        /// <summary>
        /// The form encoding type to use.
        /// </summary>
        public FormEncodingType EncodingType
        {
            get
            {
                return encodingType_;
            }
            set
            {
                encodingType_ = value;
            }
        }

        /// <summary>
        /// Retrieves the request parameters to use. If set, these request parameters will overwrite any
        /// request parameters which may be present in the {@link #getUrl() URL}. Should not be used in
        /// combination with the {@link #setRequestBody(String) request body}.
        /// </summary>
        public List<NameValuePair> RequestParameters
        {
            get
            {
                return requestParameters_;
            }
            set
            {
                if (requestBody_ != null)
                {
                    String msg = "Trying to set the request parameters, but the request body has already been specified;"
                                     + "the two are mutually exclusive!";
                    throw new RuntimeException(msg);
                }
                requestParameters_ = value;
            }
        }

        /// <summary>
        /// Returns the body content to be submitted if this is a <tt>POST</tt> request. Ignored for all other request
        /// types. Should not be used in combination with {@link #setRequestParameters(List) request parameters}.
        /// @return the body content to be submitted if this is a <tt>POST</tt> request
        /// </summary>
        public String RequestBody
        {
            get
            {
                return requestBody_;
            }
            set
            {
                if (requestParameters_ != null && !requestParameters_.isEmpty())
                {
                    String msg = "Trying to set the request body, but the request parameters have already been specified;"
                               + "the two are mutually exclusive!";
                    throw new RuntimeException(msg);
                }
                if (httpMethod_ != HttpMethod.POST && httpMethod_ != HttpMethod.PUT && httpMethod_ != HttpMethod.PATCH)
                {
                    String msg = "The request body may only be set for POST, PUT or PATCH requests!";
                    throw new RuntimeException(msg);
                }
                requestBody_ = value;
            }
        }

        /// <summary>
        /// The HTTP submit method to use.
        /// </summary>
        public HttpMethod HttpMethod
        {
            get
            {
                return httpMethod_;
            }
            set
            {
                httpMethod_ = value;
            }
        }

        /// <summary>
        /// The additional HTTP headers to use.
        /// </summary>
        public Dictionary<String, String> AdditionalHeaders
        {
            get
            {
                return additionalHeaders_;
            }
            set
            {
                additionalHeaders_ = value;
            }
        }

        /// <summary>
        /// Instantiates a {@link WebRequest} for the specified URL.
        /// </summary>
        /// <param name="url">the target URL</param>
        /// <param name="acceptHeader">the accept header to use</param>
        public WebRequest(URL url, String acceptHeader)
        {
            this.Url = url;
            SetAdditionalHeader("Accept", acceptHeader);
            SetAdditionalHeader("Accept-Encoding", "gzip, deflate");
        }

        /**
         * Instantiates a {@link WebRequest} for the specified URL.
         * @param url the target URL
         */
        public WebRequest(URL url) :
            this(url, "*/*")
        {
        }

        /**
         * Instantiates a {@link WebRequest} for the specified URL using the specified HTTP submit method.
         * @param url the target URL
         * @param submitMethod the HTTP submit method to use
         */
        public WebRequest(URL url, HttpMethod submitMethod) :
            this(url)
        {
            this.HttpMethod = submitMethod;
        }

        /// <summary>
        /// The target URL
        /// </summary>
        public URL Url
        {
            get
            {
                return UrlUtils.toUrlSafe(url_);
            }
            set
            {
                URL url = value;
                if (url != null)
                {
                    String path = url.getPath();
                    if (path.isEmpty())
                    {
                        if (!url.getFile().isEmpty() || url.getProtocol().startsWith("http"))
                        {
                            url = buildUrlWithNewFile(url, "/" + url.getFile());
                        }
                    }
                    else if (path.contains("/."))
                    {
                        String query = (url.getQuery() != null) ? "?" + url.getQuery() : "";
                        url = buildUrlWithNewFile(url, removeDots(path) + query);
                    }
                    String idn = IDN.toASCII(url.getHost());
                    if (!idn.equals(url.getHost()))
                    {
                        try
                        {
                            url = new URL(url.getProtocol(), idn, url.getPort(), url.getFile());
                        }
                        catch (Exception e)
                        {
                            throw new RuntimeException("Cannot change hostname of URL: " + url.toExternalForm(), e);
                        }
                    }
                    url_ = url.toExternalForm();

                    // http://john.smith:secret@localhost
                    String userInfo = url.getUserInfo();
                    if (userInfo != null)
                    {
                        int splitPos = userInfo.indexOf(':');
                        if (splitPos == -1)
                        {
                            urlCredentials_ = new UsernamePasswordCredentials(userInfo, "");
                        }
                        else
                        {
                            String username = userInfo.substring(0, splitPos);
                            String password = userInfo.substring(splitPos + 1);
                            urlCredentials_ = new UsernamePasswordCredentials(username, password);
                        }
                    }
                }
                else
                {
                    url_ = null;
                }
            }
        }

        /// <summary>
        /// Strip a URL string of "/./" and "/../" occurrences.
        /// <p>
        /// One trick here is to repeatedly create new matchers on a given
        /// pattern, so that we can see whether it needs to be re-applied;
        /// unfortunately .replaceAll() doesn't re-process its own output,
        /// so if we create a new match with a replacement, it is missed.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private String RemoveDots(String path)
        {
            String newPath = path;

            // remove occurrences at the beginning
            newPath = REMOVE_DOTS_PATTERN.matcher(newPath).replaceAll("/");
            if ("/..".equals(newPath))
            {
                newPath = "/";
            }

            // single dots have no effect, so just remove them
            while (DOT_PATTERN.matcher(newPath).find())
            {
                newPath = DOT_PATTERN.matcher(newPath).replaceAll("/");
            }

            // mid-path double dots should be removed WITH the previous subdirectory and replaced
            //  with "/" BUT ONLY IF that subdirectory's not also ".." (a regex lookahead helps with this)
            while (DOT_DOT_PATTERN.matcher(newPath).find())
            {
                newPath = DOT_DOT_PATTERN.matcher(newPath).replaceAll("/");
            }

            return newPath;
        }

        private URL BuildUrlWithNewFile(URL url, String newFile)
        {
            try
            {
                if (url.getRef() != null)
                {
                    newFile += '#' + url.getRef();
                }
                if ("file".equals(url.getProtocol()) && url.getAuthority() != null && url.getAuthority().endsWith(":"))
                {
                    newFile = ":" + newFile;
                }
                url = new URL(url.getProtocol(), url.getHost(), url.getPort(), newFile);
            }
            catch (Exception e)
            {
                throw new RuntimeException("Cannot set URL: " + url.toExternalForm(), e);
            }
            return url;
        }

        /// <summary>
        /// Returns whether the specified header name is already included in the additional HTTP headers.
        /// </summary>
        /// <param name="name">the name of the additional HTTP header</param>
        /// <returns>true if the specified header name is included in the additional HTTP headers</returns>
        public bool IsAdditionalHeader(String name)
        {
            foreach (String key in additionalHeaders_.keySet())
            {
                if (name.equalsIgnoreCase(key))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Sets the specified name/value pair in the additional HTTP headers.
        /// </summary>
        /// <param name="name">the name of the additional HTTP header</param>
        /// <param name="value">value the value of the additional HTTP header</param>
        public void SetAdditionalHeader(String name, String value)
        {
            foreach (String key in additionalHeaders_.keySet())
            {
                if (name.equalsIgnoreCase(key))
                {
                    name = key;
                    break;
                }
            }
            additionalHeaders_.put(name, value);
        }

        /// <summary>
        /// Removed the specified name/value pair from the additional HTTP headers.
        /// </summary>
        /// <param name="name">the name of the additional HTTP header</param>
        public void RemoveAdditionalHeader(String name)
        {
            foreach (String key in additionalHeaders_.keySet())
            {
                if (name.equalsIgnoreCase(key))
                {
                    name = key;
                    break;
                }
            }
            additionalHeaders_.remove(name);
        }

        /// <summary>
        /// The credentials to use.
        /// </summary>
        public Credentials UrlCredentials
        {
            get
            {
                return urlCredentials_;
            }
        }

        /// <summary>
        /// The credentials to use.
        /// </summary>
        public Credentials Credentials
        {
            get
            {
                return credentials_;
            }
            set
            {
                credentials_ = value;
            }
        }

        /// <summary>
        /// The character set to use to perform the request.
        /// </summary>
        public String Charset
        {
            get
            {
                return charset_;
            }
            set
            {
                charset_ = value;
            }
        }

        /// <summary>
        /// Returns a string representation of this object.
        /// </summary>
        /// <returns>a string representation of this object</returns>
        public override String ToString()
        {
            StringBuilder buffer = new StringBuilder();
            buffer.Append(this.GetType().Name);
            buffer.Append("[<");
            buffer.Append("url=\"" + url_ + '"');
            buffer.Append(", " + httpMethod_);
            buffer.Append(", " + encodingType_);
            buffer.Append(", " + requestParameters_);
            buffer.Append(", " + additionalHeaders_);
            buffer.Append(", " + credentials_);
            buffer.Append(">]");
            return buffer.ToString();
        }
    }
}
