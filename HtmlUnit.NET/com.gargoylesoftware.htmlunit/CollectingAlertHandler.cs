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
    /// A simple alert handler that keeps track of alerts in a list.
    /// @version $Revision: 10772 $
    /// @author <a href="mailto:mbowler@GargoyleSoftware.com">Mike Bowler</a>
    /// </summary>
    [Serializable]
    public class CollectingAlertHandler : IAlertHandler
    {
        private readonly List<String> collectedAlerts_;

        /// <summary>
        /// Creates a new instance, initializing it with an empty list.
        /// </summary>
        public CollectingAlertHandler() :
            this(new List<String>())
        {
        }

        /// <summary>
        /// Creates an instance with the specified list.
        /// </summary>
        /// <param name="list">the list to store alerts in</param>
        public CollectingAlertHandler(List<String> list)
        {
            WebAssert.notNull("list", list);
            collectedAlerts_ = list;
        }

        /// <summary>
        /// Handles the alert. This implementation will store the message in a list
        /// for retrieval later.
        /// </summary>
        /// <param name="page">the page that triggered the alert</param>
        /// <param name="message">the message in the alert</param>
        public void HandleAlert(IPage page, String message)
        {
            collectedAlerts_.Add(message);
        }

        /// <summary>
        /// Returns a list containing the message portion of any collected alerts.
        /// </summary>
        /// <returns>a list of alert messages</returns>
        public List<String> getCollectedAlerts()
        {
            return collectedAlerts_;
        }
    }
}
