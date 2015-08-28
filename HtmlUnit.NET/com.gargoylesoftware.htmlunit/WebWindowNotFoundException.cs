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
    /// Exception to indicate that no {@link WebWindow} could be found that matched
    /// a given name.
    /// @version $Revision: 9837 $
    /// @author <a href="mailto:mbowler@GargoyleSoftware.com">Mike Bowler</a>
    /// </summary>
    public class WebWindowNotFoundException : SystemException
    {
        private readonly String name_;

        /// <summary>
        /// Creates an instance.
        /// </summary>
        /// <param name="name">the name that was searched by</param>
        public WebWindowNotFoundException(String name) :
            base("Searching for [" + name + "]")
        {
            name_ = name;
        }

        /// <summary>
        /// The name of the {@link WebWindow} that wasn't found.
        /// </summary>
        public String Name
        {
            get
            {
                return name_;
            }
        }
    }
}
