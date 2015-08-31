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
using HtmlUnit.Helpers;

namespace HtmlUnit.com.gargoylesoftware.htmlunit
{
    /// <summary>
    /// Holder for different types of storages.
    /// <p><span style="color:red">Experimental API: May be changed in next release!</span></p>
    /// @version $Revision: 10103 $
    /// @author Ahmed Ashour
    /// @author Ronald Brill
    /// @author Marc Guillemot
    /// </summary>
    [Serializable]
    public class StorageHolder
    {
        private Dictionary<String, Dictionary<String, String>> globalStorage_ = new Dictionary<String, Dictionary<String, String>>();

        private Dictionary<String, Dictionary<String, String>> localStorage_ = new Dictionary<String, Dictionary<String, String>>();

        [NonSerialized]
        private Dictionary<String, Dictionary<String, String>> sessionStorage_ = new Dictionary<String, Dictionary<String, String>>();

        /// <summary>
        /// Gets the store of the give type for the page.
        /// </summary>
        /// <param name="storageType">the type</param>
        /// <param name="page">the page</param>
        /// <returns>the store</returns>
        public Dictionary<String, String> GetStore(StorageType storageType, IPage page)
        {
            String key = GetKey(storageType, page);
            Dictionary<String, Dictionary<String, String>> storage = GetStorage(storageType);

            lock (storage)
            {
                Dictionary<String, String> map = storage[key];
                if (map == null)
                {
                    map = new Dictionary<String, String>();
                    storage.Add(key, map);
                }
                return map;
            }
        }

        private String GetKey(StorageType type, IPage page)
        {
            switch (type)
            {
                case StorageType.GLOBAL_STORAGE:
                    return page.Url.Host;

                case StorageType.LOCAL_STORAGE:
                    URL url = page.Url;
                    return url.Protocol + "://" + url.Host + ':'
                            + url.Protocol;

                case StorageType.SESSION_STORAGE:
                    IWebWindow topWindow = page.EnclosingWindow.TopWindow;
                    return topWindow.GetHashCode().ToString("X");

                default:
                    return null;
            }
        }

        private Dictionary<String, Dictionary<String, String>> GetStorage(StorageType type)
        {
            switch (type)
            {
                case StorageType.GLOBAL_STORAGE:
                    return globalStorage_;

                case StorageType.LOCAL_STORAGE:
                    return localStorage_;

                case StorageType.SESSION_STORAGE:
                    return sessionStorage_;

                default:
                    return null;
            }
        }
    }
}