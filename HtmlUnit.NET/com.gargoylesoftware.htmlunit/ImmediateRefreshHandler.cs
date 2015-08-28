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
    /// This refresh handler immediately refreshes the specified page,
    /// using the specified URL and ignoring the wait time.
    /// 
    /// If you want a refresh handler that does not ignore the wait time,
    /// see {@link ThreadedRefreshHandler}.
    /// 
    /// @version $Revision: 10772 $
    /// @author <a href="mailto:mbowler@GargoyleSoftware.com">Mike Bowler</a>
    /// @author Daniel Gredler
    /// @author Marc Guillemot
    /// @author Ahmed Ashour
    /// </summary>
    [Serializable]
    public class ImmediateRefreshHandle : IRefreshHandler
    {

        /// <summary>
        /// Immediately refreshes the specified page using the specified URL.
        /// @throws IOException if the refresh fails
        /// </summary>
        /// <param name="page">the page that is going to be refreshed</param>
        /// <param name="url">the URL where the new page will be loaded</param>
        /// <param name="seconds">the number of seconds to wait before reloading the page (ignored!)</param>
        public void HandleRefresh(IPage page, URL url, int seconds)
        {
            IWebWindow window = page.EnclosingWindow;
            if (window == null)
            {
                return;
            }
            WebClient client = window.WebClient;
            if (String.Equals(page.Url.ToExternalForm(), url.ToExternalForm()) && HttpMethod.GET == page.WebResponse.WebRequest.HttpMethod)
            {
                String msg = "Refresh to " + url + " (" + seconds + "s) aborted by HtmlUnit: "
                    + "Attempted to refresh a page using an ImmediateRefreshHandler "
                    + "which could have caused an OutOfMemoryError "
                    + "Please use WaitingRefreshHandler or ThreadedRefreshHandler instead.";
                throw new RuntimeException(msg);
            }
            client.getPage(window, new WebRequest(url));
        }
    }
}