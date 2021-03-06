﻿/*
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
    /// This class is notified when AJAX calls are made, and has the ability to influence these calls.
    /// For instance, it can turn asynchronous AJAX calls into synchronous AJAX calls, making test code
    /// deterministic and avoiding calls to <tt>Thread.sleep()</tt>.
    /// @version $Revision: 10772 $
    /// @author Marc Guillemot
    /// </summary>
    [Serializable]
    public class AjaxController
    {
        /// <summary>
        /// Gets notified of an AJAX call to determine how it should be processed.
        /// </summary>
        /// <param name="page">the page the request comes from</param>
        /// <param name="request">the request that should be performed</param>
        /// <param name="async">indicates if the request should originally be asynchronous</param>
        /// <returns>if the call should be synchronous or not; here just like the original call</returns>
        public bool ProcessSynchron(HtmlPage page, WebRequest request, bool async)
        {
            return !async;
        }
    }
}
