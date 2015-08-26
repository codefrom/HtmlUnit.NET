/*
 * Copyright (c) 2015 3D
 *
 * C# fork v0.1
 * 
 */

using System;
using System.Collections.Generic;

namespace HtmlUnit.Helpers
{
    /// <summary>
    /// Log4j mock
    /// </summary>
    public static class LogFactory
    {
        /// <summary>
        /// Dictionary of logs
        /// </summary>
        //private static Dictionary<Type, Log> Logs = new Dictionary<Type, Log>();

        /// <summary>
        /// Returns Log from dictionary
        /// </summary>
        /// <param name="type">Type of log</param>
        /// <returns>Log for given type</returns>
        public static Log GetLog(Type type)
        {
            /*if (!Logs.ContainsKey(type))
                Logs.Add(type, new Log(type));
            return Logs[type];*/
            return new Log(type);
        }
    }
}
