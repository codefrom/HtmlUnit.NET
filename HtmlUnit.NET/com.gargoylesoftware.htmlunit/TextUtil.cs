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
    /// Utility methods relating to text.
    /// @version $Revision: 9837 $
    /// @author <a href="mailto:mbowler@GargoyleSoftware.com">Mike Bowler</a>
    /// @author Brad Clarke
    /// @author Ahmed Ashour
    /// </summary>
    public sealed class TextUtil
    {
        /// <summary>Default encoding used.</summary>
        public const String DEFAULT_CHARSET = "ISO-8859-1";

        /// <summary>Private constructor to prevent instantiation.</summary>
        private TextUtil() { }

        /// <summary>
        /// Convert a string into an input stream.
        /// </summary>
        /// <param name="content">the string</param>
        /// <returns>the resulting input stream</returns>
        public static InputStream ToInputStream(String content)
        {
            try
            {
                return ToInputStream(content, DEFAULT_CHARSET);
            }
            catch (Exception e)
            {
                /*            throw new IllegalStateException(
                                DEFAULT_CHARSET + " is an unsupported encoding!  You may have a corrupted installation of java.");*/
                throw;
            }
        }

        /// <summary>
        /// Convert a string into an input stream.
        /// @throws UnsupportedEncodingException if the encoding is not supported
        /// </summary>
        /// <param name="content">the string</param>
        /// <param name="encoding">the encoding to use when converting the string to a stream</param>
        /// <returns>the resulting input stream</returns>
        public static InputStream ToInputStream(String content, String encoding)
        {
            try
            {
                ByteArrayOutputStream byteArrayOutputStream = new ByteArrayOutputStream(content.Length * 2);
                OutputStreamWriter writer = new OutputStreamWriter(byteArrayOutputStream, encoding);
                writer.write(content);
                writer.flush();

                byte[] byteArray = byteArrayOutputStream.toByteArray();
                return new ByteArrayInputStream(byteArray);
            }
            /*        catch (UnsupportedEncodingException e) {
                        throw e;
                    }*/
            catch (IOException e)
            {
                // Theoretically impossible since all the "IO" is in memory but it's a
                // checked exception so we have to catch it.
                throw new IllegalStateException("Exception when converting a string to an input stream: '" + e + "'", e);
            }
            catch (Exception e)
            {
                throw;
            }
        }

        /// <summary>
        /// Converts a string into a byte array using the specified encoding.
        /// </summary>
        /// <param name="content">the name of a supported charset</param>
        /// <param name="charset">the string to convert</param>
        /// <returns>the String as a byte[]; if the specified encoding is not supported an empty byte[] will be returned</returns>
        public static byte[] StringToByteArray(String content, String charset)
        {
            if (content == null || String.IsNullOrEmpty(content))
            {
                return new byte[0];
            }

            try
            {
                return Encoding.GetEncoding(charset).GetBytes(content);
            }
            catch (Exception e)
            {
                return new byte[0];
            }
            /*catch (UnsupportedEncodingException e) {
                return new byte[0];
            }*/
        }
    }
}
