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
    /// <span style="color:red">INTERNAL API - SUBJECT TO CHANGE AT ANY TIME - USE AT YOUR OWN RISK.</span><br>
    /// 
    /// Base class for common WebWindow functionality. While public, this class is not
    /// exposed in any other places of the API. Internally we can cast to this class
    /// when we need access to functionality that is not present in {@link WebWindow}
    /// 
    /// @version $Revision: 10803 $
    /// @author Brad Clarke
    /// @author David K. Taylor
    /// @author Ahmed Ashour
    /// @author Ronald Brill
    /// </summary>
    public abstract class WebWindowImpl : IWebWindow
    {
        private static readonly Log LOG = LogFactory.GetLog(typeof(WebWindowImpl));

        private WebClient webClient_;
        private IPage enclosedPage_;
        private Object scriptObject_;
        private JavaScriptJobManager jobManager_;
        private List<WebWindowImpl> childWindows_ = new List<WebWindowImpl>();
        private String name_ = "";
        private History history_ = new History(this);
        private bool closed_;

        private int innerHeight_;
        private int outerHeight_;
        private int innerWidth_;
        private int outerWidth_;

        /// <summary>
        /// Creates a window and associates it with the client.
        /// </summary>
        /// <param name="webClient">the web client that "owns" this window</param>
        public WebWindowImpl(WebClient webClient)
        {
            WebAssert.notNull("webClient", webClient);
            webClient_ = webClient;
            jobManager_ = BackgroundJavaScriptFactory.theFactory().createJavaScriptJobManager(this);

            bool plus16 = false;
            innerHeight_ = 605;
            if (webClient.getBrowserVersion().hasFeature(JS_WINDOW_OUTER_INNER_HEIGHT_DIFF_63))
            {
                outerHeight_ = innerHeight_ + 63;
                plus16 = true;
            }
            else if (webClient.getBrowserVersion().hasFeature(JS_WINDOW_OUTER_INNER_HEIGHT_DIFF_94))
            {
                outerHeight_ = innerHeight_ + 94;
            }
            else if (webClient.getBrowserVersion().hasFeature(JS_WINDOW_OUTER_INNER_HEIGHT_DIFF_89))
            {
                outerHeight_ = innerHeight_ + 89;
                plus16 = true;
            }
            else
            {
                outerHeight_ = innerHeight_ + 115;
            }
            innerWidth_ = 1256;
            if (plus16)
            {
                outerWidth_ = innerWidth_ + 16;
            }
            else
            {
                outerWidth_ = innerWidth_ + 14;
            }
        }

        /// <summary>Registers the window with the client.</summary>
        protected void PerformRegistration()
        {
            webClient_.registerWebWindow(this);
        }

        public string Name
        {
            get
            {
                return name_;
            }
            set
            {
                name_ = value;
            }
        }

        /// <summary>{@inheritDoc}</summary>
        public IPage EnclosedPage
        {
            get
            {
                return enclosedPage_;
            }
            set
            {
                if (LOG.IsDebugEnabled)
                {
                    LOG.Debug("setEnclosedPage: " + value);
                }
                if (value == enclosedPage_)
                {
                    return;
                }
                DestroyChildren();
                enclosedPage_ = value;
                history_.addPage(value);
                if (IsJavaScriptInitializationNeeded())
                {
                    webClient_.initialize(this);
                }
                webClient_.initialize(value);
            }
        }

        public IWebWindow ParentWindow
        {
            get { throw new NotImplementedException(); }
        }

        public IWebWindow TopWindow
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>{@inheritDoc}</summary>
        public WebClient WebClient
        {
            get { return webClient_; }
        }

        public History History
        {
            get { return history_; }
        }

        public object ScriptObject
        {
            get
            {
                return scriptObject_;
            }
            set
            {
                scriptObject_ = value;
            }
        }

        public JavaScriptJobManager JobManager
        {
            get { return jobManager_; }
            set { jobManager_ = value; }
        }

        public bool IsClosed
        {
            get { return closed_; }
            set { closed_ = value; }
        }

        public int InnerWidth
        {
            get
            {
                return innerWidth_;
            }
            set
            {
                innerWidth_ = value;
            }
        }

        public int OuterWidth
        {
            get
            {
                return outerWidth_;
            }
            set
            {
                outerWidth_ = value;
            }
        }

        public int InnerHeight
        {
            get
            {
                return innerHeight_;
            }
            set
            {
                innerHeight_ = value;
            }
        }

        public int OuterHeight
        {
            get
            {
                return outerHeight_;
            }
            set
            {
                outerHeight_ = value;
            }
        }

        /// <summary>
        /// Returns <tt>true</tt> if this window needs JavaScript initialization to occur when the enclosed page is set.
        /// </summary>
        /// <returns><tt>true</tt> if this window needs JavaScript initialization to occur when the enclosed page is set</returns>
        protected abstract bool IsJavaScriptInitializationNeeded();

        /// <summary>
        /// <p><span style="color:red">INTERNAL API - SUBJECT TO CHANGE AT ANY TIME - USE AT YOUR OWN RISK.</span></p>
        /// <p>Adds a child to this window, for shutdown purposes.</p>
        /// </summary>
        /// <param name="child">the child window to associate with this window</param>
        public void AddChildWindow(FrameWindow child)
        {
            lock (childWindows_)
            {
                childWindows_.Add(child);
            }
        }

        /// <summary>Destroy our childs.</summary>
        protected void DestroyChildren()
        {
            if (LOG.IsDebugEnabled)
            {
                LOG.Debug("destroyChildren");
            }
            JobManager.RemoveAllJobs();

            // try to deal with js thread adding a new window in between
            while (childWindows_.Count != 0)
            {
                WebWindowImpl window = childWindows_[0];
                RemoveChildWindow(window);
            }
        }

        /// <summary>
        /// <p><span style="color:red">INTERNAL API - SUBJECT TO CHANGE AT ANY TIME - USE AT YOUR OWN RISK.</span></p>
        /// Destroy the child window.
        /// </summary>
        /// <param name="window">the child to destroy</param>
        public void RemoveChildWindow(WebWindowImpl window)
        {
            if (LOG.IsDebugEnabled)
            {
                LOG.Debug("closing child window: " + window);
            }
            window.IsClosed = true;
            window.JobManager.Shutdown();
            IPage page = window.EnclosedPage;
            if (page != null)
            {
                page.CleanUp();
            }
            window.DestroyChildren();

            lock (childWindows_)
            {
                childWindows_.Remove(window);
            }
        }
    }
}