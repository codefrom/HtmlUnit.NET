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
    /// An exception that is thrown when a specified XML element cannot be found in the DOM model.
    /// @version $Revision: 9837 $
    /// @author <a href="mailto:mbowler@GargoyleSoftware.com">Mike Bowler</a>
    /// </summary>
    public class ElementNotFoundException : SystemException
    {
        private readonly String elementName_;
        private readonly String attributeName_;
        private readonly String attributeValue_;

        /// <summary>
        /// Creates an instance from the variables that were used to search for the XML element.
        /// </summary>
        /// <param name="elementName">the name of the element</param>
        /// <param name="attributeName">the name of the attribute</param>
        /// <param name="attributeValue">the value of the attribute</param>
        public ElementNotFoundException(String elementName, String attributeName, String attributeValue) :
            base("elementName=[" + elementName
                      + "] attributeName=[" + attributeName
                      + "] attributeValue=[" + attributeValue + "]")
        {
            elementName_ = elementName;
            attributeName_ = attributeName;
            attributeValue_ = attributeValue;
        }

        /// <summary>
        /// Returns the name of the element.
        /// </summary>
        /// <returns>the name of the element</returns>
        public String ElementName
        {
            get
            {
                return elementName_;
            }
        }

        /// <summary>
        /// Returns the name of the attribute.
        /// </summary>
        /// <returns>the name of the attribute</returns>
        public String AttributeName
        {
            get
            {
                return attributeName_;
            }
        }

        /// <summary>
        /// Returns the value of the attribute.
        /// </summary>
        /// <returns>the value of the attribute</returns>
        public String AttributeValue
        {
            get
            {
                return attributeValue_;
            }
        }
    }
}
