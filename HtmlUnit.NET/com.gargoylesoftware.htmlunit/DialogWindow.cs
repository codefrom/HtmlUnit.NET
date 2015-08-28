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
    public class DialogWindow : WebWindowImpl
    {
        /// <summary>The arguments object exposed via the <tt>dialogArguments</tt> JavaScript property.</summary>
        private Object arguments_;

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="webClient">the web client that "owns" this window</param>
        /// <param name="arguments">the arguments object exposed via the <tt>dialogArguments</tt> JavaScript property</param>
        protected DialogWindow(WebClient webClient, Object arguments) :
            base(webClient)
        {
            arguments_ = arguments;
            PerformRegistration();
        }

        /// <summary>
        /// {@inheritDoc}
        /// </summary>
        /// <returns></returns>
        protected override bool IsJavaScriptInitializationNeeded()
        {
            return ScriptObject == null;
        }

        /// <summary>
        /// {@inheritDoc}
        /// </summary>
        /// <returns></returns>
        public override IWebWindow ParentWindow
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
        public override IWebWindow TopWindow
        {
            get
            {
                return this;
            }
        }

        /// <summary>
        /// {@inheritDoc}
        /// </summary>
        /// <param name="Object"></param>
        public Object ScriptObject
        {
            get 
            {
                return base.ScriptObject;
            }
            set
            {
                ScriptableObject so = (ScriptableObject)value;
                if (so != null)
                {
                    so.put("dialogArguments", so, arguments_);
                }
                base.ScriptObject = value;
            }
        }

        /// <summary>Closes this window.</summary>
        public void Close()
        {
            JobManager.Shutdown();
            DestroyChildren();
            WebClient.DeregisterWebWindow(this);
        }

        /// <summary>
        /// a string representation of this object
        /// </summary>
        /// <returns>a string representation of this object</returns>
        public override String ToString()
        {
            return "DialogWindow[name=\"" + Name + "\"]";
        }
    }
}