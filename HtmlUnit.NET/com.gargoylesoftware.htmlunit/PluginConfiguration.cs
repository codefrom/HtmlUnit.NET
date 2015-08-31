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
    /// Contains information about a plugin as available in JavaScript via <tt>document.navigator.plugins</tt>,
    /// as well as the associated mime types (for Firefox browser simulation).
    /// @version $Revision: 9871 $
    /// @author Marc Guillemot
    /// @see <a href="http://www.xulplanet.com/references/objref/Plugin.html">XUL Planet Documentation</a>
    /// </summary>
    [Serializable]
    public class PluginConfiguration : ICloneable
    {
        // nested classes
        /// <summary>Holds information about a single mime type associated with a plugin.</summary>
        [Serializable]
        public sealed class MimeType
        {
            private readonly String description_;
            private readonly String suffixes_;
            private readonly String type_;

            /// <summary>
            /// <summary>
            /// Creates a new instance.
            /// </summary>
            /// <param name="type">the mime type</param>
            /// <param name="description">the type description</param>
            /// <param name="suffixes">the file suffixes</param>
            public MimeType(String type, String description, String suffixes)
            {
                WebAssert.notNull("type", type);
                type_ = type;
                description_ = description;
                suffixes_ = suffixes;
            }

            /// <summary>
            /// the mime type's description.
            /// </summary>
            public String Description
            {
                get
                {
                    return description_;
                }
            }

            /// <summary>
            /// the mime type's suffixes.
            /// </summary>
            public String Suffixes
            {
                get
                {
                    return suffixes_;
                }
            }

            /// <summary>
            /// the mime type.
            /// </summary>
            public String Type
            {
                get
                {
                    return type_;
                }
            }

            /// <summary>
            /// {@inheritDoc}
            /// </summary>
            /// <returns></returns>
            public override int GetHashCode()
            {
                return type_.GetHashCode();
            }

            /// <summary>
            /// {@inheritDoc}
            /// </summary>
            /// <param name="o"></param>
            /// <returns></returns>
            public override bool Equals(Object o)
            {
                if (!(o is MimeType))
                {
                    return false;
                }
                MimeType other = (MimeType)o;
                return type_.Equals(other.type_);
            }
        }
        // --------------

        private readonly String description_;
        private readonly String filename_;
        private readonly String name_;
        private readonly List<MimeType> mimeTypes_ = new List<MimeType>();

        /**
         * Creates a new instance.
         * @param name the plugin name
         * @param description the plugin description
         * @param filename the plugin filename
         */
        public PluginConfiguration(String name, String description, String filename)
        {
            WebAssert.notNull("name", name);
            name_ = name;
            description_ = description;
            filename_ = filename;
        }

        /**
         * Gets the plugin's description.
         * @return the description
         */
        public String Description
        {
            get
            {
                return description_;
            }
        }

        /**
         * Gets the plugin's file name.
         * @return the file name
         */
        public String Filename
        {
            get
            {
                return filename_;
            }
        }

        /**
         * Gets the plugin's name.
         * @return the name
         */
        public String Name
        {
            get
            {
                return name_;
            }
        }

        /**
         * Gets the associated mime types.
         * @return a set of {@link MimeType}
         */
        public List<MimeType> MimeTypes
        {
            get
            {
                return mimeTypes_;
            }
        }

        /**
         * {@inheritDoc}
         */
        public override int GetHashCode()
        {
            return name_.GetHashCode();
        }

        /**
         * {@inheritDoc}
         */
        public override bool Equals(Object o)
        {
            if (!(o is PluginConfiguration))
            {
                return false;
            }
            PluginConfiguration other = (PluginConfiguration)o;
            return name_.Equals(other.name_);
        }

        /**
         * Creates and return a copy of this object. Current instance and cloned
         * object can be modified independently.
         * @return a clone of this instance.
         */
        public object Clone()
        {
            PluginConfiguration clone = new PluginConfiguration(Name, Description, Filename);
            clone.MimeTypes.AddRange(MimeTypes);
            return clone;
        }
    }
}