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

namespace HtmlUnit.com.gargoylesoftware.htmlunit.util
{
    /// <summary>
    /// Sniffs encoding settings from HTML, XML or other content. The HTML encoding sniffing algorithm is based on the
    /// <a href="http://www.whatwg.org/specs/web-apps/current-work/multipage/parsing.html#determining-the-character-encoding">HTML5
    /// encoding sniffing algorithm</a>.
    /// @version $Revision: 10768 $
    /// @author Daniel Gredler
    /// @author Ahmed Ashour
    /// @author Ronald Brill
    /// </summary>
    public sealed class EncodingSniffer
    {
    /// <summary> Logging support. </summary>
    private static readonly Log LOG = LogFactory.GetLog(typeof(EncodingSniffer));

    /// <summary>UTF-16 (little endian) charset name.</summary>
    static const String UTF16_LE = "UTF-16LE";

    /// <summary>UTF-16 (big endian) charset name.</summary>
    static const String UTF16_BE = "UTF-16BE";

    /// <summary>UTF-8 charset name.</summary>
    static const String UTF8 = "UTF-8";

    /// <summary>Sequence(s) of bytes indicating the beginning of a comment.</summary>
    private static const byte[][] COMMENT_START = new byte[][] {
        new byte[] {Convert.ToByte('<')},
        new byte[] {Convert.ToByte('!')},
        new byte[] {Convert.ToByte('-')},
        new byte[] {Convert.ToByte('-')}
    };

    /// <summary>Sequence(s) of bytes indicating the beginning of a <tt>meta</tt> HTML tag.</summary>
    private static const byte[][] META_START = new byte[][] {
        new byte[] {Convert.ToByte('<')},
        new byte[] {Convert.ToByte('m'), Convert.ToByte('M')},
        new byte[] {Convert.ToByte('e'), Convert.ToByte('E')},
        new byte[] {Convert.ToByte('t'), Convert.ToByte('T')},
        new byte[] {Convert.ToByte('a'), Convert.ToByte('A')},
        new byte[] {0x09, 0x0A, 0x0C, 0x0D, 0x20, 0x2F}
    };

    /// <summary>Sequence(s) of bytes indicating the beginning of miscellaneous HTML content.</summary>
    private static const byte[][] OTHER_START = new byte[][] {
        new byte[] {Convert.ToByte('<')},
        new byte[] {Convert.ToByte('!'), Convert.ToByte('/'), Convert.ToByte('?')}
    };

    /// <summary>Sequence(s) of bytes indicating the beginning of a charset specification.</summary>
    private static const byte[][] CHARSET_START = new byte[][] {
        new byte[] {Convert.ToByte('c'), Convert.ToByte('C')},
        new byte[] {Convert.ToByte('h'), Convert.ToByte('H')},
        new byte[] {Convert.ToByte('a'), Convert.ToByte('A')},
        new byte[] {Convert.ToByte('r'), Convert.ToByte('R')},
        new byte[] {Convert.ToByte('s'), Convert.ToByte('S')},
        new byte[] {Convert.ToByte('e'), Convert.ToByte('E')},
        new byte[] {Convert.ToByte('t'), Convert.ToByte('T')}
    };

    /// <summary><a href="http://encoding.spec.whatwg.org/#encodings">Reference</a></summary>
    private static readonly Dictionary<String, String> ENCODING_FROM_LABEL;
    
