using System;
using System.IO;

namespace HtmlUnit.com.gargoylesoftware.htmlunit.xml
{
    ///
    internal class TrackBlankContentReader : TextReader
    {
        private StreamReader reader_;
        private bool wasBlank_ = true;

        public TrackBlankContentReader(StreamReader characterStream)
        {
            reader_ = characterStream;
        }

        public bool WasBlank
        {
            get
            {
                return wasBlank_;
            }
        }

        public override void Close() {
            reader_.Close();
        }

        public override int Read(char[] cbuf, int off, int len) 
        {
            int result = reader_.Read(cbuf, off, len);

            if (wasBlank_ && result > -1) {
                for (int i = 0; i < result; i++) {
                    char ch = cbuf[off + i];
                    if (!Char.IsWhiteSpace(ch)) {
                        wasBlank_ = false;
                        break;
                    }

                }
            }
            return result;
        }
    }
}
