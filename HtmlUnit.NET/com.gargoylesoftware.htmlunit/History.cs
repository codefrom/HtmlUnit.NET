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
using HtmlUnit.Helpers;

namespace HtmlUnit.com.gargoylesoftware.htmlunit
{
    /// <summary>
    /// Representation of the navigation history of a single window.
    /// @version $Revision: 10803 $
    /// @author Daniel Gredler
    /// @author Ahmed Ashour
    /// </summary>
    [Serializable]
    public class History
    {
        /// <summary>The window to which this navigation history belongs.</summary>
        private IWebWindow window_;

        /// <summary>The {@link WebRequest}s of the pages in this navigation history.</summary>
        private readonly List<WebRequest> webRequests_ = new List<WebRequest>();

        /// <summary>
        /// Whether or not to ignore calls to {@link #addPage(Page)}; this is a bit hackish (we should probably be using
        /// explicit boolean parameters in the various methods that load new pages), but it does the job for now -- without
        /// any new API cruft.
        /// </summary>
        [NonSerialized]
        [ThreadStatic]
        private bool ignoreNewPages_;

        /// <summary>The current index within the list of pages which make up this navigation history.</summary>
        private int index_ = -1;

        /// <summary>
        /// Creates a new navigation history for the specified window.
        /// </summary>
        /// <param name="window">the window which owns the new navigation history</param>
        public History(IWebWindow window)
        {
            window_ = window;
            InitTransientFields();
        }

        /// <summary>
        /// Initializes the transient fields.
        /// </summary>
        private void InitTransientFields()
        {
            ignoreNewPages_ = false;
        }

        /// <summary>
        /// the length of the navigation history.
        /// </summary>
        public int Length
        {
            get
            {
                return webRequests_.Count;
            }
        }

        /// <summary>
        /// the current (zero-based) index within the navigation history.
        /// </summary>
        public int Index
        {
            get
            {
                return index_;
            }
        }

        /// <summary>
        /// Returns the URL at the specified index in the navigation history, or <tt>null</tt> if the index is not valid.
        /// </summary>
        /// <param name="index">the index of the URL to be returned</param>
        /// <returns>the URL at the specified index in the navigation history, or <tt>null</tt> if the index is not valid</returns>
        public URL GetUrl(int index)
        {
            if (index >= 0 && index < webRequests_.Count)
            {
                return UrlUtils.toUrlSafe(webRequests_[index].Url.ToExternalForm());
            }
            return null;
        }

        /// <summary>
        /// Goes back one step in the navigation history, if possible.
        /// @throws IOException if an IO error occurs
        /// </summary>
        /// <returns>this navigation history, after going back one step</returns>
        public History Back()
        {
            if (index_ > 0)
            {
                index_--;
                GoToUrlAtCurrentIndex();
            }
            return this;
        }

        /// <summary>
        /// Goes forward one step in the navigation history, if possible.
        /// @throws IOException if an IO error occurs
        /// </summary>
        /// <returns>this navigation history, after going forward one step</returns>
        public History Forward()
        {
            if (index_ < webRequests_.Count - 1)
            {
                index_++;
                GoToUrlAtCurrentIndex();
            }
            return this;
        }

        /// <summary>
        /// Goes forward or backwards in the navigation history, according to whether the specified relative index
        /// is positive or negative. If the specified index is <tt>0</tt>, this method reloads the current page.
        /// @throws IOException if an IO error occurs
        /// </summary>
        /// <param name="relativeIndex">the index to move to, relative to the current index</param>
        /// <returns>this navigation history, after going forwards or backwards the specified number of steps</returns>
        public History Go(int relativeIndex)
        {
            int i = index_ + relativeIndex;
            if (i < webRequests_.Count && i >= 0)
            {
                index_ = i;
                GoToUrlAtCurrentIndex();
            }
            return this;
        }

        /// <summary>
        /// {@inheritDoc}
        /// </summary>
        /// <returns></returns>
        public override String ToString()
        {
            return webRequests_.ToString();
        }

        /// <summary>
        /// Removes the current URL from the history.
        /// </summary>
        public void RemoveCurrent()
        {
            if (index_ >= 0 && index_ < webRequests_.Count)
            {
                webRequests_.RemoveAt(index_);
                if (index_ > 0)
                {
                    index_--;
                }
            }
        }

        /// <summary>
        /// Adds a new page to the navigation history.
        /// </summary>
        /// <param name="page">the page to add to the navigation history</param>
        protected void AddPage(IPage page)
        {
            bool ignoreNewPages = ignoreNewPages_;
            if (ignoreNewPages != null && ignoreNewPages)
            {
                return;
            }
            index_++;
            while (webRequests_.Count > index_)
            {
                webRequests_.RemoveAt(index_);
            }
            WebRequest request = page.WebResponse.WebRequest;
            WebRequest newRequest = new WebRequest(request.Url, request.HttpMethod);
            newRequest.RequestParameters = request.RequestParameters;
            webRequests_.Add(newRequest);
        }

        /// <summary>
        /// Loads the URL at the current index into the window to which this navigation history belongs.
        /// @throws IOException if an IO error occurs
        /// </summary>
        private void GoToUrlAtCurrentIndex()
        {
            WebRequest request = webRequests_[index_];

            bool old = ignoreNewPages_;
            try
            {
                ignoreNewPages_ = true;
                window_.WebClient.GetPage(window_, request);
            }
            finally
            {
                ignoreNewPages_ = old;
            }
        }

        /**
         * Re-initializes transient fields when an object of this type is deserialized.
         * @param in the object input stream
         * @throws IOException if an error occurs
         * @throws ClassNotFoundException if an error occurs
         */
        /*    private void ReadObject(Stream input)
            {
                in.defaultReadObject();
                initTransientFields();
            }*/
    }
}