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

namespace HtmlUnit.com.gargoylesoftware.htmlunit
{
    /// <summary>
    /// A {@link WebResponse} implementation to deliver with content from cache. The response
    /// is the same but the request may have some variation like an anchor.
    /// @version $Revision: 9837 $
    /// @author Marc Guillemot
    /// </summary>
    public class WebResponseFromCache : WebResponseWrapper
    {
        private WebRequest request_;

        /// <summary>
        /// Wraps the provide response for the given request
        /// </summary>
        /// <param name="cachedResponse">the response from cache</param>
        /// <param name="currentRequest">the new request</param>
        public WebResponseFromCache(WebResponse cachedResponse, WebRequest currentRequest) :
            base(cachedResponse)
        {
            request_ = currentRequest;
        }

        /// <summary>
        /// {@inheritDoc}
        /// </summary>
        /// <returns></returns>
        public WebRequest getWebRequest()
        {
            return request_;
        }
    }
}
