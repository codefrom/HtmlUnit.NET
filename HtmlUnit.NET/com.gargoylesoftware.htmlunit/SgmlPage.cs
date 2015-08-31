using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using HtmlUnit.Helpers;

namespace HtmlUnit.com.gargoylesoftware.htmlunit
{
    public abstract class SgmlPage : DomNode, IPage
    {
    private DocumentType documentType_;
    private readonly WebResponse webResponse_;
    private IWebWindow enclosingWindow_;
    private readonly WebClient webClient_;

     * Creates an instance of SgmlPage.
     *
     * @param webResponse the web response that was used to create this page
     * @param webWindow the window that this page is being loaded into
    public SgmlPage(WebResponse webResponse, IWebWindow webWindow):
        base(null)
    {
        webResponse_ = webResponse;
        enclosingWindow_ = webWindow;
        webClient_ = webWindow.WebClient;
    }

    /**
     * {@inheritDoc}
     */
    public void CleanUp() {
        if (WebClient.Cache.GetCachedResponse(webResponse_.WebRequest) == null) {
            webResponse_.CleanUp();
        }
    }

    /**
     * {@inheritDoc}
     */
    public WebResponse WebResponse 
    {
        get
        {
            return webResponse_;
        }
    }

    /**
     * {@inheritDoc}
     */
    public void Initialize()
    {
        // nothing to do here
    }

    /**
     * Gets the name for the current node.
     * @return the node name
     */
    public String NodeName 
    {
        get
        {
            return "#document";
        }
    }

    /**
     * Gets the type of the current node.
     * @return the node type
     */
    public short NodeType {
        get
        {
            return org.w3c.dom.Node.DOCUMENT_NODE;
        }
    }

    /**
     * Returns the window that this page is sitting inside.
     *
     * @return the enclosing frame or null if this page isn't inside a frame
     */
    public IWebWindow EnclosingWindow 
    {
        get
        {
            return enclosingWindow_;
        }
        set
        {
            enclosingWindow_ = value;
        }
    }

    /**
     * Returns the WebClient that originally loaded this page.
     *
     * @return the WebClient that originally loaded this page
     */
    public WebClient WebClient {
        get
        {
            return webClient_;
        }
    }

    /**
     * Creates an empty {@link DomDocumentFragment} object.
     * @return a newly created {@link DomDocumentFragment}
     */
    [Obsolete("as of 2.18, please use {@link #createDocumentFragment()} instead")]
    public DomDocumentFragment CreateDomDocumentFragment() {
        return CreateDocumentFragment();
    }

    /**
     * Creates an empty {@link DomDocumentFragment} object.
     * @return a newly created {@link DomDocumentFragment}
     */
    public DomDocumentFragment CreateDocumentFragment() {
        return new DomDocumentFragment(this);
    }

    /**
     * Returns the document type.
     * @return the document type
     */
    public DocumentType Doctype {
        get
        {
            return documentType_;
        }
        set
        {
            documentType_ = value;
        }
    }

    /**
     * {@inheritDoc}
     */
    public SgmlPage Page {
        get
        {
            return this;
        }
    }

    /**
     * Creates an element, the type of which depends on the specified tag name.
     * @param tagName the tag name which determines the type of element to be created
     * @return an element, the type of which depends on the specified tag name
     */
    public abstract XmlElement CreateElement(String tagName);

    /**
     * Create a new Element with the given namespace and qualified name.
     * @param namespaceURI the URI that identifies an XML namespace
     * @param qualifiedName the qualified name of the element type to instantiate
     * @return the new element
     */
    public abstract XmlElement CreateElementNS(String namespaceURI, String qualifiedName);

    /**
     * Returns the page encoding.
     * @return the page encoding
     */
    public abstract String PageEncoding { get; }

    /**
     * Returns the document element.
     * @return the document element
     */
    public DomElement GetDocumentElement() {
        DomNode childNode = getFirstChild();
        while (childNode != null && !(childNode is DomElement)) {
            childNode = childNode.getNextSibling();
        }
        return (DomElement) childNode;
    }

    /**
     * Creates a clone of this instance.
     * @return a clone of this instance
     */
    protected SgmlPage Clone() {
        try {
            SgmlPage result = (SgmlPage) base.Clone();
            return result;
        }
        catch (CloneNotSupportedException e) {
            throw new IllegalStateException("Clone not supported");
        }
    }

    /**
     * {@inheritDoc}
     */
    public String AsXml() {
        DomElement documentElement = GetDocumentElement();
        if (documentElement == null) {
            return "";
        }
        return documentElement.AsXml();
    }

    /**
     * Returns <tt>true</tt> if this page has case-sensitive tag names, <tt>false</tt> otherwise. In general,
     * XML has case-sensitive tag names, and HTML doesn't. This is especially important during XPath matching.
     * @return <tt>true</tt> if this page has case-sensitive tag names, <tt>false</tt> otherwise
     */
    public abstract bool HasCaseSensitiveTagNames();

    /**
     * {@inheritDoc}
     * The current implementation just {@link DomNode#normalize()}s the document element.
     */
    public void NormalizeDocument() {
        GetDocumentElement().Normalize();
    }

    /**
     * {@inheritDoc}
     */
    public String GetCanonicalXPath() {
        return "/";
    }

    /**
     * {@inheritDoc}
     */
    public DomAttr CreateAttribute(String name) 
    {
        return new DomAttr(Page, null, name, "", false);
    }

    /**
     * Returns the URL of this page.
     * @return the URL of this page
     */
    public URL Url 
    {
        get
        {
            return WebResponse.WebRequest.Url;
        }
    }

    public bool IsHtmlPage() 
    {
        return false;
    }

    /**
     * {@inheritDoc}
     */
    public override DomNodeList<DomElement> GetElementsByTagName(String tagName)
    {
/*        return new AbstractDomNodeList<DomElement>(this) {
            protected List<DomElement> ProvideElements() {
                List<DomElement> res = new LinkedList<>();
                foreach (DomElement elem in GetDomElementDescendants()) {
                    if (String.Equals(elem.LocalName, tagName)) 
                    {
                        res.add(elem);
                    }
                }
                return res;
            }
        };*/
    }

    /**
     * {@inheritDoc}
     */
    public CDATASection CreateCDATASection(String dat) {
        return new DomCDataSection(this, dat);
    }

    /**
     * {@inheritDoc}
     */
    public override Text createTextNode(final String data) {
        return new DomText(this, data);
    }

    /**
     * {@inheritDoc}
     */
    @Override
    public Comment createComment(final String data) {
        return new DomComment(this, data);
    }
    }
}
