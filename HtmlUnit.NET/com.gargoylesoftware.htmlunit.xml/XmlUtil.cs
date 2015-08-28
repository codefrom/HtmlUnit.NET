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
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using HtmlUnit.Helpers;

namespace HtmlUnit.com.gargoylesoftware.htmlunit.xml
{
    /// <summary>
    /// <span style="color:red">INTERNAL API - SUBJECT TO CHANGE AT ANY TIME - USE AT YOUR OWN RISK.</span><br>
    /// Provides facility method to work with XML responses.
    /// @version $Revision: 10913 $
    /// @author Marc Guillemot
    /// @author Ahmed Ashour
    /// @author Sudhan Moghe
    /// @author Ronald Brill
    /// @author Chuck Dumont
    /// @author Frank Danek
    /// </summary>
    public sealed class XmlUtil
    {
        /// <summary>Default encoding used.</summary>
        public const String DEFAULT_CHARSET = "UTF-8";

        private static readonly Log LOG = LogFactory.GetLog(typeof(XmlUtil));

        /// <summary>
        /// Utility class, hide constructor.
        /// </summary>
        private XmlUtil()
        {
            // Empty.
        }

        /// <summary>
        /// Builds a document from the content of the web response.
        /// A warning is logged if an exception is thrown while parsing the XML content
        /// (for instance when the content is not a valid XML and can't be parsed).
        /// @throws IOException if the page could not be created
        /// @throws SAXException if the parsing fails
        /// @throws ParserConfigurationException if a DocumentBuilder cannot be created
        /// </summary>
        /// <param name="WebResponse">the response from the server</param>
        /// <returns>the parse result</returns>
        public static XmlDocument BuildDocument(WebResponse webResponse)
        {
            XmlDocument xml = new XmlDocument();
            if (webResponse == null)
            {
                return xml;
            }

            InputStreamReader reader = new InputStreamReader(webResponse.GetContentAsStream(), Encoding.GetEncoding(webResponse.GetContentCharset()));

            // we have to do the blank input check and the parsing in one step
            TrackBlankContentReader tracker = new TrackBlankContentReader(reader);

            try
            {
                xml.Load(tracker);
                return xml;
            }
            catch (XmlException e)
            {
                if (tracker.WasBlank)
                {
                    return new XmlDocument();
                }
                throw e;
            }
        }

        /// <summary>
        /// Recursively appends a {@link Node} child to {@link DomNode} parent.
        /// </summary>
        /// <param name="page">the owner page of {@link DomElement}s to be created</param>
        /// <param name="parent">the parent DomNode</param>
        /// <param name="child">the child Node</param>
        /// <param name="handleXHTMLAsHTML">if true elements from the XHTML namespace are handled as HTML elements instead of DOM elements</param>
        public static void AppendChild(SgmlPage page, DomNode parent, XmlNode child, bool handleXHTMLAsHTML)
        {
            XmlDocumentType documentType = child.OwnerDocument.DocumentType;
            if (documentType != null && page is XmlPage)
            {
                DomDocumentType domDoctype = new DomDocumentType(
                        page, documentType.Name, documentType.PublicId, documentType.SystemId);
                ((XmlPage)page).setDocumentType(domDoctype);
            }
            DomNode childXml = CreateFrom(page, child, handleXHTMLAsHTML);
            parent.appendChild(childXml);
            Copy(page, child, childXml, handleXHTMLAsHTML);
        }

        private static DomNode CreateFrom(SgmlPage page, XmlNode source, bool handleXHTMLAsHTML)
        {
            if (source.NodeType == XmlNodeType.Text)
            {
                return new DomText(page, source.Value);
            }
            if (source.NodeType == XmlNodeType.ProcessingInstruction)
            {
                return new DomProcessingInstruction(page, source.Name, source.Value);
            }
            if (source.NodeType == XmlNodeType.Comment)
            {
                return new DomComment(page, source.Value);
            }
            if (source.NodeType == XmlNodeType.DocumentType)
            {
                XmlDocumentType documentType = (XmlDocumentType)source;
                return new DomDocumentType(page, documentType.Name, documentType.PublicId, documentType.SystemId);
            }
            String ns = source.NamespaceURI;
            String localName = source.LocalName;
            if (handleXHTMLAsHTML && String.Equals(HTMLParser.XHTML_NAMESPACE, ns))
            {
                ElementFactory factory = HTMLParser.getFactory(localName);
                return factory.createElementNS(page, ns, localName, source.Attributes);
            }
            XmlAttributeCollection nodeAttributes = source.Attributes;
            if (page != null && page.isHtmlPage())
            {
                localName = localName.ToUpper(); // TODO : Locale.ENGLISH
            }
            String qualifiedName;
            if (source.Prefix == null)
            {
                qualifiedName = localName;
            }
            else
            {
                qualifiedName = source.Prefix + ':' + localName;
            }

            String namespaceURI = source.NamespaceURI;
            if (String.Equals(HTMLParser.SVG_NAMESPACE, namespaceURI))
            {
                return HTMLParser.SVG_FACTORY.createElementNS(page, namespaceURI, qualifiedName, nodeAttributes);
            }

            Dictionary<String, DomAttr> attributes = new Dictionary<string, DomAttr>();
            for (int i = 0; i < nodeAttributes.Count; i++)
            {
                XmlAttribute attribute = (XmlAttribute)nodeAttributes.Item(i);
                String attributeNamespaceURI = attribute.NamespaceURI;
                String attributeQualifiedName;
                if (attribute.Prefix != null)
                {
                    attributeQualifiedName = attribute.Prefix + ':' + attribute.LocalName;
                }
                else
                {
                    attributeQualifiedName = attribute.LocalName;
                }
                String value = attribute.Value;
                bool specified = attribute.Specified;
                DomAttr xmlAttribute =
                        new DomAttr(page, attributeNamespaceURI, attributeQualifiedName, value, specified);
                attributes.Add(attribute.Name, xmlAttribute);
            }
            return new DomElement(namespaceURI, qualifiedName, page, attributes);
        }

