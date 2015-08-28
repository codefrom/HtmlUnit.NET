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
    /// An exception that is thrown when the server returns a failing status code.
    /// @version $Revision: 9837 $
    /// @author <a href="mailto:mbowler@GargoyleSoftware.com">Mike Bowler</a>
    /// @author Marc Guillemot
    /// </summary>
    public class FailingHttpStatusCodeException : SystemException
    {
        private readonly WebResponse response_;

        /**
         * Creates an instance.
         * @param failingResponse the failing response
         */
        public FailingHttpStatusCodeException(WebResponse failingResponse) :
            this(BuildMessage(failingResponse), failingResponse)
        {
        }

        /// <summary>
        /// Creates an instance.
        /// </summary>
        /// <param name="message">the message</param>
        /// <param name="failingResponse">the failing response</param>
        public FailingHttpStatusCodeException(String message, WebResponse failingResponse) :
            base(message)
        {
            response_ = failingResponse;
        }


        /// <summary>
        /// The failing status code.
        /// </summary>
        public int StatusCode
        {
            get
            {
                return response_.StatusCode;
            }
        }


        /// <summary>
        /// The message associated with the failing status code.
        /// </summary>
        public String StatusMessage
        {
            get
            {
                return response_.StatusMessage;
            }
        }

        private static String BuildMessage(WebResponse failingResponse)
        {
            int code = failingResponse.StatusCode;
            String msg = failingResponse.StatusMessage;
            URL url = failingResponse.WebRequest.Url;
            return code + " " + msg + " for " + url;
        }

        /// <summary>
        /// The failing response.
        /// </summary>
        public WebResponse Response
        {
            get
            {
                return response_;
            }
        }
    }
}
