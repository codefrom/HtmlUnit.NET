using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HtmlUnit.com.gargoylesoftware.htmlunit.util
{
    public class Attribute
    {
        private String name_;
        private String value_;
        private int updatedIndex_;

        public String Name
        {
            get
            {
                return name_;
            }
        }

        public String Value
        {
            get
            {
                return value_;
            }
        }

        public int UpdatedIndex
        {
            get
            {
                return updatedIndex_;
            }
        }
        
        public Attribute(String name, String value, int updatedIndex)
        {
            name_ = name;
            value_ = value;
            updatedIndex_ = updatedIndex;
        }
    }
}
