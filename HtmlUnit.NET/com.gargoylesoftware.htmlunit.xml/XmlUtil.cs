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

    /**
     * Default encoding used.
     */
    public static const String DEFAULT_CHARSET = "UTF-8";

    private static readonly Log LOG = LogFactory.GetLog(typeof(XmlUtil));

    private static readonly ErrorHandler DISCARD_MESSAGES_HANDLER = new ErrorHandler();
    {
        /// <summary>
        /// Does nothing as we're not interested in this.
        /// </summary>
        /// <param name="exception"></param>
        public void Error(SAXParseException exception) {
            // Does nothing as we're not interested in this.
        }
        
        /// <summary>
        /// Does nothing as we're not interested in this.
        /// </summary>
        /// <param name="exception"></param>
        public void FatalError(SAXParseException exception) {
            // Does nothing as we're not interested in this.
        }
        
        /// <summary>
        /// Does nothing as we're not interested in this.
        /// </summary>
        /// <param name="exception"></param>
        public void Warning(SAXParseException exception) {
            // Does nothing as we're not interested in this.
        }
    };

    /// <summary>
    /// Utility class, hide constructor.
    /// </summary>
    private XmlUtil() {
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
        DocumentBuilderFactory factory = DocumentBuilderFactory.NewInstance();

        if (webResponse == null) {
            return factory.NewDocumentBuilder().NewDocument();
        }

        factory.SetNamespaceAware(true);
        InputStreamReader reader = new InputStreamReader(webResponse.GetContentAsStream(), Encoding.GetEncoding(webResponse.GetContentCharset());

        // we have to do the blank input check and the parsing in one step
        TrackBlankContentReader tracker = new TrackBlankContentReader(reader);

        InputSource source = new InputSource(tracker);
        DocumentBuilder builder = factory.newDocumentBuilder();
        builder.setErrorHandler(DISCARD_MESSAGES_HANDLER);
        builder.setEntityResolver(new EntityResolver() {
            public override InputSource resolveEntity(String publicId, String systemId)
                throws SAXException, IOException {
                return new InputSource(new StringReader(""));
            }});
        try {
            return builder.parse(source);
        }
        catch (SAXException e) {
            if (tracker.wasBlank()) {
                return factory.newDocumentBuilder().newDocument();
            }
            throw e;
        }
    }

    ///
    private static class TrackBlankContentReader extends Reader {
        private Reader reader_;
        private boolean wasBlank_ = true;

        public TrackBlankContentReader(Reader characterStream) {
            reader_ = characterStream;
        }

        public boolean wasBlank() {
            return wasBlank_;
        }

        @Override
        public void close() throws IOException {
            reader_.close();
        }

        @Override
        public int read(char[] cbuf, int off, int len) throws IOException {
            int result = reader_.read(cbuf, off, len);

            if (wasBlank_ && result > -1) {
                for (int i = 0; i < result; i++) {
                    char ch = cbuf[off + i];
                    if (!Character.isWhitespace(ch)) {
                        wasBlank_ = false;
                        break;
                    }

                }
            }
            return result;
        }
    }

    /**
     * Recursively appends a {@link Node} child to {@link DomNode} parent.
     *
     * @param page the owner page of {@link DomElement}s to be created
     * @param parent the parent DomNode
     * @param child the child Node
     * @param handleXHTMLAsHTML if true elements from the XHTML namespace are handled as HTML elements instead of
     *     DOM elements
     */
    public static void appendChild(SgmlPage page, DomNode parent, Node child,
        boolean handleXHTMLAsHTML) {
        DocumentType documentType = child.getOwnerDocument().getDoctype();
        if (documentType != null && page instanceof XmlPage) {
            DomDocumentType domDoctype = new DomDocumentType(
                    page, documentType.getName(), documentType.getPublicId(), documentType.getSystemId());
            ((XmlPage) page).setDocumentType(domDoctype);
        }
        DomNode childXml = createFrom(page, child, handleXHTMLAsHTML);
        parent.appendChild(childXml);
        copy(page, child, childXml, handleXHTMLAsHTML);
    }

    private static DomNode createFrom(SgmlPage page, Node source, boolean handleXHTMLAsHTML) {
        if (source.getNodeType() == Node.TEXT_NODE) {
            return new DomText(page, source.getNodeValue());
        }
        if (source.getNodeType() == Node.PROCESSING_INSTRUCTION_NODE) {
            return new DomProcessingInstruction(page, source.getNodeName(), source.getNodeValue());
        }
        if (source.getNodeType() == Node.COMMENT_NODE) {
            return new DomComment(page, source.getNodeValue());
        }
        if (source.getNodeType() == Node.DOCUMENT_TYPE_NODE) {
            DocumentType documentType = (DocumentType) source;
            return new DomDocumentType(page, documentType.getName(), documentType.getPublicId(),
                    documentType.getSystemId());
        }
        String ns = source.getNamespaceURI();
        String localName = source.getLocalName();
        if (handleXHTMLAsHTML && HTMLParser.XHTML_NAMESPACE.equals(ns)) {
            ElementFactory factory = HTMLParser.getFactory(localName);
            return factory.createElementNS(page, ns, localName, namedNodeMapToSaxAttributes(source.getAttributes()));
        }
        NamedNodeMap nodeAttributes = source.getAttributes();
        if (page != null && page.isHtmlPage()) {
            localName = localName.toUpperCase(Locale.ENGLISH);
        }
        String qualifiedName;
        if (source.getPrefix() == null) {
            qualifiedName = localName;
        }
        else {
            qualifiedName = source.getPrefix() + ':' + localName;
        }

        String namespaceURI = source.getNamespaceURI();
        if (HTMLParser.SVG_NAMESPACE.equals(namespaceURI)) {
            return HTMLParser.SVG_FACTORY.createElementNS(page, namespaceURI, qualifiedName,
                    namedNodeMapToSaxAttributes(nodeAttributes));
        }

        Map<String, DomAttr> attributes = new LinkedHashMap<>();
        for (int i = 0; i < nodeAttributes.getLength(); i++) {
            Attr attribute = (Attr) nodeAttributes.item(i);
            String attributeNamespaceURI = attribute.getNamespaceURI();
            String attributeQualifiedName;
            if (attribute.getPrefix() != null) {
                attributeQualifiedName = attribute.getPrefix() + ':' + attribute.getLocalName();
            }
            else {
                attributeQualifiedName = attribute.getLocalName();
            }
            String value = attribute.getNodeValue();
            boolean specified = attribute.getSpecified();
            DomAttr xmlAttribute =
                    new DomAttr(page, attributeNamespaceURI, attributeQualifiedName, value, specified);
            attributes.put(attribute.getNodeName(), xmlAttribute);
        }
        return new DomElement(namespaceURI, qualifiedName, page, attributes);
    }

    private static Attributes namedNodeMapToSaxAttributes(NamedNodeMap attributesMap) {
        AttributesImpl attributes = new AttributesImpl();
        int length = attributesMap.getLength();
        for (int i = 0; i < length; ++i) {
            Node attr = attributesMap.item(i);
            attributes.addAttribute(attr.getNamespaceURI(), attr.getLocalName(),
                attr.getNodeName(), null, attr.getNodeValue());
        }

        return attributes;
    }

    /**
     * Copy all children from 'source' to 'dest', within the context of the specified page.
     * @param page the page which the nodes belong to
     * @param source the node to copy from
     * @param dest the node to copy to
     * @param handleXHTMLAsHTML if true elements from the XHTML namespace are handled as HTML elements instead of
     *     DOM elements
     */
    private static void copy(SgmlPage page, Node source, DomNode dest,
        boolean handleXHTMLAsHTML) {
        NodeList nodeChildren = source.getChildNodes();
        for (int i = 0; i < nodeChildren.getLength(); i++) {
            Node child = nodeChildren.item(i);
            switch (child.getNodeType()) {
                case Node.ELEMENT_NODE:
                    DomNode childXml = createFrom(page, child, handleXHTMLAsHTML);
                    dest.appendChild(childXml);
                    copy(page, child, childXml, handleXHTMLAsHTML);
                    break;

                case Node.TEXT_NODE:
                    dest.appendChild(new DomText(page, child.getNodeValue()));
                    break;

                case Node.CDATA_SECTION_NODE:
                    dest.appendChild(new DomCDataSection(page, child.getNodeValue()));
                    break;

                case Node.COMMENT_NODE:
                    dest.appendChild(new DomComment(page, child.getNodeValue()));
                    break;

                case Node.PROCESSING_INSTRUCTION_NODE:
                    dest.appendChild(new DomProcessingInstruction(page, child.getNodeName(), child.getNodeValue()));
                    break;

                default:
                    LOG.warn("NodeType " + child.getNodeType()
                        + " (" + child.getNodeName() + ") is not yet supported.");
            }
        }
    }

    /**
     * Search for the namespace URI of the given prefix, starting from the specified element.
     * The default namespace can be searched for by specifying "" as the prefix.
     * @param element the element to start searching from
     * @param prefix the namespace prefix
     * @return the namespace URI bound to the prefix; or null if there is no such namespace
     */
    public static String lookupNamespaceURI(DomElement element, String prefix) {
        String uri = DomElement.ATTRIBUTE_NOT_DEFINED;
        if (prefix.isEmpty()) {
            uri = element.getAttribute("xmlns");
        }
        else {
            uri = element.getAttribute("xmlns:" + prefix);
        }
        if (uri == DomElement.ATTRIBUTE_NOT_DEFINED) {
            DomNode parentNode = element.getParentNode();
            if (parentNode instanceof DomElement) {
                uri = lookupNamespaceURI((DomElement) parentNode, prefix);
            }
        }
        return uri;
    }

    /**
     * Search for the prefix associated with specified namespace URI.
     * @param element the element to start searching from
     * @param namespace the namespace prefix
     * @return the prefix bound to the namespace URI; or null if there is no such namespace
     */
    public static String lookupPrefix(DomElement element, String namespace) {
        Map<String, DomAttr> attributes = element.getAttributesMap();
        for (Map.Entry<String, DomAttr> entry : attributes.entrySet()) {
            String name = entry.getKey();
            DomAttr value = entry.getValue();
            if (name.startsWith("xmlns:") && value.getValue().equals(namespace)) {
                return name.substring(6);
            }
        }
        for (DomNode child : element.getChildren()) {
            if (child instanceof DomElement) {
                String prefix = lookupPrefix((DomElement) child, namespace);
                if (prefix != null) {
                    return prefix;
                }
            }
        }
        return null;
    }
    }
}
