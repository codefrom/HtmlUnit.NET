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
    /// A script pre processor call back. This interface is called when ScriptEngine.execute()
    /// is called. It gives developers the opportunity to modify the script to be executed.
    /// @version $Revision: 10103 $
    /// @author <a href="mailto:bcurren@esomnie.com">Ben Curren</a>
    /// </summary>
    public interface IScriptPreProcessor
    {
        /// <summary>
        /// Pre process the specified source code in the context of the given page.
        /// </summary>
        /// <param name="htmlPage">the page</param>
        /// <param name="sourceCode">the code to execute</param>
        /// <param name="sourceName">a name for the chunk of code that is going to be executed (used in error messages)</param>
        /// <param name="lineNumber">the line number of the source code</param>
        /// <param name="htmlElement">the HTML element that will act as the context</param>
        /// <returns>the source code after pre processing</returns>
        public String PreProcess(HtmlPage htmlPage, String sourceCode, String sourceName, int lineNumber, HtmlElement htmlElement);
    }
}
