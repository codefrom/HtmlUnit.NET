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

namespace HtmlUnit.com.gargoylesoftware.htmlunit
{
    /// <summary>
    /// An abstract page that represents some content returned from a server.
    /// @version $Revision: 10875 $
    /// @author <a href="mailto:mbowler@GargoyleSoftware.com">Mike Bowler</a>
    /// @author David K. Taylor
    /// @author Marc Guillemot
    /// @author Ronald Brill
    /// </summary>
    public interface IPage {
        /// <summary>
        /// Initialize this page.
        /// This method gets called when a new page is loaded and you should probably never
        /// need to call it directly.
        /// @throws IOException if an IO problem occurs
        /// </summary>
        void Initialize();

        /// <summary>
        /// Clean up this page.
        /// This method gets called by the web client when an other page is loaded in the window
        /// and you should probably never need to call it directly
        /// </summary>
        void CleanUp();
    }
}