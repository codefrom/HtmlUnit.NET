using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HtmlUnit.com.gargoylesoftware.htmlunit
{
    public sealed class WebWindowEvent/* : EventObject*/
    {
        private readonly IPage oldPage_;
        private readonly IPage newPage_;
        private readonly int type_;

        /** A window has opened. */
        public const int OPEN = 1;

        /** A window has closed. */
        public const int CLOSE = 2;

        /** The content of the window has changed. */
        public const int CHANGE = 3;

        /**
         * Creates an instance.
         *
         * @param webWindow the WebWindow that caused the event
         * @param type the type - one of {@link #OPEN}, {@link #CLOSE} or {@link #CHANGE}
         * @param oldPage the old contents of the web window
         * @param newPage the new contents of the web window
         */
        public WebWindowEvent(
                IWebWindow webWindow,
                int type,
                IPage oldPage,
                IPage newPage) :
            base(webWindow)
        {
            oldPage_ = oldPage;
            newPage_ = newPage;

            switch (type)
            {
                case OPEN:
                case CLOSE:
                case CHANGE:
                    type_ = type;
                    break;

                default:
                    throw new ArgumentException("type must be one of OPEN, CLOSE, CHANGE but got " + type);
            }
        }

        /**
         * Returns true if the two objects are equal.
         *
         * @param object the object to compare against
         * @return true if the two objects are equal
         */
        public override bool Equals(Object obj)
        {
            if (null == obj)
            {
                return false;
            }
            if (GetType() == obj.GetType())
            {
                WebWindowEvent ev = (WebWindowEvent)obj;
                return IsEqual(Source, ev.Source)
                    && EventType == ev.EventType
                    && IsEqual(OldPage, ev.OldPage)
                    && IsEqual(NewPage, ev.NewPage);
            }
            return false;
        }

        /**
         * Returns the hash code for this object.
         * @return the hash code for this object
         */
        public override int HashCode()
        {
            return Source.hashCode();
        }

        /**
         * Returns the oldPage.
         * @return the page or null if the window has no page
         */
        public IPage OldPage
        {
            get
            {
                return oldPage_;
            }
        }

        /**
         * Returns the oldPage.
         * @return the page or null if the window has no page
         */
        public IPage NewPage
        {
            get
            {
                return newPage_;
            }
        }

        /**
         * Returns the web window that fired the event.
         * @return the web window that fired the event
         */
        public IWebWindow WebWindow
        {
            get
            {
                return (IWebWindow)Source;
            }
        }

        private bool IsEqual(Object object1, Object object2)
        {
            bool result;

            if (object1 == null && object2 == null)
            {
                result = true;
            }
            else if (object1 == null || object2 == null)
            {
                result = false;
            }
            else
            {
                result = object1.Equals(object2);
            }

            return result;
        }

        /**
         * Returns a string representation of this event.
         * @return a string representation of this event
         */
        public override String ToString()
        {
            StringBuilder buffer = new StringBuilder(80);
            buffer.Append("WebWindowEvent(source=[");
            buffer.Append(Source);
            buffer.Append("] type=[");
            switch (type_)
            {
                case OPEN:
                    buffer.Append("OPEN");
                    break;
                case CLOSE:
                    buffer.Append("CLOSE");
                    break;
                case CHANGE:
                    buffer.Append("CHANGE");
                    break;
                default:
                    buffer.Append(type_);
                    break;
            }
            buffer.Append("] oldPage=[");
            buffer.Append(OldPage);
            buffer.Append("] newPage=[");
            buffer.Append(NewPage);
            buffer.Append("])");

            return buffer.ToString();
        }

        /** @return the event type */
        public int EventType
        {
            get
            {
                return type_;
            }
        }
    }
}