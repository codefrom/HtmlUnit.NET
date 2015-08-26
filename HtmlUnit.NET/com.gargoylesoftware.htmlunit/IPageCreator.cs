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
    /// Something that knows how to create a page object. It is also the responsibility
    /// of the page creator to establish the relationship between the <code>webWindow</code>
    /// and the page, usually by calling {@link WebWindow#setEnclosedPage(Page)}. This should
    /// be done as early as possible, e.g. to allow for re-loading of pages during page parsing.
    /// 
    /// @version $Revision: 10103 $
    /// @author <a href="mailto:mbowler@GargoyleSoftware.com">Mike Bowler</a>
    /// @author <a href="mailto:cse@dynabean.de">Christian Sell</a>
    /// </summary>
    public interface IPageCreator
    {
        /// <summary>
        /// Create a Page object for the specified web response.
        /// @exception IOException If an io problem occurs
        /// </summary>
        /// <param name="webResponse">the response from the server</param>
        /// <param name="webWindow">the window that this page will be loaded into</param>
        /// <returns>the new page</returns>
        public IPage CreatePage(WebResponse webResponse, WebWindow webWindow);
    }
}
