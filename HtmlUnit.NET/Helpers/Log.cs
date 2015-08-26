/*
 * Copyright (c) 2015 3D
 *
 * C# fork v0.1
 * 
 */

using System;

namespace HtmlUnit.Helpers
{
    public class Log
    {
        private readonly Type _type;

        public Log()
        {
            _type = typeof (Log);
        }

        public Log(Type type)
        {
            _type = type;
        }

        public void Warn(Exception exc)
        {
        }

        public void Warn(string message)
        {
        }

        public void Warn(string message, Exception exc)
        { 
        }

        public bool IsDebugEnabled { get; set; }

        public void Debug(string message)
        { 
        }
    }
}
