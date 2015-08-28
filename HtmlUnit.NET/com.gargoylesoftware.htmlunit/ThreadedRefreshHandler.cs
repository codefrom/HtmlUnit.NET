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
using System.Threading;
using HtmlUnit.Helpers;

namespace HtmlUnit.com.gargoylesoftware.htmlunit
{
    /// <summary>
    /// This refresh handler spawns a new thread that waits the specified
    /// number of seconds before refreshing the specified page, using the
    /// specified URL.
    /// 
    /// If you want a refresh handler that ignores the wait time, see
    /// {@link ImmediateRefreshHandler}.
    /// 
    /// @version $Revision: 10772 $
    /// @author <a href="mailto:mbowler@GargoyleSoftware.com">Mike Bowler</a>
    /// @author Daniel Gredler
    /// </summary>
    public class ThreadedRefreshHandler : IRefreshHandler
    {
        /// <summary>Logging support.</summary>
        private static readonly Log LOG = LogFactory.GetLog(typeof(ThreadedRefreshHandler));

        /// <summary>
        /// Refreshes the specified page using the specified URL after the specified number
        /// of seconds.
        /// </summary>
        /// <param name="page">the page that is going to be refreshed</param>
        /// <param name="url">the URL where the new page will be loaded</param>
        /// <param name="seconds">the number of seconds to wait before reloading the page</param>
        public void HandleRefresh(IPage page, URL url, int seconds)
        {
            Thread thread = new Thread(new ThreadStart(() =>
            {
                try
                {
                    new WaitingRefreshHandler().HandleRefresh(page, url, seconds);
                }
                catch (IOException e)
                {
                    LOG.Error("Unable to refresh page!", e);
                    throw new SystemException("Unable to refresh page!", e);
                }
            }));
            // TODO : thread.setDaemon(true);
            thread.Start();
        }
    }
}