    static EncodingSniffer() 
    {
        ENCODING_FROM_LABEL = new Dictionary<String, String>();

        // The Encoding
        // ------------
        ENCODING_FROM_LABEL.Add("unicode-1-1-utf-8", "utf-8");
        ENCODING_FROM_LABEL.Add("utf-8", "utf-8");
        ENCODING_FROM_LABEL.Add("utf8", "utf-8");

        // Legacy single-byte encodings
        // ----------------------------

        // ibm866
        ENCODING_FROM_LABEL.Add("866", "ibm866");
        ENCODING_FROM_LABEL.Add("cp866", "ibm866");
        ENCODING_FROM_LABEL.Add("csibm866", "ibm866");
        ENCODING_FROM_LABEL.Add("ibm866", "ibm866");

        // iso-8859-2
        ENCODING_FROM_LABEL.Add("csisolatin2", "iso-8859-2");
        ENCODING_FROM_LABEL.Add("iso-8859-2", "iso-8859-2");
        ENCODING_FROM_LABEL.Add("iso-ir-101", "iso-8859-2");
        ENCODING_FROM_LABEL.Add("iso8859-2", "iso-8859-2");
        ENCODING_FROM_LABEL.Add("iso88592", "iso-8859-2");
        ENCODING_FROM_LABEL.Add("iso_8859-2", "iso-8859-2");
        ENCODING_FROM_LABEL.Add("iso_8859-2:1987", "iso-8859-2");
        ENCODING_FROM_LABEL.Add("l2", "iso-8859-2");
        ENCODING_FROM_LABEL.Add("latin2", "iso-8859-2");

        // iso-8859-3
        ENCODING_FROM_LABEL.Add("csisolatin2", "iso-8859-3");
        ENCODING_FROM_LABEL.Add("csisolatin3", "iso-8859-3");
        ENCODING_FROM_LABEL.Add("iso-8859-3", "iso-8859-3");
        ENCODING_FROM_LABEL.Add("iso-ir-109", "iso-8859-3");
        ENCODING_FROM_LABEL.Add("iso8859-3", "iso-8859-3");
        ENCODING_FROM_LABEL.Add("iso88593", "iso-8859-3");
        ENCODING_FROM_LABEL.Add("iso_8859-3", "iso-8859-3");
        ENCODING_FROM_LABEL.Add("iso_8859-3:1988", "iso-8859-3");
        ENCODING_FROM_LABEL.Add("l3", "iso-8859-3");
        ENCODING_FROM_LABEL.Add("latin3", "iso-8859-3");

        // iso-8859-4
        ENCODING_FROM_LABEL.Add("csisolatin4", "iso-8859-4");
        ENCODING_FROM_LABEL.Add("iso-8859-4", "iso-8859-4");
        ENCODING_FROM_LABEL.Add("iso-ir-110", "iso-8859-4");
        ENCODING_FROM_LABEL.Add("iso8859-4", "iso-8859-4");
        ENCODING_FROM_LABEL.Add("iso88594", "iso-8859-4");
        ENCODING_FROM_LABEL.Add("iso_8859-4", "iso-8859-4");
        ENCODING_FROM_LABEL.Add("iso_8859-4:1988", "iso-8859-4");
        ENCODING_FROM_LABEL.Add("l4", "iso-8859-4");
        ENCODING_FROM_LABEL.Add("latin4", "iso-8859-4");

        // iso-8859-5
        ENCODING_FROM_LABEL.Add("csisolatincyrillic", "iso-8859-5");
        ENCODING_FROM_LABEL.Add("csisolatincyrillic", "iso-8859-5");
        ENCODING_FROM_LABEL.Add("cyrillic", "iso-8859-5");
        ENCODING_FROM_LABEL.Add("iso-8859-5", "iso-8859-5");
        ENCODING_FROM_LABEL.Add("iso-ir-144", "iso-8859-5");
        ENCODING_FROM_LABEL.Add("iso8859-5", "iso-8859-5");
        ENCODING_FROM_LABEL.Add("iso88595", "iso-8859-5");
        ENCODING_FROM_LABEL.Add("iso_8859-5", "iso-8859-5");
        ENCODING_FROM_LABEL.Add("iso_8859-5:1988", "iso-8859-5");

        // iso-8859-6
        ENCODING_FROM_LABEL.Add("arabic", "iso-8859-6");
        ENCODING_FROM_LABEL.Add("asmo-708", "iso-8859-6");
        ENCODING_FROM_LABEL.Add("csiso88596e", "iso-8859-6");
        ENCODING_FROM_LABEL.Add("csiso88596i", "iso-8859-6");
        ENCODING_FROM_LABEL.Add("csisolatinarabic", "iso-8859-6");
        ENCODING_FROM_LABEL.Add("ecma-114", "iso-8859-6");
        ENCODING_FROM_LABEL.Add("iso-8859-6", "iso-8859-6");
        ENCODING_FROM_LABEL.Add("iso-8859-6-e", "iso-8859-6");
        ENCODING_FROM_LABEL.Add("iso-8859-6-i", "iso-8859-6");
        ENCODING_FROM_LABEL.Add("iso-ir-127", "iso-8859-6");
        ENCODING_FROM_LABEL.Add("iso8859-6", "iso-8859-6");
        ENCODING_FROM_LABEL.Add("iso88596", "iso-8859-6");
        ENCODING_FROM_LABEL.Add("iso_8859-6", "iso-8859-6");
        ENCODING_FROM_LABEL.Add("iso_8859-6:1987", "iso-8859-6");

        // iso-8859-7
        ENCODING_FROM_LABEL.Add("csisolatingreek", "iso-8859-7");
        ENCODING_FROM_LABEL.Add("ecma-118", "iso-8859-7");
        ENCODING_FROM_LABEL.Add("elot_928", "iso-8859-7");
        ENCODING_FROM_LABEL.Add("greek", "iso-8859-7");
        ENCODING_FROM_LABEL.Add("greek8", "iso-8859-7");
        ENCODING_FROM_LABEL.Add("iso-8859-7", "iso-8859-7");
        ENCODING_FROM_LABEL.Add("iso-ir-126", "iso-8859-7");
        ENCODING_FROM_LABEL.Add("iso8859-7", "iso-8859-7");
        ENCODING_FROM_LABEL.Add("iso88597", "iso-8859-7");
        ENCODING_FROM_LABEL.Add("iso_8859-7", "iso-8859-7");
        ENCODING_FROM_LABEL.Add("iso_8859-7:1987", "iso-8859-7");
        ENCODING_FROM_LABEL.Add("sun_eu_greek", "iso-8859-7");

        // iso-8859-8
        ENCODING_FROM_LABEL.Add("csisolatingreek", "iso-8859-8");
        ENCODING_FROM_LABEL.Add("csiso88598e", "iso-8859-8");
        ENCODING_FROM_LABEL.Add("csisolatinhebrew", "iso-8859-8");
        ENCODING_FROM_LABEL.Add("hebrew", "iso-8859-8");
        ENCODING_FROM_LABEL.Add("iso-8859-8", "iso-8859-8");
        ENCODING_FROM_LABEL.Add("iso-8859-8-e", "iso-8859-8");
        ENCODING_FROM_LABEL.Add("iso-ir-138", "iso-8859-8");
        ENCODING_FROM_LABEL.Add("iso8859-8", "iso-8859-8");
        ENCODING_FROM_LABEL.Add("iso88598", "iso-8859-8");
        ENCODING_FROM_LABEL.Add("iso_8859-8", "iso-8859-8");
        ENCODING_FROM_LABEL.Add("iso_8859-8:1988", "iso-8859-8");
        ENCODING_FROM_LABEL.Add("visual", "iso-8859-8");

        // iso-8859-8-i
        ENCODING_FROM_LABEL.Add("csiso88598i", "iso-8859-8-i");
        ENCODING_FROM_LABEL.Add("iso-8859-8-i", "iso-8859-8-i");
        ENCODING_FROM_LABEL.Add("logical", "iso-8859-8-i");

        // iso-8859-10
        ENCODING_FROM_LABEL.Add("csisolatin6", "iso-8859-10");
        ENCODING_FROM_LABEL.Add("iso-8859-10", "iso-8859-10");
        ENCODING_FROM_LABEL.Add("iso-ir-157", "iso-8859-10");
        ENCODING_FROM_LABEL.Add("iso8859-10", "iso-8859-10");
        ENCODING_FROM_LABEL.Add("iso885910", "iso-8859-10");
        ENCODING_FROM_LABEL.Add("l6", "iso-8859-10");
        ENCODING_FROM_LABEL.Add("latin6", "iso-8859-10");

        // iso-8859-13
        ENCODING_FROM_LABEL.Add("iso-8859-13", "iso-8859-13");
        ENCODING_FROM_LABEL.Add("iso8859-13", "iso-8859-13");
        ENCODING_FROM_LABEL.Add("iso885913", "iso-8859-13");

        // iso-8859-14
        ENCODING_FROM_LABEL.Add("iso-8859-14", "iso-8859-14");
        ENCODING_FROM_LABEL.Add("iso8859-14", "iso-8859-14");
        ENCODING_FROM_LABEL.Add("iso885914", "iso-8859-14");

        // iso-8859-15
        ENCODING_FROM_LABEL.Add("csisolatin9", "iso-8859-15");
        ENCODING_FROM_LABEL.Add("iso-8859-15", "iso-8859-15");
        ENCODING_FROM_LABEL.Add("iso8859-15", "iso-8859-15");
        ENCODING_FROM_LABEL.Add("iso885915", "iso-8859-15");
        ENCODING_FROM_LABEL.Add("iso_8859-15", "iso-8859-15");
        ENCODING_FROM_LABEL.Add("l9", "iso-8859-15");

        // iso-8859-16
        ENCODING_FROM_LABEL.Add("iso-8859-16", "iso-8859-16");

        // koi8-r
        ENCODING_FROM_LABEL.Add("cskoi8r", "koi8-r");
        ENCODING_FROM_LABEL.Add("koi", "koi8-r");
        ENCODING_FROM_LABEL.Add("koi8", "koi8-r");
        ENCODING_FROM_LABEL.Add("koi8-r", "koi8-r");
        ENCODING_FROM_LABEL.Add("koi8_r", "koi8-r");

        // koi8-u
        ENCODING_FROM_LABEL.Add("koi8-u", "koi8-u");

        // macintosh
        ENCODING_FROM_LABEL.Add("csmacintosh", "macintosh");
        ENCODING_FROM_LABEL.Add("mac", "macintosh");
        ENCODING_FROM_LABEL.Add("macintosh", "macintosh");
        ENCODING_FROM_LABEL.Add("x-mac-roman", "macintosh");

        // windows-874
        ENCODING_FROM_LABEL.Add("dos-874", "windows-874");
        ENCODING_FROM_LABEL.Add("iso-8859-11", "windows-874");
        ENCODING_FROM_LABEL.Add("iso8859-11", "windows-874");
        ENCODING_FROM_LABEL.Add("iso885911", "windows-874");
        ENCODING_FROM_LABEL.Add("tis-620", "windows-874");
        ENCODING_FROM_LABEL.Add("windows-874", "windows-874");

        // windows-1250
        ENCODING_FROM_LABEL.Add("cp1250", "windows-1250");
        ENCODING_FROM_LABEL.Add("windows-1250", "windows-1250");
        ENCODING_FROM_LABEL.Add("x-cp1250", "windows-1250");

        // windows-1251
        ENCODING_FROM_LABEL.Add("cp1251", "windows-1251");
        ENCODING_FROM_LABEL.Add("windows-1251", "windows-1251");
        ENCODING_FROM_LABEL.Add("x-cp1251", "windows-1251");

        // windows-1252
        ENCODING_FROM_LABEL.Add("ansi_x3.4-1968", "windows-1252");
        ENCODING_FROM_LABEL.Add("ascii", "windows-1252");
        ENCODING_FROM_LABEL.Add("cp1252", "windows-1252");
        ENCODING_FROM_LABEL.Add("cp819", "windows-1252");
        ENCODING_FROM_LABEL.Add("csisolatin1", "windows-1252");
        ENCODING_FROM_LABEL.Add("ibm819", "windows-1252");
        ENCODING_FROM_LABEL.Add("iso-8859-1", "windows-1252");
        ENCODING_FROM_LABEL.Add("iso-ir-100", "windows-1252");
        ENCODING_FROM_LABEL.Add("iso8859-1", "windows-1252");
        ENCODING_FROM_LABEL.Add("iso88591", "windows-1252");
        ENCODING_FROM_LABEL.Add("iso_8859-1", "windows-1252");
        ENCODING_FROM_LABEL.Add("iso_8859-1:1987", "windows-1252");
        ENCODING_FROM_LABEL.Add("l1", "windows-1252");
        ENCODING_FROM_LABEL.Add("latin1", "windows-1252");
        ENCODING_FROM_LABEL.Add("us-ascii", "windows-1252");
        ENCODING_FROM_LABEL.Add("windows-1252", "windows-1252");
        ENCODING_FROM_LABEL.Add("x-cp1252", "windows-1252");

        // windows-1253
        ENCODING_FROM_LABEL.Add("cp1253", "windows-1253");
        ENCODING_FROM_LABEL.Add("windows-1253", "windows-1253");
        ENCODING_FROM_LABEL.Add("x-cp1253", "windows-1253");

        // windows-1254
        ENCODING_FROM_LABEL.Add("cp1254", "windows-1254");
        ENCODING_FROM_LABEL.Add("csisolatin5", "windows-1254");
        ENCODING_FROM_LABEL.Add("iso-8859-9", "windows-1254");
        ENCODING_FROM_LABEL.Add("iso-ir-148", "windows-1254");
        ENCODING_FROM_LABEL.Add("iso8859-9", "windows-1254");
        ENCODING_FROM_LABEL.Add("iso88599", "windows-1254");
        ENCODING_FROM_LABEL.Add("iso_8859-9", "windows-1254");
        ENCODING_FROM_LABEL.Add("iso_8859-9:1989", "windows-1254");
        ENCODING_FROM_LABEL.Add("l5", "windows-1254");
        ENCODING_FROM_LABEL.Add("latin5", "windows-1254");
        ENCODING_FROM_LABEL.Add("windows-1254", "windows-1254");
        ENCODING_FROM_LABEL.Add("x-cp1254", "windows-1254");

        // windows-1255
        ENCODING_FROM_LABEL.Add("cp1255", "windows-1255");
        ENCODING_FROM_LABEL.Add("windows-1255", "windows-1255");
        ENCODING_FROM_LABEL.Add("x-cp1255", "windows-1255");

        // windows-1256
        ENCODING_FROM_LABEL.Add("cp1256", "windows-1256");
        ENCODING_FROM_LABEL.Add("windows-1256", "windows-1256");
        ENCODING_FROM_LABEL.Add("x-cp1256", "windows-1256");

        // windows-1257
        ENCODING_FROM_LABEL.Add("cp1257", "windows-1257");
        ENCODING_FROM_LABEL.Add("windows-1257", "windows-1257");
        ENCODING_FROM_LABEL.Add("x-cp1257", "windows-1257");

        // windows-1258
        ENCODING_FROM_LABEL.Add("cp1258", "windows-1258");
        ENCODING_FROM_LABEL.Add("windows-1258", "windows-1258");
        ENCODING_FROM_LABEL.Add("x-cp1258", "windows-1258");

        // windows-1258
        ENCODING_FROM_LABEL.Add("x-mac-cyrillic", "x-mac-cyrillic");
        ENCODING_FROM_LABEL.Add("x-mac-ukrainian", "x-mac-cyrillic");

        // Legacy multi-byte Chinese (simplified) encodings
        // ------------------------------------------------

        // gb18030
        ENCODING_FROM_LABEL.Add("chinese", "gb18030");
        ENCODING_FROM_LABEL.Add("csgb2312", "gb18030");
        ENCODING_FROM_LABEL.Add("csiso58gb231280", "gb18030");
        ENCODING_FROM_LABEL.Add("gb18030", "gb18030");
        ENCODING_FROM_LABEL.Add("gb2312", "gb18030");
        ENCODING_FROM_LABEL.Add("gb_2312", "gb18030");
        ENCODING_FROM_LABEL.Add("gb_2312-80", "gb18030");
        ENCODING_FROM_LABEL.Add("gbk", "gb18030");
        ENCODING_FROM_LABEL.Add("iso-ir-58", "gb18030");
        ENCODING_FROM_LABEL.Add("x-gbk", "gb18030");

        // gb18030
        ENCODING_FROM_LABEL.Add("hz-gb-2312", "hz-gb-2312");

        // Legacy multi-byte Chinese (traditional) encodings
        // ------------------------------------------------

        // big5
        ENCODING_FROM_LABEL.Add("big5", "big5");
        ENCODING_FROM_LABEL.Add("big5-hkscs", "big5");
        ENCODING_FROM_LABEL.Add("cn-big5", "big5");
        ENCODING_FROM_LABEL.Add("csbig5", "big5");
        ENCODING_FROM_LABEL.Add("x-x-big5", "big5");

        // Legacy multi-byte Japanese encodings
        // ------------------------------------

        // euc-jp
        ENCODING_FROM_LABEL.Add("cseucpkdfmtjapanese", "euc-jp");
        ENCODING_FROM_LABEL.Add("euc-jp", "euc-jp");
        ENCODING_FROM_LABEL.Add("x-euc-jp", "euc-jp");

        // iso-2022-jp
        ENCODING_FROM_LABEL.Add("csiso2022jp", "iso-2022-jp");
        ENCODING_FROM_LABEL.Add("iso-2022-jp", "iso-2022-jp");

        // iso-2022-jp
        ENCODING_FROM_LABEL.Add("csshiftjis", "shift_jis");
        ENCODING_FROM_LABEL.Add("ms_kanji", "shift_jis");
        ENCODING_FROM_LABEL.Add("shift-jis", "shift_jis");
        ENCODING_FROM_LABEL.Add("shift_jis", "shift_jis");
        ENCODING_FROM_LABEL.Add("sjis", "shift_jis");
        ENCODING_FROM_LABEL.Add("windows-31j", "shift_jis");
        ENCODING_FROM_LABEL.Add("x-sjis", "shift_jis");

        // Legacy multi-byte Korean encodings
        // ------------------------------------

        // euc-kr
        ENCODING_FROM_LABEL.Add("cseuckr", "euc-kr");
        ENCODING_FROM_LABEL.Add("csksc56011987", "euc-kr");
        ENCODING_FROM_LABEL.Add("euc-kr", "euc-kr");
        ENCODING_FROM_LABEL.Add("iso-ir-149", "euc-kr");
        ENCODING_FROM_LABEL.Add("korean", "euc-kr");
        ENCODING_FROM_LABEL.Add("ks_c_5601-1987", "euc-kr");
        ENCODING_FROM_LABEL.Add("ks_c_5601-1989", "euc-kr");
        ENCODING_FROM_LABEL.Add("ksc5601", "euc-kr");
        ENCODING_FROM_LABEL.Add("ksc_5601", "euc-kr");
        ENCODING_FROM_LABEL.Add("windows-949", "euc-kr");

        // Legacy miscellaneous encodings
        // ------------------------------------

        // replacement
        ENCODING_FROM_LABEL.Add("csiso2022kr", "replacement");
        ENCODING_FROM_LABEL.Add("iso-2022-cn", "replacement");
        ENCODING_FROM_LABEL.Add("iso-2022-cn-ext", "replacement");
        ENCODING_FROM_LABEL.Add("iso-2022-kr", "replacement");

        // utf-16be
        ENCODING_FROM_LABEL.Add("utf-16be", "utf-16be");

        // utf-16le
        ENCODING_FROM_LABEL.Add("utf-16", "utf-16le");
        ENCODING_FROM_LABEL.Add("utf-16le", "utf-16le");

        // utf-16le
        ENCODING_FROM_LABEL.Add("x-user-defined", "x-user-defined");
    }

