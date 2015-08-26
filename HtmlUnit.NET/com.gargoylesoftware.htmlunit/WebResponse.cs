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
using System.IO;
using System.Text;
using HtmlUnit.com.gargoylesoftware.htmlunit.util;
using HtmlUnit.Helpers;

namespace HtmlUnit.com.gargoylesoftware.htmlunit
{
    /// <summary>
    /// A response from a web server.
    /// @version $Revision: 10905 $
    /// @author <a href="mailto:mbowler@GargoyleSoftware.com">Mike Bowler</a>
    /// @author Brad Clarke
    /// @author Noboru Sinohara
    /// @author Marc Guillemot
    /// @author Ahmed Ashour
    /// @author Ronald Brill
    /// </summary>
    [Serializable]
    public class WebResponse
    {
        private static readonly Log LOG = LogFactory.GetLog(typeof (WebResponse));

        private long loadTime_;
        private WebResponseData responseData_;
        private WebRequest request_;

        /// <summary>
        /// The request used to load this response.
        /// </summary>
        public WebRequest WebRequest
        {
            get
            {
                return request_;
            }
        }

        /// <summary>
        /// The response headers as a list of {@link NameValuePair}s.
        /// </summary>
        public List<NameValuePair> ResponseHeaders
        {
            get
            {
                return responseData_.ResponseHeaders;
            }
        }

        /// <summary>
        /// The status code that was returned by the server.
        /// </summary>
        public int StatusCode
        {
            get
            {
                return responseData_.StatusCode;
            }
        }

        /// <summary>
        /// The status message that was returned from the server.
        /// </summary>
        public String StatusMessage
        {
            get
            {
                return responseData_.StatusMessage;
            }
        }

        /// <summary>
        /// Returns the time it took to load this web response, in milliseconds.
        /// </summary>
        /// <returns>the time it took to load this web response, in milliseconds</returns>
        public long LoadTime
        {
            get
            {
                return loadTime_;
            }
        }

        /// <summary>
        /// Constructs with all data.
        /// </summary>
        /// <param name="responseData">Data that was send back</param>
        /// <param name="url">Where this response came from</param>
        /// <param name="requestMethod">the method used to get this response</param>
        /// <param name="loadTime">How long the response took to be sent</param>
        public WebResponse(WebResponseData responseData, URL url, HttpMethod requestMethod, long loadTime) :
            this(responseData, new WebRequest(url, requestMethod), loadTime)
        {
        }

        /// <summary>
        /// Constructs with all data.
        /// </summary>
        /// <param name="responseData">Data that was send back</param>
        /// <param name="request">the request used to get this response</param>
        /// <param name="loadTime">How long the response took to be sent</param>
        public WebResponse(WebResponseData responseData, WebRequest request, long loadTime)
        {
            responseData_ = responseData;
            request_ = request;
            loadTime_ = loadTime;
        }


        /// <summary>
        /// Returns the value of the specified response header.
        /// </summary>
        /// <param name="headerName">the name of the header whose value is to be returned</param>
        /// <returns>the header value, {@code null} if no response header exists with this name</returns>
        public String ResponseHeaderValue(String headerName)
        {
            foreach (NameValuePair pair in responseData_.ResponseHeaders)
            {
                if (String.Equals(pair.Name, headerName, StringComparison.InvariantCultureIgnoreCase))
                {
                    return pair.Value;
                }
            }
            return null;
        }

        /// <summary>
        /// Returns the content type returned from the server, e.g. "text/html".
        /// </summary>
        /// <returns>the content type returned from the server, e.g. "text/html"</returns>
        public String GetContentType()
        {
            String contentTypeHeader = ResponseHeaderValue("content-type");
            if (contentTypeHeader == null)
            {
                // Not technically legal but some servers don't return a content-type
                return "";
            }
            int index = contentTypeHeader.IndexOf(';');
            if (index == -1)
            {
                return contentTypeHeader;
            }
            return contentTypeHeader.Substring(0, index);
        }

        /// <summary>
        /// Returns the content charset specified explicitly in the header or in the content,
        /// or <tt>null</tt> if none was specified.
        /// </summary>
        /// <returns>the content charset specified explicitly in the header or in the content, or <tt>null</tt> if none was specified</returns>
        public String GetContentCharsetOrNull()
        {
            InputStream stream = null;
            try
            {
                stream = GetContentAsStream();
                return EncodingSniffer.SniffEncoding(ResponseHeaders, stream);
            }
            catch (IOException e)
            {
                LOG.Warn("Error trying to sniff encoding.", e);
                return null;
            }
            finally
            {
                IOUtils.CloseQuietly(stream);
            }
        }

        /// <summary>
        /// Returns the content charset for this response, even if no charset was specified explicitly.
        /// This method always returns a valid charset. This method first checks the "Content-Type"
        /// header; if not found, it checks the request charset; as a last resort, this method
        /// returns {@link TextUtil#DEFAULT_CHARSET}.
        /// If no charset is defined for an xml respose, then UTF-8 is used
        /// @see <a href="http://www.w3.org/TR/xml/#charencoding">Character Encoding</a>
        /// </summary>
        /// <returns>the content charset for this response</returns>
        public String GetContentCharset()
        {
            String charset = GetContentCharsetOrNull();
            if (charset == null)
            {
                String contentType = GetContentType();

                // xml pages are using a different content type
                if (null != contentType
                    && PageType.XML == DefaultPageCreator.DeterminePageType(contentType))
                {
                    return XmlUtil.DEFAULT_CHARSET;
                }

                charset = WebRequest.Charset;
            }
            if (charset == null)
            {
                charset = TextUtil.DEFAULT_CHARSET;
            }
            return charset;
        }

        /// <summary>
        /// Returns the response content as a string, using the charset/encoding specified in the server response.
        /// </summary>
        /// <returns>the response content as a string, using the charset/encoding specified in the server response</returns>
        public String GetContentAsString()
        {
            return GetContentAsString(GetContentCharset());
        }

        /// <summary>
        /// Returns the response content as a string, using the specified charset/encoding,
        /// rather than the charset/encoding specified in the server response. If the specified
        /// charset/encoding is not supported then the default system encoding is used.
        /// </summary>
        /// <param name="encoding">the charset/encoding to use to convert the response content into a string</param>
        /// <returns>the response content as a string</returns>
        public String GetContentAsString(String encoding)
        {
            InputStream stream = null;
            try
            {
                stream = responseData_.GetInputStream();
                if (null == stream)
                {
                    return null;
                }

                // first verify the charset because we can't read the
                // input stream twice
                try
                {
                    Encoding.GetEncoding(encoding);
                }
                catch (Exception e)
                {
                    String cs = GetContentCharset();
                    LOG.Warn("Attempted to use unsupported encoding '"
                             + encoding + "'; using default content charset ('" + cs + "').");
                    return IOUtils.ToString(stream, cs);
                }

                return IOUtils.ToString(stream, encoding);
            }
            catch (IOException e)
            {
                LOG.Warn(e);
                return null;
            }
            finally
            {
                IOUtils.CloseQuietly(stream);
            }
        }

        /// <summary>
        /// Returns the response content as an input stream.
        /// @throws IOException in case of IOProblems
        /// </summary>
        /// <returns>the response content as an input stream</returns>
        public InputStream GetContentAsStream()
        {
            return responseData_.GetInputStream();
        }

        /// <summary>
        /// Clean up the response data.
        /// </summary>
        public void CleanUp()
        {
            if (responseData_ != null)
            {
                responseData_.CleanUp();
            }
        }
    }
}
