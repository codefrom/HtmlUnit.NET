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
using System.IO;
using System.Linq;
using System.Text;
using HtmlUnit.Helpers;

namespace HtmlUnit.com.gargoylesoftware.htmlunit
{
    /// <summary>
    /// Wrapper for content downloaded from a remote server.
    /// @version $Revision: 10772 $
    /// @author Marc Guillemot
    /// @author Ronald Brill
    /// </summary>
    public interface IDownloadedContent
    {
        // nested classes
        /// <summary>
        /// Implementation keeping content on the file system.
        /// </summary>
        public class OnFile : IDownloadedContent, IDisposable
        {
            private File file_;
            private bool temporary_;

            /// <summary>
            /// 
            /// </summary>
            /// <param name="file">the file</param>
            /// <param name="temporary">if true, the file will be deleted when cleanUp() is called.</param>
            public OnFile(File file, bool temporary)
            {
                file_ = file;
                temporary_ = temporary;
            }

            public InputStream GetInputStream()
            {
                return new FileInputStream(file_);
            }

            public void CleanUp()
            {
                if (temporary_)
                {
                    FileUtils.deleteQuietly(file_);
                }
            }

            public bool IsEmpty()
            {
                return false;
            }

            public void Dispose()
            {
                CleanUp();
            }
        }

        public class InMemory : IDownloadedContent
        {
            private byte[] bytes_;

            public InMemory(byte[] byteArray)
            {
                if (byteArray == null)
                {
                    bytes_ = ArrayUtils.EMPTY_BYTE_ARRAY;
                }
                else
                {
                    bytes_ = byteArray;
                }
            }

            public InputStream GetInputStream()
            {
                return new ByteArrayInputStream(bytes_);
            }

            public void CleanUp()
            {
                // nothing to do
            }

            public bool IsEmpty()
            {
                return bytes_.Length == 0;
            }
        }
        // --------------

        /// <summary>
        /// Returns a new {@link InputStream} allowing to read the downloaded content.
        /// @throws IOException in case of problem accessing the content
        /// </summary>
        /// <returns>the InputStream</returns>
        public InputStream GetInputStream();

        /// <summary>
        /// Clean up resources associated to this content.
        /// </summary>
        public void CleanUp();

        /// <summary>
        /// Returns true if the content is empty.
        /// </summary>
        /// <returns></returns>
        public bool IsEmpty();
    }
}