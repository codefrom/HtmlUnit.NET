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
    /// A window representing a top level browser window.
    /// @version $Revision: 10772 $
    /// @author <a href="mailto:mbowler@GargoyleSoftware.com">Mike Bowler</a>
    /// @author David K. Taylor
    /// @author David D. Kilzer
    /// @author Ahmed Ashour
    /// @author Ronald Brill
    /// </summary>
    public class TopLevelWindow : WebWindowImpl
    {
        /// <summary>Logging support.</summary>
        private static Log LOG = LogFactory.GetLog(typeof(TopLevelWindow));

        /// <summary>The window which caused this window to be opened, if any.</summary>
        private IWebWindow opener_;

        /// <summary>
        /// Creates an instance.
        /// </summary>
        /// <param name="name">the name of the new window</param>
        /// <param name="webClient">the web client that "owns" this window</param>
        protected TopLevelWindow(String name, WebClient webClient) :
            base(webClient)
        {
            WebAssert.notNull("name", name);
            this.Name = name;
            PerformRegistration();
        }

        /// <summary>
        /// {@inheritDoc}
        /// Since this is a top level window, return this window.
        /// </summary>
        public override IWebWindow ParentWindow
        {
            get
            {
                return this;
            }
        }

        /// <summary>
        /// {@inheritDoc}
        /// Since this is a top level window, return this window.
        /// </summary>
        public IWebWindow TopWindow
        {
            get
            {
                return this;
            }
        }

        /// <summary>
        /// {@inheritDoc}
        /// </summary>
        /// <returns></returns>
        protected override bool IsJavaScriptInitializationNeeded()
        {
            IPage enclosedPage = EnclosedPage;
            return ScriptObject == null
                || enclosedPage.Url == WebClient.URL_ABOUT_BLANK
                || !(enclosedPage.WebResponse is StringWebResponse);
            // TODO: find a better way to distinguish content written by document.open(),...
        }

        /// <summary>
        /// Returns a string representation of this object.
        /// </summary>
        /// <returns>a string representation of this object.</returns>
        public override String ToString()
        {
            return "TopLevelWindow[name=\"" + Name + "\"]";
        }

        /// <summary>
        /// opener property. This is the WebWindow that caused this new window to be opened.
        /// </summary>
        public IWebWindow Opener
        {
            get
            {
                return opener_;
            }
            set
            {
                opener_ = value;
            }
        }

        /// <summary>
        /// Closes this window.
        /// </summary>
        public void Close()
        {
            IsClosed = true;
            IPage page = EnclosedPage;
            if (page != null)
            {
                if (page.IsHtmlPage())
                {
                    HtmlPage htmlPage = (HtmlPage)page;
                    if (!htmlPage.isOnbeforeunloadAccepted())
                    {
                        if (LOG.IsDebugEnabled)
                        {
                            LOG.Debug("The registered OnbeforeunloadHandler rejected the window close event.");
                        }
                        return;
                    }
                }
                page.CleanUp();
            }

            JobManager.Shutdown();
            DestroyChildren();
            WebClient.deregisterWebWindow(this);
        }
    }
}