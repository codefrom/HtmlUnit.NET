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
using System.Text.RegularExpressions;

namespace HtmlUnit.com.gargoylesoftware.htmlunit
{
    /// <summary>
    /// Class which centralizes proxy configuration, in an effort to reduce clutter in the {@link WebClient}
    /// class. One instance of this class exists for each <tt>WebClient</tt> instance.
    /// @version $Revision: 9868 $
    /// @author Daniel Gredler
    /// @see WebClientOptions#getProxyConfig()
    /// </summary>
    [Serializable]
    public class ProxyConfig
    {
        private String proxyHost_;
        private int proxyPort_;
        private bool isSocksProxy_;
        private readonly Dictionary<String, Regex> proxyBypassHosts_ = new Dictionary<String, Regex>();
        private String proxyAutoConfigUrl_;
        private String proxyAutoConfigContent_;

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public ProxyConfig() :
            this(null, 0)
        {
        }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="proxyHost">the proxy host</param>
        /// <param name="proxyPort">the proxy port</param>
        public ProxyConfig(String proxyHost, int proxyPort) :
            this(proxyHost, proxyPort, false)
        {
        }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="proxyHost">the proxy host</param>
        /// <param name="proxyPort">the proxy port</param>
        /// <param name="isSocks">whether SOCKS proxy or not</param>
        public ProxyConfig(String proxyHost, int proxyPort, bool isSocks)
        {
            proxyHost_ = proxyHost;
            proxyPort_ = proxyPort;
            isSocksProxy_ = isSocks;
        }

        /// <summary>
        /// the proxy host used to perform HTTP requests.
        /// </summary>
        public String ProxyHost
        {
            get
            {
                return proxyHost_;
            }
            set
            {
                proxyHost_ = value;
            }
        }


        /// <summary>
        /// the proxy port used to perform HTTP requests.
        /// </summary>
        public int ProxyPort
        {
            get
            {
                return proxyPort_;
            }
            set
            {
                proxyPort_ = value;
            }
        }

        /// <summary>
        /// whether SOCKS proxy or not.
        /// </summary>
        public bool IsSocksProxy
        {
            get
            {
                return isSocksProxy_;
            }
            set
            {
                isSocksProxy_ = value;
            }
        }

        /// <summary>
        /// Any hosts matched by the specified regular expression pattern will bypass the configured proxy.
        /// @see Pattern
        /// </summary>
        /// <param name="pattern">a regular expression pattern that matches the hostnames of the hosts which should bypass the configured proxy.</param>
        public void AddHostsToProxyBypass(String pattern)
        {
            proxyBypassHosts_.Add(pattern, new Regex(pattern));
        }

        /// <summary>
        /// Any hosts matched by the specified regular expression pattern will no longer bypass the configured proxy.
        /// @see Pattern
        /// </summary>
        /// <param name="pattern">the previously added regular expression pattern</param>
        public void removeHostsFromProxyBypass(String pattern)
        {
            proxyBypassHosts_.Remove(pattern);
        }

        /// <summary>
        /// Returns <tt>true</tt> if the host with the specified hostname should be accessed bypassing the
        /// configured proxy.
        /// </summary>
        /// <param name="hostname">the name of the host to check</param>
        /// <returns><tt>true</tt> if the host with the specified hostname should be accessed bypassing the configured proxy, <tt>false</tt> otherwise.</returns>
        protected bool ShouldBypassProxy(String hostname)
        {
            bool bypass = false;
            foreach (Regex p in proxyBypassHosts_.Values)
            {
                if (p.Match(hostname).Success)
                {
                    bypass = true;
                    break;
                }
            }
            return bypass;
        }

        /// <summary>
        /// the proxy auto-config URL.
        /// </summary>
        public String ProxyAutoConfigUrl
        {
            get
            {
                return proxyAutoConfigUrl_;
            }
            set
            {
                proxyAutoConfigUrl_ = value;
                ProxyAutoConfigContent = null;
            }
        }

        /// <summary>
        /// the proxy auto-config content.
        /// </summary>
        protected String ProxyAutoConfigContent
        {
            get
            {
                return proxyAutoConfigContent_;
            }
            set
            {
                proxyAutoConfigContent_ = value;
            }
        }
    }
}