// Copyright (c) Dieter Lunn. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.

using System.IO;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Ubiety.Xmpp.Net;
using Ubiety.Xmpp.States;

namespace Ubiety.Xmpp
{
    /// <summary>
    ///     Main class
    /// </summary>
    public class XmppClient
    {
        private readonly IConfigurationBuilder configuration;
        private readonly StateManager stateManager;

        /// <summary>
        ///     Initializes a new instance of the <see cref="XmppClient" /> class
        /// </summary>
        public XmppClient()
        {
            this.configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("XmppConfig.json");

            this.stateManager = new StateManager();
        }

        /// <summary>
        ///     Gets the current version of the library
        /// </summary>
        public static string Version { get; }

        /// <summary>
        ///     Connect to a server
        /// </summary>
        public void Connect()
        {
            var socket = new AsyncSocket(this.configuration.Build());
            this.stateManager.FireConnect(socket);
        }
    }
}