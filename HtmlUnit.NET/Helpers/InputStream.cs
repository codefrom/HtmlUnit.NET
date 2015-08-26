/*
 * Copyright (c) 2015 3D
 *
 * C# fork v0.1
 * 
 */
using System.IO;

namespace HtmlUnit.Helpers
{
    public class InputStream : Stream
    {
        private long _resetPosition;
        private int _markReadLimit;

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public bool MarkSupported
        {
            get
            {
                return this.CanSeek;
            }
        }

        public void Mark(int readlimit)
        {
            _resetPosition = Position;
        }

        public void Reset()
        {
            if (!MarkSupported) throw new IOException("Mark is not supported");

            // if readed more then mark - reset to begining
            if (_markReadLimit > Position - _resetPosition)
                Seek(0, SeekOrigin.Begin);
            else
                Seek(_resetPosition, SeekOrigin.Begin);
        }
    }
}
