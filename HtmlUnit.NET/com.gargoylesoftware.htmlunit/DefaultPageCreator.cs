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
    /// The default implementation of {@link PageCreator}. Designed to be extended for easier handling of new content
    /// types. Just check the content type in <tt>createPage()</tt> and call <tt>super(createPage())</tt> if your custom
    /// type isn't found. There are also protected <tt>createXXXXPage()</tt> methods for creating the {@link Page} types
    /// which HtmlUnit already knows about for your custom content types.
    /// 
    /// <p>
    /// The following table shows the type of {@link Page} created depending on the content type:<br>
    /// <br>
    ///  <table border="1" width="50%" summary="Page Types">
    ///    <tr>
    ///      <th>Content type</th>
    ///      <th>Type of page</th>
    ///    </tr>
    ///    <tr>
    ///      <td>text/html</td>
    ///      <td>{@link HtmlPage}</td>
    ///    </tr>
    ///    <tr>
    ///      <td>text/xml<br>
    ///      application/xml<br>
    ///      text/vnd.wap.wml<br>
    ///      *+xml
    ///      </td>
    ///      <td>{@link XmlPage}, or an {@link XHtmlPage} if an XHTML namespace is used</td>
    ///    </tr>
    ///    <tr>
    ///      <td>text/javascript<br>
    ///      application/x-javascript
    ///      </td>
    ///      <td>{@link JavaScriptPage}</td>
    ///    </tr>
    ///    <tr>
    ///      <td>text/*</td>
    ///      <td>{@link TextPage}</td>
    ///    </tr>
    ///    <tr>
    ///      <td>Anything else</td>
    ///      <td>{@link UnexpectedPage}</td>
    ///    </tr>
    ///  </table>
    /// 
    /// @version $Revision: 10875 $
    /// @author <a href="mailto:mbowler@GargoyleSoftware.com">Mike Bowler</a>
    /// @author <a href="mailto:cse@dynabean.de">Christian Sell</a>
    /// @author <a href="mailto:yourgod@users.sourceforge.net">Brad Clarke</a>
    /// @author Marc Guillemot
    /// @author Ahmed Ashour
    /// @author Daniel Gredler
    /// @author Ronald Brill
    /// </summary>
    [Serializable]
    public class DefaultPageCreator : IPageCreator
    {
    /// <summary>
    /// Determines the kind of page to create from the content type.
    /// </summary>
    /// <param name="contentType">the content type to evaluate</param>
    /// <returns>"xml", "html", "javascript", "text" or "unknown"</returns>
    public static PageType DeterminePageType(String contentType) {
        if (null == contentType) {
            return PageType.UNKNOWN;
        }

        if (String.Equals("text/html", contentType)) {
            return PageType.HTML;
        }

        if (String.Equals("text/javascript", contentType) || String.Equals("application/x-javascript", contentType)
                || String.Equals("application/javascript", contentType)) {
            return PageType.JAVASCRIPT;
        }

        if (String.Equals("text/xml", contentType)
            || String.Equals("application/xml", contentType)
            || String.Equals("text/vnd.wap.wml", contentType)
            || contentType.EndsWith("+xml")) {
            return PageType.XML;
        }

        if (contentType.StartsWith("text/")) {
            return PageType.TEXT;
        }

        return PageType.UNKNOWN;
    }

    /// <summary>
    /// Creates an instance.
    /// </summary>
    public DefaultPageCreator() {
        // Empty.
    }

    /// <summary>
    /// Create a Page object for the specified web response.
    /// </summary>
    /// <param name="webResponse">the response from the server</param>
    /// <param name="webWindow">the window that this page will be loaded into</param>
    /// <returns>the new page object</returns>
    public IPage CreatePage(WebResponse webResponse, IWebWindow webWindow) 
    {
        String contentType = DetermineContentType(webResponse.GetContentType().ToLower(), // TODO : Locale.ENGLISH
            webResponse.GetContentAsStream());

        PageType pageType = DeterminePageType(contentType);
        switch (pageType) {
            case PageType.HTML:
                return CreateHtmlPage(webResponse, webWindow);

            case PageType.JAVASCRIPT:
                return CreateJavaScriptPage(webResponse, webWindow);

            case PageType.XML:
                SgmlPage sgmlPage = CreateXmlPage(webResponse, webWindow);
                DomElement doc = sgmlPage.getDocumentElement();
                if (doc != null && HTMLParser.XHTML_NAMESPACE.equals(doc.getNamespaceURI())) {
                    return CreateXHtmlPage(webResponse, webWindow);
                }
                return sgmlPage;

            case PageType.TEXT:
                return CreateTextPage(webResponse, webWindow);

            default:
                return CreateUnexpectedPage(webResponse, webWindow);
        }
    }

    /// <summary>
    /// Tries to determine the content type.
    /// TODO: implement a content type sniffer based on the
    /// <a href="http://tools.ietf.org/html/draft-abarth-mime-sniff-05">Content-Type Processing Model</a>
    /// @exception IOException if an IO problem occurs
    /// </summary>
    /// <param name="contentType">the contentType header if any</param>
    /// <param name="contentAsStream">stream allowing to read the downloaded content</param>
    /// <returns>the sniffed mime type</returns>
    protected String DetermineContentType(String contentType, InputStream contentAsStream) 
    {
        byte[] markerUTF8 = {(byte) 0xef, (byte) 0xbb, (byte) 0xbf};
        byte[] markerUTF16BE = {(byte) 0xfe, (byte) 0xff};
        byte[] markerUTF16LE = {(byte) 0xff, (byte) 0xfe};

        try {
            if (!String.IsNullOrEmpty(contentType)) {
                return contentType;
            }

            byte[] bytes = Read(contentAsStream, 500);
            if (bytes.Length == 0) {
                return "text/plain";
            }

            String asAsciiString = Encoding.ASCII.GetString(bytes).ToUpper(); // TODO : Locale.ENGLISH
            if (asAsciiString.Contains("<HTML")) {
                return "text/html";
            }
            else if (StartsWith(bytes, markerUTF8) || StartsWith(bytes, markerUTF16BE)
                    || StartsWith(bytes, markerUTF16LE)) {
                return "text/plain";
            }
            else if (IsBinary(bytes)) {
                return "application/octet-stream";
            }
        }
        finally {
            IOUtils.CloseQuietly(contentAsStream);
        }
        return "text/plain";
    }

    /// <summary>
    /// See http://tools.ietf.org/html/draft-abarth-mime-sniff-05#section-4
    /// </summary>
    /// <param name="bytes">the bytes to check</param>
    /// <returns></returns>
    private bool IsBinary(byte[] bytes) {
        foreach (byte b in bytes) {
            if (b < 0x08
                || b == 0x0B
                || (b >= 0x0E && b <= 0x1A)
                || (b >= 0x1C && b <= 0x1F)) {
                return true;
            }
        }
        return false;
    }

    private bool StartsWith(byte[] bytes, byte[] lookFor) {
        if (bytes.Length < lookFor.Length) {
            return false;
        }

        for (int i = 0; i < lookFor.Length; ++i) {
            if (bytes[i] != lookFor[i]) {
                return false;
            }
        }

        return true;
    }

    private byte[] Read(InputStream stream, int maxNb) 
    {
        byte[] buffer = new byte[maxNb];
        int nbRead = stream.Read(buffer, 0, maxNb);
        if (nbRead == buffer.Length) {
            return buffer;
        }
        return ArrayUtils.subarray(buffer, 0, nbRead);
    }

    /// <summary>
    /// Creates an HtmlPage for this WebResponse.
    /// </summary>
    /// <param name="webResponse">the page's source</param>
    /// <param name="webWindow">the WebWindow to place the HtmlPage in</param>
    /// <returns>the newly created HtmlPage</returns>
    protected HtmlPage CreateHtmlPage(WebResponse webResponse, IWebWindow webWindow) 
    {
        return HTMLParser.parseHtml(webResponse, webWindow);
    }

    /// <summary>
    /// Creates an XHtmlPage for this WebResponse.
    /// @throws IOException if the page could not be created
    /// </summary>
    /// <param name="webResponse">the page's source</param>
    /// <param name="webWindow">the WebWindow to place the HtmlPage in</param>
    /// <returns>the newly created XHtmlPage</returns>
    protected XHtmlPage CreateXHtmlPage(WebResponse webResponse, IWebWindow webWindow)
    {
        return HTMLParser.parseXHtml(webResponse, webWindow);
    }

    /// <summary>
    /// Creates a JavaScriptPage for this WebResponse.
    /// </summary>
    /// <param name="webResponse">the page's source</param>
    /// <param name="webWindow">the WebWindow to place the JavaScriptPage in</param>
    /// <returns>the newly created JavaScriptPage</returns>
    protected JavaScriptPage CreateJavaScriptPage(WebResponse webResponse, IWebWindow webWindow) {
        JavaScriptPage newPage = new JavaScriptPage(webResponse, webWindow);
        webWindow.EnclosedPage = newPage;
        return newPage;
    }

    /// <summary>
    /// Creates a TextPage for this WebResponse.
    /// </summary>
    /// <param name="webResponse">the page's source</param>
    /// <param name="webWindow">the WebWindow to place the TextPage in</param>
    /// <returns>the newly created TextPage</returns>
    protected TextPage CreateTextPage(WebResponse webResponse, IWebWindow webWindow) {
        TextPage newPage = new TextPage(webResponse, webWindow);
        webWindow.EnclosedPage = newPage;
        return newPage;
    }

    /// <summary>
    /// Creates an UnexpectedPage for this WebResponse.
    /// </summary>
    /// <param name="webResponse">the page's source</param>
    /// <param name="webWindow">the WebWindow to place the UnexpectedPage in</param>
    /// <returns>the newly created UnexpectedPage</returns>
    protected UnexpectedPage CreateUnexpectedPage(WebResponse webResponse, IWebWindow webWindow) {
        UnexpectedPage newPage = new UnexpectedPage(webResponse, webWindow);
        webWindow.EnclosedPage = newPage;
        return newPage;
    }

    /// <summary>
    /// Creates an SgmlPage for this WebResponse.
    /// @throws IOException if the page could not be created
    /// </summary>
    /// <param name="webResponse">the page's source</param>
    /// <param name="webWindow">the WebWindow to place the TextPage in</param>
    /// <returns>the newly created TextPage</returns>
    protected SgmlPage CreateXmlPage(WebResponse webResponse, IWebWindow webWindow)
    {
        SgmlPage page = new XmlPage(webResponse, webWindow);
        if (IsSvg(page)) {
            page = new SvgPage(webResponse, page.getDocumentElement(), webWindow);
        }
        webWindow.EnclosedPage = page;
        return page;
    }

    /// <summary>
    /// Returns whether the specified {@code page} is {@link SvgPage} or not.
    /// </summary>
    /// <param name="page">the page</param>
    /// <returns>whether the specified {@code page} is {@link SvgPage} or not</returns>
    protected bool IsSvg(SgmlPage page) 
    {
        DomElement documentElement = page.getDocumentElement();
        return documentElement != null && page.hasFeature(SVG) && String.Equals("svg", documentElement.getTagName())
                && HTMLParser.SVG_NAMESPACE.equals(documentElement.getNamespaceURI());
    }
    }
}
