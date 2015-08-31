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
using net.sourceforge.htmlunit.corejs.javascript;

namespace HtmlUnit.com.gargoylesoftware.htmlunit
{

    /// <summary>
    /// This object contains the result of executing a chunk of script code.
    /// @version $Revision: 9837 $
    /// @author <a href="mailto:mbowler@GargoyleSoftware.com">Mike Bowler</a>
    /// @author Marc Guillemot
    /// </summary>
    public sealed class ScriptResult
    {
        /// <summary>The object that was returned from the script engine.</summary>
        private readonly Object javaScriptResult_;

        /// <summary>The page that is currently loaded at the end of the script execution.</summary>
        private readonly IPage newPage_;

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="javaScriptResult">the object that was returned from the script engine</param>
        /// <param name="newPage">the page that is currently loaded at the end of the script execution</param>
        public ScriptResult(Object javaScriptResult, IPage newPage)
        {
            javaScriptResult_ = javaScriptResult;
            newPage_ = newPage;
        }

        /// <summary>
        /// the object that was the output of the script engine.
        /// </summary>
        public Object JavaScriptResult
        {
            get
            {
                return javaScriptResult_;
            }
        }

        /// <summary>
        /// the page that is loaded at the end of the script execution.
        /// </summary>
        public IPage NewPage
        {
            get
            {
                return newPage_;
            }
        }

        /// <summary>
        /// {@inheritDoc}
        /// </summary>
        /// <returns></returns>
        public override String ToString()
        {
            return "ScriptResult[result=" + javaScriptResult_ + " page=" + newPage_ + "]";
        }

        /// <summary>
        /// Utility method testing if a script result is <tt>false</tt>.
        /// </summary>
        /// <param name="scriptResult">a script result (may be <tt>null</tt>)</param>
        /// <returns><tt>true</tt> if <tt>scriptResult</tt> is <tt>false</tt></returns>
        public static bool IsFalse(ScriptResult scriptResult)
        {
            return scriptResult != null && String.Equals(Boolean.FalseString, scriptResult.JavaScriptResult);
        }

        /// <summary>
        /// Utility method testing if a script result is undefined (there was no return value).
        /// </summary>
        /// <param name="scriptResult">a script result (may be <tt>null</tt>)</param>
        /// <returns><tt>true</tt> if <tt>scriptResult</tt> is undefined (there was no return value)</returns>
        public static bool IsUndefined(ScriptResult scriptResult)
        {
            return scriptResult != null && scriptResult.JavaScriptResult is Undefined;
        }

        /// <summary>
        /// Creates and returns a composite {@link ScriptResult} based on the two input {@link ScriptResult}s. This
        /// method defines how the return values for multiple event handlers are combined during event capturing and
        /// bubbling. The behavior of this method varies based on whether or not we are emulating IE.
        /// </summary>
        /// <param name="newResult">the new {@link ScriptResult} (may be <tt>null</tt>)</param>
        /// <param name="originalResult">the original {@link ScriptResult} (may be <tt>null</tt>)</param>
        /// <param name="ie">whether or not we are emulating IE</param>
        /// <returns>a composite {@link ScriptResult}, based on the two input {@link ScriptResult}s</returns>
        public static ScriptResult Combine(ScriptResult newResult, ScriptResult originalResult, bool ie)
        {
            Object jsResult;
            IPage page;

            // If we're emulating IE, the overall JavaScript return value is the last return value.
            // If we're emulating FF, the overall JavaScript return value is false if the return value
            // was false at any level.
            if (ie)
            {
                if (newResult != null && !ScriptResult.IsUndefined(newResult))
                {
                    jsResult = newResult.JavaScriptResult;
                }
                else if (originalResult != null)
                {
                    jsResult = originalResult.JavaScriptResult;
                }
                else
                {
                    jsResult = null;
                }
            }
            else
            {
                if (ScriptResult.IsFalse(newResult))
                {
                    jsResult = newResult.JavaScriptResult;
                }
                else if (originalResult != null)
                {
                    jsResult = originalResult.JavaScriptResult;
                }
                else
                {
                    jsResult = null;
                }
            }

            // The new page is always the newest page.
            if (newResult != null)
            {
                page = newResult.NewPage;
            }
            else if (originalResult != null)
            {
                page = originalResult.NewPage;
            }
            else
            {
                page = null;
            }

            // Build and return the composite script result.
            if (jsResult == null && page == null)
            {
                return null;
            }
            return new ScriptResult(jsResult, page);
        }
    }
}