    private static const byte[] XML_DECLARATION_PREFIX = Encoding.ASCII.GetBytes("<?xml ");

    /// <summary>
    /// The number of HTML bytes to sniff for encoding info embedded in <tt>meta</tt> tags;
    /// relatively large because we don't have a fallback.
    /// </summary>
    private static const int SIZE_OF_HTML_CONTENT_SNIFFED = 4096;

    /// <summary>
    /// The number of XML bytes to sniff for encoding info embedded in the XML declaration;
    /// relatively small because it's always at the very beginning of the file.
    /// </summary>
    private static const int SIZE_OF_XML_CONTENT_SNIFFED = 512;

    /// <summary>
    /// Disallow instantiation of this class.
    /// </summary>
    private EncodingSniffer() {
        // Empty.
    }

    /// <summary>
    /// <p>If the specified content is HTML content, this method sniffs encoding settings
    /// from the specified HTML content and/or the corresponding HTTP headers based on the
    /// <a href="http://www.whatwg.org/specs/web-apps/current-work/multipage/parsing.html#determining-the-character-encoding">HTML5
    /// encoding sniffing algorithm</a>.</p>
    /// 
    /// <p>If the specified content is XML content, this method sniffs encoding settings
    /// from the specified XML content and/or the corresponding HTTP headers using a custom algorithm.</p>
    /// 
    /// <p>Otherwise, this method sniffs encoding settings from the specified content of unknown type by looking for
    /// <tt>Content-Type</tt> information in the HTTP headers and
    /// <a href="http://en.wikipedia.org/wiki/Byte_Order_Mark">Byte Order Mark</a> information in the content.</p>
    /// 
    /// <p>Note that if an encoding is found but it is not supported on the current platform, this method returns
    /// <tt>null</tt>, as if no encoding had been found.</p>
    /// </summary>
    /// <param name="headers">the HTTP response headers sent back with the content to be sniffed</param>
    /// <param name="content">the content to be sniffed</param>
    /// <returns>the encoding sniffed from the specified content and/or the corresponding HTTP headers, or <tt>null</tt> if the encoding could not be determined</returns>
    public static String SniffEncoding(List<NameValuePair> headers, InputStream content) 
    {
        if (IsHtml(headers)) {
            return SniffHtmlEncoding(headers, content);
        }
        else if (IsXml(headers)) {
            return SniffXmlEncoding(headers, content);
        }
        else {
            return SniffUnknownContentTypeEncoding(headers, content);
        }
    }

