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
    /// An interface that represents one window in a browser. It could be a top level window or a frame.
    /// @version $Revision: 10905 $
    /// @author <a href="mailto:mbowler@GargoyleSoftware.com">Mike Bowler</a>
    /// @author David K. Taylor
    /// @author David D. Kilzer
    /// </summary>
    [Serializable]
    public class WebWindow
    {
        /// <summary>
        /// The name of this window.
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// The currently loaded page or null if no page has been loaded.
        /// </summary>
        public IPage EnclosedPage { get; set; }

        /// <summary>
        /// Returns the window that contains this window. If this is a top
        /// level window, then return this window.
        /// </summary>
        public IWebWindow ParentWindow { get; }

        /// <summary>
        /// Returns the top level window that contains this window. If this
        /// is a top level window, then return this window.
        /// </summary>
        /// <returns></returns>
        public IWebWindow getTopWindow { get; }

        /// <summary>
        /// The web client that "owns" this window.
        /// </summary>
        public WebClient WebClient { get; }

        /// <summary>
        /// This window's navigation history.
        /// </summary>
        public History History { get; }

        /// <summary>
        /// <span style="color:red">INTERNAL API - SUBJECT TO CHANGE AT ANY TIME - USE AT YOUR OWN RISK.</span><br>
        /// The JavaScript object that corresponds to this element.
        /// </summary>
        public Object ScriptObject { get; set; }

        /// <summary>
        /// <span style="color:red">INTERNAL API - SUBJECT TO CHANGE AT ANY TIME - USE AT YOUR OWN RISK.</span><br>
        /// The job manager for this window.
        /// </summary>
        public JavaScriptJobManager JobManager { get; }

        /// <summary>
        /// Indicates if this window is closed. No action should be performed on a closed window.
        /// </summary>
        public bool IsClosed { get; }

        /// <summary>
        /// Returns the width (in pixels) of the browser window viewport including, if rendered, the vertical scrollbar.
        /// </summary>
        public int InnerWidth { get; set; }

        /// <summary>
        /// Returns the width of the outside of the browser window.
        /// It represents the width of the whole browser window including sidebar (if expanded),
        /// window chrome and window resizing borders/handles.
        /// </summary>
        public int OuterWidth { get; set; }

        /// <summary>
        /// The height (in pixels) of the browser window viewport including, if rendered, the horizontal scrollbar.
        /// </summary>
        public int InnerHeight { get; set; }

        /// <summary>
        /// Returns the height in pixels of the whole browser window.
        /// It represents the height of the whole browser window including sidebar (if expanded),
        /// window chrome and window resizing borders/handles.
        /// </summary>
        public int OuterHeight { get; set; }
    }
}
