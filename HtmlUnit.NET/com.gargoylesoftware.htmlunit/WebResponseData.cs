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
using System.Linq;
using System.Text;
using HtmlUnit.com.gargoylesoftware.htmlunit.util;
using HtmlUnit.Helpers;

namespace HtmlUnit.com.gargoylesoftware.htmlunit
{
    /// <summary>
    /// Simple data object to simplify WebResponse creation.
    /// @version $Revision: 9837 $
    /// @author Brad Clarke
    /// @author Daniel Gredler
    /// @author Ahmed Ashour
    /// @author Ronald Brill
    /// </summary>
    [Serializable]
    public class WebResponseData
    {
        private readonly int statusCode_;
        private readonly String statusMessage_;
        private readonly List<NameValuePair> responseHeaders_;
        private readonly DownloadedContent downloadedContent_;

        /// <summary>
        /// Response headers
        /// </summary>
        public List<NameValuePair> ResponseHeaders
        {
            get
            {
                return responseHeaders_;
            }
        }

        /// <summary>
        /// Response status code
        /// </summary>
        public int StatusCode
        {
            get
            {
                return statusCode_;
            }
        }

        /// <summary>
        /// Response status message
        /// </summary>
        public String StatusMessage
        {
            get
            {
                return statusMessage_;
            }
        }

        /// <summary>
        /// Constructs with a raw byte[] (mostly for testing).
        /// </summary>
        /// <param name="body">Body of this response</param>
        /// <param name="statusCode">Status code from the server</param>
        /// <param name="statusMessage">Status message from the server</param>
        /// <param name="responseHeaders">Headers in this response</param>
        public WebResponseData(byte[] body, int statusCode, String statusMessage,
                List<NameValuePair> responseHeaders) :
            this(new DownloadedContent.InMemory(body), statusCode, statusMessage, responseHeaders)
        {
        }

        /// <summary>
        /// Constructs without data stream for subclasses that override getBody().
        /// </summary>
        /// <param name="statusCode">Status code from the server</param>
        /// <param name="statusMessage">Status message from the server</param>
        /// <param name="responseHeaders">Headers in this response</param>
        protected WebResponseData(int statusCode,
                String statusMessage, List<NameValuePair> responseHeaders) :
            this(ArrayUtils.EMPTY_BYTE_ARRAY, statusCode, statusMessage, responseHeaders)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="downloadedContent">the downloaded content</param>
        /// <param name="statusCode">Status code from the server</param>
        /// <param name="statusMessage">Status message from the server</param>
        /// <param name="responseHeaders">Headers in this response</param>
        public WebResponseData(DownloadedContent downloadedContent, int statusCode, String statusMessage,
                List<NameValuePair> responseHeaders)
        {
            statusCode_ = statusCode;
            statusMessage_ = statusMessage;
            responseHeaders_ = Collections.unmodifiableList(responseHeaders);
            downloadedContent_ = downloadedContent;
        }

        private InputStream GetStream(DownloadedContent downloadedContent,
                    List<NameValuePair> headers)
        {

            InputStream stream = downloadedContent_.InputStream;
            if (stream == null)
            {
                return null;
            }

            if (downloadedContent.IsEmpty())
            {
                return stream;
            }

            String encoding = GetHeader(headers, "content-encoding");
            if (encoding != null)
            {
                if (StringUtils.contains(encoding, "gzip"))
                {
                    stream = new GZIPInputStream(stream);
                }
                else if (StringUtils.contains(encoding, "deflate"))
                {
                    bool zlibHeader = false;
                    if (stream.MarkSupported)
                    { // should be always the case as the content is in a byte[] or in a file
                        stream.Mark(2);
                        byte[] buffer = new byte[2];
                        stream.Read(buffer, 0, 2);
                        zlibHeader = (((buffer[0] & 0xff) << 8) | (buffer[1] & 0xff)) == 0x789c;
                        stream.Reset();
                    }
                    if (zlibHeader)
                    {
                        stream = new InflaterInputStream(stream);
                    }
                    else
                    {
                        stream = new InflaterInputStream(stream, new Inflater(true));
                    }
                }
            }
            return stream;
        }

        private String GetHeader(List<NameValuePair> headers, String name)
        {
            foreach (NameValuePair header in headers)
            {
                String headerName = header.Name.Trim();
                if (String.Equals(name, headerName, StringComparison.InvariantCultureIgnoreCase))
                {
                    return header.Value;
                }
            }

            return null;
        }

        /// <summary>
        /// Returns the response body.
        /// This may cause memory problem for very large responses.
        /// </summary>
        /// <returns>response body</returns>
        public byte[] GetBody()
        {
            try
            {
                return IOUtils.toByteArray(GetInputStream());
            }
            catch (IOException e)
            {
                throw new RuntimeException(e); // shouldn't we allow the method to throw IOException?
            }
        }

        /// <summary>
        /// Returns a new {@link InputStream} allowing to read the downloaded content.
        /// @throws IOException in case of IO problems
        /// </summary>
        /// <returns>the associated InputStream</returns>
        public InputStream GetInputStream()
        {
            return GetStream(downloadedContent_, ResponseHeaders);
        }

        /// <summary>
        /// Clean up the downloaded content.
        /// </summary>
        public void CleanUp()
        {
            downloadedContent_.cleanUp();
        }
    }
}