    /// <summary>
    /// Returns <tt>true</tt> if the specified HTTP response headers indicate an HTML response.
    /// </summary>
    /// <param name="List">the HTTP response headers</param>
    /// <returns><tt>true</tt> if the specified HTTP response headers indicate an HTML response</returns>
    static bool IsHtml(List<NameValuePair> headers) {
        return ContentTypeEndsWith(headers, "text/html");
    }

    /// <summary>
    /// Returns <tt>true</tt> if the specified HTTP response headers indicate an XML response.
    /// </summary>
    /// <param name="headers">the HTTP response headers</param>
    /// <returns><tt>true</tt> if the specified HTTP response headers indicate an XML response</returns>
    static bool IsXml(List<NameValuePair> headers) {
        return ContentTypeEndsWith(headers, "text/xml", "application/xml", "text/vnd.wap.wml", "+xml");
    }

    /// <summary>
    /// Returns <tt>true</tt> if the specified HTTP response headers contain a <tt>Content-Type</tt> that
    /// ends with one of the specified strings.
    /// </summary>
    /// <param name="headers">the HTTP response headers</param>
    /// <param name="contentTypeEndings">the content type endings to search for</param>
    /// <returns><tt>true</tt> if the specified HTTP response headers contain a <tt>Content-Type</tt> that ends with one of the specified strings</returns>
    static bool ContentTypeEndsWith(List<NameValuePair> headers, params String[] contentTypeEndings) {
        foreach (NameValuePair pair in headers) 
        {
            String name = pair.Name;
            if (String.Equals("content-type", name, StringComparison.InvariantCultureIgnoreCase)) {
                String value = pair.Value;
                int i = value.IndexOf(';');
                if (i != -1) {
                    value = value.Substring(0, i);
                }
                value = value.Trim().ToLower(); // TODO : was Locale.ENGLISH
                bool found = false;
                foreach (String ending in contentTypeEndings) {
                    if (value.EndsWith(ending.ToLower())) { // TODO : was Locale.ENGLISH
                        found = true;
                        break;
                    }
                }
                return found;
            }
        }
        return false;
    }

