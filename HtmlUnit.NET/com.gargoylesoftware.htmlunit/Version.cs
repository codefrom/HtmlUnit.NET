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
    /// Class to display version information about HtmlUnit. This is the class
    /// that will be executed if the JAR file is run.
    /// @version $Revision: 10115 $
    /// @author <a href="mailto:mbowler@GargoyleSoftware.com">Mike Bowler</a>
    /// @author Ahmed Ashour
    /// </summary>
    public sealed class Version
    {
        /// <summary>Prevent instantiation.</summary>
        private Version()
        {
            // Empty.
        }

        /// <summary>
        /// The main entry point into this class.
        /// @throws Exception if an error occurs
        /// </summary>
        /// <param name="args">the arguments passed on the command line</param>
        public static void Main(String[] args)
        {
            if (args.Length == 1 && String.Equals("-SanityCheck", args[0]))
            {
                RunSanityCheck();
                return;
            }
            Console.WriteLine(GetProductName());
            Console.WriteLine(GetCopyright());
            Console.WriteLine("Version: " + GetProductVersion());
        }

        /// <summary>
        /// Runs the sanity check.
        /// @throws Exception if anything goes wrong
        /// </summary>
        private static void RunSanityCheck()
        {
            try
            {
                WebClient webClient = new WebClient();
                HtmlPage page = webClient.getPage("http://htmlunit.sourceforge.net/index.html");
                page.executeJavaScript("document.location");
                Console.WriteLine("Sanity check complete.");
            }
            catch (Exception exc)
            {
                Console.WriteLine("Sanity check failed {0}.", exc);
            }
        }

        /// <summary>
        /// ProductName
        /// </summary>
        /// <returns>ProductName</returns>
        public static String GetProductName()
        {
            return "HtmlUnit";
        }

        /// <summary>
        /// Returns the current implementation version.
        /// </summary>
        /// <returns>the current implementation version.</returns>
        public static String GetProductVersion()
        {
            //return typeof(Version).getPackage().getImplementationVersion();
            return "1.0";
        }

        /// <summary>
        /// Returns the copyright notice.
        /// </summary>
        /// <returns>the copyright notice.</returns>
        public static String GetCopyright()
        {
            return "Copyright (c) 2002-2015 Gargoyle Software Inc. All rights reserved.";
        }
    }
}