        /// <summary>
        /// Copy all children from 'source' to 'dest', within the context of the specified page.
        /// </summary>
        /// <param name="page">the page which the nodes belong to</param>
        /// <param name="source">the node to copy from</param>
        /// <param name="dest">the node to copy to</param>
        /// <param name="handleXHTMLAsHTML">if true elements from the XHTML namespace are handled as HTML elements instead of DOM elements</param>
        private static void Copy(SgmlPage page, XmlNode source, DomNode dest, bool handleXHTMLAsHTML)
        {
            XmlNodeList nodeChildren = source.ChildNodes;
            for (int i = 0; i < nodeChildren.Count; i++)
            {
                XmlNode child = nodeChildren.Item(i);
                switch (child.NodeType)
                {
                    case XmlNodeType.Element:
                        DomNode childXml = CreateFrom(page, child, handleXHTMLAsHTML);
                        dest.appendChild(childXml);
                        Copy(page, child, childXml, handleXHTMLAsHTML);
                        break;

                    case XmlNodeType.Text:
                        dest.appendChild(new DomText(page, child.Value));
                        break;

                    case XmlNodeType.CDATA:
                        dest.appendChild(new DomCDataSection(page, child.Value));
                        break;

                    case XmlNodeType.Comment:
                        dest.appendChild(new DomComment(page, child.Value));
                        break;

                    case XmlNodeType.ProcessingInstruction:
                        dest.appendChild(new DomProcessingInstruction(page, child.Name, child.Value));
                        break;

                    default:
                        LOG.Warn("NodeType " + child.NodeType + " (" + child.Name + ") is not yet supported.");
                }
            }
        }

        /// <summary>
        /// Search for the namespace URI of the given prefix, starting from the specified element.
        /// The default namespace can be searched for by specifying "" as the prefix.
        /// </summary>
        /// <param name="element">the element to start searching from</param>
        /// <param name="prefix">the namespace prefix</param>
        /// <returns>the namespace URI bound to the prefix; or null if there is no such namespace</returns>
        public static String LookupNamespaceURI(DomElement element, String prefix)
        {
            String uri = DomElement.ATTRIBUTE_NOT_DEFINED;
            if (String.IsNullOrEmpty(prefix))
            {
                uri = element.getAttribute("xmlns");
            }
            else
            {
                uri = element.getAttribute("xmlns:" + prefix);
            }
            if (uri == DomElement.ATTRIBUTE_NOT_DEFINED)
            {
                DomNode parentNode = element.getParentNode();
                if (parentNode is DomElement)
                {
                    uri = LookupNamespaceURI((DomElement)parentNode, prefix);
                }
            }
            return uri;
        }

        /// <summary>
        /// Search for the prefix associated with specified namespace URI.
        /// </summary>
        /// <param name="element">the element to start searching from</param>
        /// <param name="ns">the namespace prefix</param>
        /// <returns>the prefix bound to the namespace URI; or null if there is no such namespace</returns>
        public static String LookupPrefix(DomElement element, String ns)
        {
            Dictionary<String, DomAttr> attributes = element.getAttributesMap();
            foreach (KeyValuePair<String, DomAttr> entry in attributes)
            {
                String name = entry.Key;
                DomAttr value = entry.Value;
                if (name.StartsWith("xmlns:") && String.Equals(value.getValue(), ns))
                {
                    return name.Substring(6);
                }
            }
            foreach (DomNode child in element.getChildren())
            {
                if (child is DomElement)
                {
                    String prefix = LookupPrefix((DomElement)child, ns);
                    if (prefix != null)
                    {
                        return prefix;
                    }
                }
            }
            return null;
        }
    }
}