    /// <summary>
    /// <p>Sniffs encoding settings from the specified HTML content and/or the corresponding HTTP headers based on the
    /// <a href="http://www.whatwg.org/specs/web-apps/current-work/multipage/parsing.html#determining-the-character-encoding">HTML5
    /// encoding sniffing algorithm</a>.</p>
    /// 
    /// <p>Note that if an encoding is found but it is not supported on the current platform, this method returns
    /// <tt>null</tt>, as if no encoding had been found.</p>
    /// </summary>
    /// <param name="headers">the HTTP response headers sent back with the HTML content to be sniffed</param>
    /// <param name="content">the HTML content to be sniffed</param>
    /// <returns>the encoding sniffed from the specified HTML content and/or the corresponding HTTP headers, or <tt>null</tt> if the encoding could not be determined</returns>
    public static String SniffHtmlEncoding(List<NameValuePair> headers, InputStream content) 
    {
        String encoding = SniffEncodingFromHttpHeaders(headers);
        if (encoding != null || content == null) {
            return encoding;
        }

        byte[] bytes = Read(content, 3);
        encoding = SniffEncodingFromUnicodeBom(bytes);
        if (encoding != null) {
            return encoding;
        }

        bytes = ReadAndPrepend(content, SIZE_OF_HTML_CONTENT_SNIFFED, bytes);
        encoding = SniffEncodingFromMetaTag(bytes);
        return encoding;
    }

    /// <summary>
    /// <p>Sniffs encoding settings from the specified XML content and/or the corresponding HTTP headers using
    /// a custom algorithm.</p>
    /// <p>Note that if an encoding is found but it is not supported on the current platform, this method returns
    /// <tt>null</tt>, as if no encoding had been found.</p>
    /// </summary>
    /// <param name="headers">the HTTP response headers sent back with the XML content to be sniffed</param>
    /// <param name="content">the XML content to be sniffed</param>
    /// <returns>the encoding sniffed from the specified XML content and/or the corresponding HTTP headers, or <tt>null</tt> if the encoding could not be determined</returns>
    public static String SniffXmlEncoding(List<NameValuePair> headers, InputStream content)
    {
        String encoding = SniffEncodingFromHttpHeaders(headers);
        if (encoding != null || content == null) {
            return encoding;
        }

        byte[] bytes = Read(content, 3);
        encoding = SniffEncodingFromUnicodeBom(bytes);
        if (encoding != null) {
            return encoding;
        }

        bytes = ReadAndPrepend(content, SIZE_OF_XML_CONTENT_SNIFFED, bytes);
        encoding = SniffEncodingFromXmlDeclaration(bytes);
        return encoding;
    }

    /// <summary>
    /// <p>Sniffs encoding settings from the specified content of unknown type by looking for <tt>Content-Type</tt>
    /// information in the HTTP headers and <a href="http://en.wikipedia.org/wiki/Byte_Order_Mark">Byte Order Mark</a>
    /// information in the content.</p>
    /// 
    /// <p>Note that if an encoding is found but it is not supported on the current platform, this method returns
    /// <tt>null</tt>, as if no encoding had been found.</p>
    /// </summary>
    /// <param name="headers">the HTTP response headers sent back with the content to be sniffed</param>
    /// <param name="content">the content to be sniffed</param>
    /// <returns>the encoding sniffed from the specified content and/or the corresponding HTTP headers, or <tt>null</tt> if the encoding could not be determined</returns>
    public static String SniffUnknownContentTypeEncoding(List<NameValuePair> headers, InputStream content)
    {
        String encoding = SniffEncodingFromHttpHeaders(headers);
        if (encoding != null || content == null) {
            return encoding;
        }

        byte[] bytes = Read(content, 3);
        encoding = SniffEncodingFromUnicodeBom(bytes);
        return encoding;
    }

    /// <summary>
    /// Attempts to sniff an encoding from the specified HTTP headers.
    /// </summary>
    /// <param name="headers">the HTTP headers to examine</param>
    /// <returns>the encoding sniffed from the specified HTTP headers, or <tt>null</tt> if the encoding could not be determined</returns>
    static String SniffEncodingFromHttpHeaders(List<NameValuePair> headers) {
        String encoding = null;
        foreach (NameValuePair pair in headers) {
            String name = pair.Name;
            if (String.Equals("content-type", name, StringComparison.InvariantCultureIgnoreCase)) {
                String value = pair.Value;
                encoding = ExtractEncodingFromContentType(value);
                if (encoding != null) {
                    encoding = encoding.ToUpper(); // TODO : was Locale.ENGLISH
                    break;
                }
            }
        }
        if (encoding != null && LOG.IsDebugEnabled) {
            LOG.Debug("Encoding found in HTTP headers: '" + encoding + "'.");
        }
        return encoding;
    }

