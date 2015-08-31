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
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Xml.Serialization;
using HtmlUnit.Helpers;

namespace HtmlUnit.com.gargoylesoftware.htmlunit
{
    /// <summary>
    /// This {@link AjaxController} resynchronizes calls calling from the main thread.
    /// The idea is that asynchronous AJAX calls performed directly in response to a user
    /// action (therefore in the "main" thread and not in the thread of a background task)
    /// are directly useful for the user. To easily have a testable state, these calls
    /// are performed synchronously.
    /// @version $Revision: 10904 $
    /// @author Marc Guillemot
    /// </summary>
    [Serializable]
    public class NicelyResynchronizingAjaxController : AjaxController
    {
        private static readonly Log LOG = LogFactory.GetLog(typeof(NicelyResynchronizingAjaxController));

        [NonSerialized]
        private WeakReference originatedThread_;

        /// <summary>Creates an instance.</summary>
        public NicelyResynchronizingAjaxController()
        {
            Init();
        }

        /// <summary>Initializes this instance.</summary>
        private void Init()
        {
            originatedThread_ = new WeakReference(Thread.CurrentThread);
        }

        /// <summary>
        /// Resynchronizes calls performed from the thread where this instance has been created.
        /// </summary>
        /// <param name="page"></param>
        /// <param name="settings"></param>
        /// <param name="async"></param>
        /// <returns></returns>
        public override bool ProcessSynchron(HtmlPage page, WebRequest settings, bool async)
        {
            if (async && IsInOriginalThread())
            {
                LOG.Info("Re-synchronized call to " + settings.Url);
                return true;
            }
            return !async;
        }

        /// <summary>
        /// Indicates if the currently executing thread is the one in which this instance has been created.
        /// </summary>
        /// <returns>{@code true} if it's the same thread</returns>
        bool IsInOriginalThread()
        {
            return Thread.CurrentThread == originatedThread_.Target;
        }

        /// <summary>
        /// Custom deserialization logic.
        /// @throws IOException if an IO error occurs
        /// @throws ClassNotFoundException if a class cannot be found
        /// </summary>
        /// <param name="stream">the stream from which to read the object</param>
        /// <returns></returns>
        private NicelyResynchronizingAjaxController ReadObject(Stream stream)
        {
            BinaryFormatter binSer = new BinaryFormatter();
            NicelyResynchronizingAjaxController ser = (NicelyResynchronizingAjaxController)binSer.Deserialize(stream);
            ser.Init();
            return ser;
        }
    }
}