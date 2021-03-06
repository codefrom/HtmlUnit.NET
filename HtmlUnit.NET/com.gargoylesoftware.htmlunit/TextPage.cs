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
    /// A generic page that will be returned for any text related content.
    /// Specifically any content types that start with {@code text/}
    /// @version $Revision: 10913 $
    /// @author <a href="mailto:mbowler@GargoyleSoftware.com">Mike Bowler</a>
    /// @author David K. Taylor
    /// @author Ronald Brill
    /// @author Ahmed Ashour
    /// </summary>
    public class TextPage : AbstractPage
    {
        /// <summary>
        /// Creates an instance.
        /// </summary>
        /// <param name="webResponse">the response from the server</param>
        /// <param name="enclosingWindow">the window that holds the page</param>
        public TextPage(WebResponse webResponse, IWebWindow enclosingWindow) :
            base(webResponse, enclosingWindow)
        {
        }

        /// <summary>
        /// Returns the content of this page.
        /// </summary>
        /// <returns>the content of this page</returns>
        public String GetContent()
        {
            return WebResponse.GetContentAsString();
        }
    }
}