    /// <summary>
    /// Attempts to sniff an encoding from a <a href="http://en.wikipedia.org/wiki/Byte_Order_Mark">Byte Order Mark</a>
    /// in the specified byte array.
    /// </summary>
    /// <param name="bytes">the bytes to check for a Byte Order Mark</param>
    /// <returns>the encoding sniffed from the specified bytes, or <tt>null</tt> if the encoding could not be determined</returns>
    static String SniffEncodingFromUnicodeBom(byte[] bytes) {
        if (bytes == null) {
            return null;
        }

        String encoding = null;
        // 0xef, 0xbb, 0xbf
        if (bytes.Length > 2
                && ((byte) 0xef) == bytes[0]
                && ((byte) 0xbb) == bytes[1]
                && ((byte) 0xbf) == bytes[2]) {
            encoding = UTF8;
        }
        // 0xfe, 0xff
        else if (bytes.Length > 2
                && ((byte) 0xfe) == bytes[0]
                && ((byte) 0xff) == bytes[1]) {
            encoding = UTF16_BE;
        }
        // 0xff, 0xfe
        else if (bytes.Length > 2
                && ((byte) 0xff) == bytes[0]
                && ((byte) 0xfe) == bytes[1]) {
            encoding = UTF16_LE;
        }

        if (encoding != null && LOG.IsDebugEnabled) {
            LOG.Debug("Encoding found in Unicode Byte Order Mark: '" + encoding + "'.");
        }
        return encoding;
    }

