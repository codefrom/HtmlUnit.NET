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
    /// A collection of constants that represent the various ways a form can be encoded when submitted.
    /// 
    /// @version $Revision: 9837 $
    /// @author Brad Clarke
    /// @author Ahmed Ashour
    /// </summary>
    [Serializable]
    public sealed class FormEncodingType
    {
        /// <summary>URL-encoded form encoding.</summary>
        public static readonly FormEncodingType URL_ENCODED = new FormEncodingType("application/x-www-form-urlencoded");

        /// <summary>Multipart form encoding (used to be a constant in HttpClient but it was deprecated with no alternative).</summary>
        public static readonly FormEncodingType MULTIPART = new FormEncodingType("multipart/form-data");

        private readonly String name_;

        private FormEncodingType(String name)
        {
            name_ = name;
        }

        /// <summary>
        /// Name of this encoding type.
        /// </summary>
        public String Name
        {
            get
            {
                return name_;
            }
        }

        /// <summary>
        /// Returns the constant that matches the specified name.
        /// </summary>
        /// <param name="name">the name to search by</param>
        /// <returns>the constant corresponding to the specified name, {@link #URL_ENCODED} if none match.</returns>
        public static FormEncodingType GetInstance(String name)
        {
            String lowerCaseName = name.ToLower(); // TODO : Locale.ENGLISH

            if (String.Equals(MULTIPART.Name, lowerCaseName))
            {
                return MULTIPART;
            }

            return URL_ENCODED;
        }

        /// <summary>
        /// Returns a string representation of this object.
        /// </summary>
        /// <returns>a string representation of this object</returns>
        public override String ToString()
        {
            return "EncodingType[name=" + Name + "]";
        }
    }
}