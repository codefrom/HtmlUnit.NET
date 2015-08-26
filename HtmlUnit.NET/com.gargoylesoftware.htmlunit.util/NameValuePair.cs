using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HtmlUnit.com.gargoylesoftware.htmlunit.util
{
    [Serializable]
    public class NameValuePair
    {

    /** The name. */
    private readonly String name_;

    /** The value. */
    private readonly String value_;

    /**
     * Creates a new instance.
     * @param name the name
     * @param value the value
     */
    public NameValuePair(String name, String value) {
        name_ = name;
        value_ = value;
    }

    /**
     * Returns the name.
     * @return the name
     */
    public String Name {
        get
        {
            return name_;
        }
    }

    /**
     * Returns the value.
     * @return the value
     */
    public String Value 
    {
        get
        {
            return value_;
        }
    }

    /**
     * {@inheritDoc}
     */
    public override bool Equals(object obj)
    {
        if (!(obj is NameValuePair)) {
            return false;
        }
        NameValuePair other = (NameValuePair) object;
        return LangUtils.equals(name_, other.name_) && LangUtils.equals(value_, other.value_);
    }

    /**
     * {@inheritDoc}
     */
    public override int GetHashCode()
    {
        int hash = LangUtils.HASH_SEED;
        hash = LangUtils.hashCode(hash, name_);
        hash = LangUtils.hashCode(hash, value_);
        return hash;
    }

    /**
     * {@inheritDoc}
     */
    public override string ToString()
    {
        return name_ + "=" + value_;
    }

    /**
     * Converts the specified name/value pairs into HttpClient name/value pairs.
     * @param pairs the name/value pairs to convert
     * @return the converted name/value pairs
     */
    public static org.apache.http.NameValuePair[] toHttpClient(NameValuePair[] pairs) {
        org.apache.http.NameValuePair[] pairs2 =
            new org.apache.http.NameValuePair[pairs.length];
        for (int i = 0; i < pairs.length; i++) {
            NameValuePair pair = pairs[i];
            pairs2[i] = new BasicNameValuePair(pair.getName(), pair.getValue());
        }
        return pairs2;
    }

    /**
     * Converts the specified name/value pairs into HttpClient name/value pairs.
     * @param pairs the name/value pairs to convert
     * @return the converted name/value pairs
     */
    public static org.apache.http.NameValuePair[] toHttpClient(List<NameValuePair> pairs) {
        org.apache.http.NameValuePair[] pairs2 = new org.apache.http.NameValuePair[pairs.size()];
        for (int i = 0; i < pairs.size(); i++) {
            NameValuePair pair = pairs.get(i);
            pairs2[i] = new BasicNameValuePair(pair.getName(), pair.getValue());
        }
        return pairs2;
    }
    }
}
