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
using HtmlUnit.Helpers;

namespace HtmlUnit.com.gargoylesoftware.htmlunit
{
    /// <summary>
    /// A basic {@link Page} implementation.
    /// @version $Revision: 10913 $
    /// @author Ahmed Ashour
    /// </summary>
    [Serializable]
    public class AbstractPage : IPage
    {
        private WebResponse webResponse_;
        private IWebWindow enclosingWindow_;

        /**
         * Returns the web response that was originally used to create this page.
         *
         * @return the web response that was originally used to create this page
         */
        public WebResponse WebResponse
        {
            get
            {
                return webResponse_;
            }
        }

        /// <summary>
        /// The window that this page is sitting inside.
        /// </summary>
        public IWebWindow EnclosingWindow 
        {
            get
            {
                return enclosingWindow_;
            }
        }

        /// <summary>
        /// The URL of this page.
        /// </summary>
        public URL Url
        {
            get
            {
                return webResponse_.WebRequest.Url;
            }
        }

        /// <summary>
        /// Creates an instance.
        /// </summary>
        /// <param name="webResponse">the response from the server</param>
        /// <param name="enclosingWindow">the window that holds the page</param>
        public AbstractPage(WebResponse webResponse, IWebWindow enclosingWindow)
        {
            webResponse_ = webResponse;
            enclosingWindow_ = enclosingWindow;
        }

        /// <summary>
        /// Initializes this page.
        /// </summary>
        public void Initialize()
        {
            // nothing to do
        }

        /// <summary>
        /// Cleans up this page.
        /// </summary>
        public void CleanUp()
        {
            if (EnclosingWindow.WebClient.Cache.GetCachedResponse(webResponse_.WebRequest) == null)
            {
                webResponse_.CleanUp();
            }
        }

        public bool IsHtmlPage()
        {
            return false;
        }
    }
}