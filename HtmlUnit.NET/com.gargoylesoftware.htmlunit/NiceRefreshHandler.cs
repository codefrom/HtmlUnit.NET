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
using HtmlUnit.Helpers;

namespace HtmlUnit.com.gargoylesoftware.htmlunit
{
    /// <summary>
    /// This refresh handler performs an immediate refresh if the refresh delay is
    /// less or equal to the configured time and otherwise ignores totally the refresh instruction.
    /// @version $Revision: 9837 $
    /// @author Marc Guillemot
    /// </summary>
    public class NiceRefreshHandler : ImmediateRefreshHandler
    {
        private readonly int maxDelay_;

        /// <summary>
        /// Creates a new refresh handler that will immediately refresh if the refresh delay is no
        /// longer than <tt>maxDelay</tt>. No refresh will be perform at all for refresh values
        /// larger than <tt>maxDelay</tt>.
        /// </summary>
        /// <param name="maxDelay">the maximum refreshValue (in seconds) that should cause a refresh</param>
        public NiceRefreshHandler(int maxDelay)
        {
            if (maxDelay <= 0)
            {
                throw new ArgumentException("Invalid maxDelay: " + maxDelay);
            }
            maxDelay_ = maxDelay;
        }

        /// <summary>
        /// Refreshes the specified page using the specified URL immediately if the <tt>requestedWait</tt>
        /// not larget that the <tt>maxDelay</tt>. Does nothing otherwise.
        /// @throws IOException if the refresh fails
        /// </summary>
        /// <param name="page">the page that is going to be refreshed</param>
        /// <param name="url">the URL where the new page will be loaded</param>
        /// <param name="requestedWait">the number of seconds to wait before reloading the page</param>
        public void HandleRefresh(IPage page, URL url, int requestedWait)
        {
            if (requestedWait > maxDelay_)
            {
                return;
            }

            base.HandleRefresh(page, url, requestedWait);
        }
    }
}