    /// <summary>
    /// Attempts to sniff an encoding from an HTML <tt>meta</tt> tag in the specified byte array.
    /// </summary>
    /// <param name="bytes">the bytes to check for an HTML <tt>meta</tt> tag</param>
    /// <returns>the encoding sniffed from the specified bytes, or <tt>null</tt> if the encoding could not be determined</returns>
    static String SniffEncodingFromMetaTag(byte[] bytes) {
        for (int i = 0; i < bytes.Length; i++) {
            if (Matches(bytes, i, COMMENT_START)) {
                i = IndexOfSubArray(bytes, new byte[] {Convert.ToByte('-'), Convert.ToByte('-'), Convert.ToByte('>')}, i);
                if (i == -1) {
                    break;
                }
                i += 2;
            }
            else if (Matches(bytes, i, META_START)) {
                i += META_START.Length;
                for (Attribute att = GetAttribute(bytes, i); att != null; att = GetAttribute(bytes, i)) {
                    i = att.UpdatedIndex;
                    String name = att.Name;
                    String value = att.Value;
                    if (String.Equals("charset", name, StringComparison.InvariantCultureIgnoreCase) || String.Equals("content", name, StringComparison.InvariantCultureIgnoreCase)) {
                        String charset = null;
                        if (String.Equals("charset", name, StringComparison.InvariantCultureIgnoreCase)
                        {
                            charset = value;
                        }
                        else if (String.Equals("content", name, StringComparison.InvariantCultureIgnoreCase))
                        {
                            charset = ExtractEncodingFromContentType(value);
                            if (charset == null) {
                                continue;
                            }
                        }
                        if (String.Equals(UTF16_BE, charset, StringComparison.InvariantCultureIgnoreCase) || String.Equals(UTF16_LE, charset, StringComparison.InvariantCultureIgnoreCase)) {
                            charset = UTF8;
                        }
                        if (IsSupportedCharset(charset)) {
                            charset = charset.ToUpper(); // Locale.ENGLISH
                            if (LOG.IsDebugEnabled) {
                                LOG.Debug("Encoding found in meta tag: '" + charset + "'.");
                            }
                            return charset;
                        }
                    }
                }
            }
            else if (i + 1 < bytes.Length && bytes[i] == '<' && Char.IsLetter(Convert.ToChar(bytes[i + 1]))) 
            {
                i = SkipToAnyOf(bytes, i, new byte[] {0x09, 0x0A, 0x0C, 0x0D, 0x20, 0x3E});
                if (i == -1) {
                    break;
                }
                Attribute att;
                while ((att = GetAttribute(bytes, i)) != null) {
                    i = att.UpdatedIndex;
                }
            }
            else if (i + 2 < bytes.Length && bytes[i] == '<' && bytes[i + 1] == '/' && Char.IsLetter(Convert.ToChar(bytes[i + 2]))) {
                i = SkipToAnyOf(bytes, i, new byte[] {0x09, 0x0A, 0x0C, 0x0D, 0x20, 0x3E});
                if (i == -1) {
                    break;
                }
                Attribute attribute;
                while ((attribute = GetAttribute(bytes, i)) != null) {
                    i = attribute.UpdatedIndex;
                }
            }
            else if (Matches(bytes, i, OTHER_START)) {
                i = SkipToAnyOf(bytes, i, new byte[] {0x3E});
                if (i == -1) {
                    break;
                }
            }
        }
        return null;
    }


    /// <summary>
    /// Extracts an attribute from the specified byte array, starting at the specified index, using the
    /// <a href="http://www.whatwg.org/specs/web-apps/current-work/multipage/parsing.html#concept-get-attributes-when-sniffing">HTML5
    /// attribute algorithm</a>.
    /// </summary>
    /// <param name="bytes">the byte array to extract an attribute from</param>
    /// <param name="i">the index to start searching from</param>
    /// <returns>the next attribute in the specified byte array, or <tt>null</tt> if one is not available</returns>
    static Attribute GetAttribute(byte[] bytes, int i) {
        if (i >= bytes.Length) {
            return null;
        }
        while (bytes[i] == 0x09 || bytes[i] == 0x0A || bytes[i] == 0x0C || bytes[i] == 0x0D || bytes[i] == 0x20 || bytes[i] == 0x2F) {
            i++;
            if (i >= bytes.Length) {
                return null;
            }
        }
        if (bytes[i] == '>') {
            return null;
        }
        StringBuilder name = new StringBuilder();
        StringBuilder value = new StringBuilder();
        for ( ;; i++) {
            if (i >= bytes.Length) {
                return new Attribute(name.ToString(), value.ToString(), i);
            }
            if (bytes[i] == '=' && name.Length != 0) {
                i++;
                break;
            }
            if (bytes[i] == 0x09 || bytes[i] == 0x0A || bytes[i] == 0x0C || bytes[i] == 0x0D || bytes[i] == 0x20) {
                while (bytes[i] == 0x09 || bytes[i] == 0x0A || bytes[i] == 0x0C || bytes[i] == 0x0D || bytes[i] == 0x20) {
                    i++;
                    if (i >= bytes.Length) {
                        return new Attribute(name.ToString(), value.ToString(), i);
                    }
                }
                if (bytes[i] != '=') {
                    return new Attribute(name.ToString(), value.ToString(), i);
                }
                i++;
                break;
            }
            if (bytes[i] == '/' || bytes[i] == '>') {
                return new Attribute(name.ToString(), value.ToString(), i);
            }
            name.Append((char) bytes[i]);
        }
        if (i >= bytes.Length) {
            return new Attribute(name.ToString(), value.ToString(), i);
        }
        while (bytes[i] == 0x09 || bytes[i] == 0x0A || bytes[i] == 0x0C || bytes[i] == 0x0D || bytes[i] == 0x20) {
            i++;
            if (i >= bytes.Length) {
                return new Attribute(name.ToString(), value.ToString(), i);
            }
        }
        if (bytes[i] == '"' || bytes[i] == '\'') {
            byte b = bytes[i];
            for (i++; i < bytes.Length; i++) {
                if (bytes[i] == b) {
                    i++;
                    return new Attribute(name.ToString(), value.ToString(), i);
                }
                else if (bytes[i] >= 'A' && bytes[i] <= 'Z') {
                    byte b2 = (byte) (bytes[i] + 0x20);
                    value.Append((char) b2);
                }
                else {
                    value.Append((char) bytes[i]);
                }
            }
            return new Attribute(name.ToString(), value.ToString(), i);
        }
        else if (bytes[i] == '>') {
            return new Attribute(name.ToString(), value.ToString(), i);
        }
        else if (bytes[i] >= 'A' && bytes[i] <= 'Z') {
            byte b = (byte) (bytes[i] + 0x20);
            value.Append((char) b);
            i++;
        }
        else {
            value.Append((char) bytes[i]);
            i++;
        }
        for ( ; i < bytes.Length; i++) {
            if (bytes[i] == 0x09 || bytes[i] == 0x0A || bytes[i] == 0x0C || bytes[i] == 0x0D || bytes[i] == 0x20 || bytes[i] == 0x3E) {
                return new Attribute(name.ToString(), value.ToString(), i);
            }
            else if (bytes[i] >= 'A' && bytes[i] <= 'Z') {
                byte b = (byte) (bytes[i] + 0x20);
                value.Append((char) b);
            }
            else {
                value.Append((char) bytes[i]);
            }
        }
        return new Attribute(name.ToString(), value.ToString(), i);
    }

    /// <summary>
    /// Extracts an encoding from the specified <tt>Content-Type</tt> value using
    /// <a href="http://ietfreport.isoc.org/idref/draft-abarth-mime-sniff/">the IETF algorithm</a>; if
    /// no encoding is found, this method returns <tt>null</tt>.
    /// </summary>
    /// <param name="s">the <tt>Content-Type</tt> value to search for an encoding</param>
    /// <returns>the encoding found in the specified <tt>Content-Type</tt> value, or <tt>null</tt> if no encoding was found</returns>
    static String ExtractEncodingFromContentType(String s) {
        if (s == null) {
            return null;
        }
        byte[] bytes = Encoding.ASCII.GetBytes(s);
        int i;
        for (i = 0; i < bytes.Length; i++) {
            if (Matches(bytes, i, CHARSET_START)) {
                i += CHARSET_START.Length;
                break;
            }
        }
        if (i == bytes.Length) {
            return null;
        }
        while (bytes[i] == 0x09 || bytes[i] == 0x0A || bytes[i] == 0x0C || bytes[i] == 0x0D || bytes[i] == 0x20) {
            i++;
            if (i == bytes.Length) {
                return null;
            }
        }
        if (bytes[i] != '=') {
            return null;
        }
        i++;
        if (i == bytes.Length) {
            return null;
        }
        while (bytes[i] == 0x09 || bytes[i] == 0x0A || bytes[i] == 0x0C || bytes[i] == 0x0D || bytes[i] == 0x20) {
            i++;
            if (i == bytes.Length) {
                return null;
            }
        }
        if (bytes[i] == '"') {
            if (bytes.Length <= i + 1) {
                return null;
            }
            int index = ArrayUtils.indexOf(bytes, (byte) '"', i + 1);
            if (index == -1) {
                return null;
            }
            String charset = Encoding.ASCII.GetString(ArrayUtils.subarray(bytes, i + 1, index));
            return IsSupportedCharset(charset) ? charset : null;
        }
        if (bytes[i] == '\'') {
            if (bytes.Length <= i + 1) {
                return null;
            }
            int index = ArrayUtils.indexOf(bytes, (byte) '\'', i + 1);
            if (index == -1) {
                return null;
            }
            String charset = Encoding.ASCII.GetString(ArrayUtils.subarray(bytes, i + 1, index));
            return IsSupportedCharset(charset) ? charset : null;
        }
        int end = SkipToAnyOf(bytes, i, new byte[] {0x09, 0x0A, 0x0C, 0x0D, 0x20, 0x3B});
        if (end == -1) {
            end = bytes.Length;
        }
        String characterset = Encoding.ASCII.GetString(ArrayUtils.subarray(bytes, i, end));
        return IsSupportedCharset(characterset) ? characterset : null;
    }

    /// <summary>
    /// Searches the specified XML content for an XML declaration and returns the encoding if found,
    /// otherwise returns <tt>null</tt>.
    /// </summary>
    /// <param name="bytes">the XML content to sniff</param>
    /// <returns>the encoding of the specified XML content, or <tt>null</tt> if it could not be determined</returns>
    static String SniffEncodingFromXmlDeclaration(byte[] bytes) {
        String encoding = null;
        if (bytes.Length > 5
                && XML_DECLARATION_PREFIX[0] == bytes[0]
                && XML_DECLARATION_PREFIX[1] == bytes[1]
                && XML_DECLARATION_PREFIX[2] == bytes[2]
                && XML_DECLARATION_PREFIX[3] == bytes[3]
                && XML_DECLARATION_PREFIX[4] == bytes[4]
                && XML_DECLARATION_PREFIX[5] == bytes[5]) {
            int index = ArrayUtils.indexOf(bytes, (byte) '?', 2);
            if (index + 1 < bytes.Length && bytes[index + 1] == '>') {
                String declaration = Encoding.ASCII.GetString(bytes, 0, index + 2);
                int start = declaration.IndexOf("encoding");
                if (start != -1) {
                    start += 8;
                    char delimiter;
                    while (true) {
                        bool breakOuter = false;
                        switch (declaration[start]) {
                            case '"':
                            case '\'':
                                delimiter = declaration[start];
                                start = start + 1;
                                breakOuter = true;
                                break;
                            default:
                                start++;
                                break;
                        }
                        if (breakOuter) break;
                    }
                    int end = declaration.IndexOf(delimiter, start);
                    encoding = declaration.Substring(start, end);
                }
            }
        }
        if (encoding != null && !IsSupportedCharset(encoding)) {
            encoding = null;
        }
        if (encoding != null && LOG.IsDebugEnabled) {
            LOG.Debug("Encoding found in XML declaration: '" + encoding + "'.");
        }
        return encoding;
    }

    /// <summary>
    /// Returns <tt>true</tt> if the specified charset is supported on this platform.
    /// </summary>
    /// <param name="charset">the charset to check</param>
    /// <returns><tt>true</tt> if the specified charset is supported on this platform</returns>
    static bool IsSupportedCharset(String charset) {
        try {
            Encoding.GetEncoding(charset);
            return true;
        }
        catch (Exception e) {
            return false;
        }
    }

    /// <summary>
    /// Returns <tt>true</tt> if the byte in the specified byte array at the specified index matches one of the
    /// specified byte array patterns.
    /// </summary>
    /// <param name="bytes">the byte array to search in</param>
    /// <param name="i">the index at which to search</param>
    /// <param name="sought">the byte array patterns to search for</param>
    /// <returns><tt>true</tt> if the byte in the specified byte array at the specified index matches one of the specified byte array patterns</returns>
    static bool Matches(byte[] bytes, int i, byte[][] sought) {
        if (i + sought.Length > bytes.Length) {
            return false;
        }
        for (int x = 0; x < sought.Length; x++) {
            byte[] possibilities = sought[x];
            bool match = false;
            for (int y = 0; y < possibilities.Length; y++) {
                if (bytes[i + x] == possibilities[y]) {
                    match = true;
                    break;
                }
            }
            if (!match) {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// Skips ahead to the first occurrence of any of the specified targets within the specified array,
    /// starting at the specified index. This method returns <tt>-1</tt> if none of the targets are found.
    /// </summary>
    /// <param name="bytes">the array to search through</param>
    /// <param name="i">the index to start looking at</param>
    /// <param name="targets">the targets to search for</param>
    /// <returns>the index of the first occurrence of any of the specified targets within the specified array</returns>
    static int SkipToAnyOf(byte[] bytes, int i, byte[] targets) {
        for ( ; i < bytes.Length; i++) {
            if (ArrayUtils.contains(targets, bytes[i])) {
                break;
            }
        }
        if (i == bytes.Length) {
            i = -1;
        }
        return i;
    }

    /// <summary>
    /// Finds the first index of the specified sub-array inside the specified array, starting at the
    /// specified index. This method returns <tt>-1</tt> if the specified sub-array cannot be found.
    /// </summary>
    /// <param name="array">the array to traverse for looking for the sub-array</param>
    /// <param name="subarray">the sub-array to find</param>
    /// <param name="startIndex">the start index to traverse forwards from</param>
    /// <returns>the index of the sub-array within the array</returns>
    static int IndexOfSubArray(byte[] array, byte[] subarray, int startIndex) {
        for (int i = startIndex; i < array.Length; i++) {
            bool found = true;
            if (i + subarray.Length > array.Length) {
                break;
            }
            for (int j = 0; j < subarray.Length; j++) {
                byte a = array[i + j];
                byte b = subarray[j];
                if (a != b) {
                    found = false;
                    break;
                }
            }
            if (found) {
                return i;
            }
        }
        return -1;
    }

    /// <summary>
    /// Attempts to read <tt>size</tt> bytes from the specified input stream. Note that this method is not guaranteed
    /// to be able to read <tt>size</tt> bytes; however, the returned byte array will always be the exact length of the
    /// number of bytes read.
    /// @throws IOException if an IO error occurs
    /// </summary>
    /// <param name="content">the input stream to read from</param>
    /// <param name="size">size the number of bytes to try to read</param>
    /// <returns>the bytes read from the specified input stream</returns>
    static byte[] Read(InputStream content, int size) 
    {
        byte[] bytes = new byte[size];
        int count = content.Read(bytes, 0, size);
        if (count == -1) {
            bytes = new byte[0];
        }
        else if (count < size) {
            byte[] smaller = new byte[count];
            Array.Copy(bytes, 0, smaller, 0, count);
            bytes = smaller;
        }
        return bytes;
    }

    /// <summary>
    /// Attempts to read <tt>size</tt> bytes from the specified input stream and then prepends the specified prefix to
    /// the bytes read, returning the resultant byte array. Note that this method is not guaranteed to be able to read
    /// <tt>size</tt> bytes; however, the returned byte array will always be the exact length of the number of bytes
    /// read plus the length of the prefix array.
    /// @throws IOException if an IO error occurs
    /// </summary>
    /// <param name="content">the input stream to read from</param>
    /// <param name="size">the number of bytes to try to read</param>
    /// <param name="prefix">the byte array to prepend to the bytes read from the specified input stream</param>
    /// <returns>the bytes read from the specified input stream, prefixed by the specified prefix</returns>
    static byte[] ReadAndPrepend(InputStream content, int size, byte[] prefix)
    {
        byte[] bytes = Read(content, size);
        byte[] joined = new byte[prefix.Length + bytes.Length];
        Array.Copy(prefix, 0, joined, 0, prefix.Length);
        Array.Copy(bytes, 0, joined, prefix.Length, bytes.Length);
        return joined;
    }

    /// <summary>
    /// Translates the given encoding label into a normalized form
    /// according to <a href="http://encoding.spec.whatwg.org/#encodings">Reference</a>.
    /// </summary>
    /// <param name="encodingLabel">the label to translate</param>
    /// <returns>the normalized encoding name or null if not found</returns>
    public static String TranslateEncodingLabel(String encodingLabel) {
        if (null == encodingLabel) {
            return null;
        }
        String encLC = encodingLabel.Trim().ToLower(); // TODO : Locale.ENGLISH
        String enc = ENCODING_FROM_LABEL[encLC];
        if (String.Equals(encLC, enc)) {
            return encodingLabel;
        }
        return enc;
    }
    }
}
