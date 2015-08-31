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
using System.Threading;
using HtmlUnit.Helpers;

namespace HtmlUnit.com.gargoylesoftware.htmlunit
{
    /// <summary>
    /// This refresh handler waits the specified number of seconds (or a user defined maximum)
    /// before refreshing the specified page, using the specified URL. Waiting happens
    /// on the current thread
    /// If you want a refresh handler that ignores the wait time, see
    /// {@link ImmediateRefreshHandler}.
    /// @version $Revision: 10772 $
    /// @author <a href="mailto:mbowler@GargoyleSoftware.com">Mike Bowler</a>
    /// @author Daniel Gredler
    /// </summary>
    public class WaitingRefreshHandler : IRefreshHandler
    {
        /// <summary>Logging support.</summary>
        private static readonly Log LOG = LogFactory.GetLog(typeof(WaitingRefreshHandler));

        private readonly int maxwait_;

        /// <summary>
        /// Creates a new refresh handler that will wait whatever time the server or content asks, unless
        /// it it longer than <tt>maxwait</tt>. A value of <tt>maxwait</tt> that is less than <tt>1</tt>
        /// will cause the refresh handler to always wait for whatever time the server or content requests.
        /// </summary>
        /// <param name="maxwait">the maximum wait time before the refresh (in seconds)</param>
        public WaitingRefreshHandler(int maxwait)
        {
            maxwait_ = maxwait;
        }

        /// <summary>
        /// Creates a new refresh handler that will always wait whatever time the server or content asks.
        /// </summary>
        public WaitingRefreshHandler()
        {
            maxwait_ = 0;
        }

        /// <summary>
        /// Refreshes the specified page using the specified URL after the specified number of seconds.
        /// @throws IOException if the refresh fails
        /// </summary>
        /// <param name="page">the page that is going to be refreshed</param>
        /// <param name="url">the URL where the new page will be loaded</param>
        /// <param name="requestedWait">the number of seconds to wait before reloading the page; if this is greater than <tt>maxwait</tt> then <tt>maxwait</tt> will be used instead</param>
        public void HandleRefresh(AbstractPage page, URL url, int requestedWait)
        {
            int seconds = requestedWait;
            if (seconds > maxwait_ && maxwait_ > 0)
            {
                seconds = maxwait_;
            }
            try
            {
                Thread.Sleep(seconds * 1000);
            }
            catch (/*InterruptedException*/Exception e)
            {
                /* This can happen when the refresh is happening from a navigation that started
                 * from a setTimeout or setInterval. The navigation will cause all threads to get
                 * interrupted, including the current thread in this case. It should be safe to
                 * ignore it since this is the thread now doing the navigation. Eventually we should
                 * refactor to force all navigation to happen back on the main thread.
                 */
                if (LOG.IsDebugEnabled)
                {
                    LOG.Debug("Waiting thread was interrupted. Ignoring interruption to continue navigation.");
                }
            }
            IWebWindow window = page.EnclosingWindow;
            if (window == null)
            {
                return;
            }
            WebClient client = window.WebClient;
            client.getPage(window, new WebRequest(url));
        }
    }
}
