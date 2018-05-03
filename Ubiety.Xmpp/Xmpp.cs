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
    /// </summary>
    public class Xmpp
    {
        private readonly IConfigurationBuilder _configuration;
        private readonly StateManager _stateManager;

        /// <summary>
        /// </summary>
        public Xmpp()
        {
            _configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("XmppConfig.json");
            
            _stateManager = new StateManager();
        }

        /// <summary>
        /// </summary>
        public static string Version;
            
        /// <summary>
        /// </summary>
        public void Connect()
        {
            var socket = new AsyncSocket(_configuration.Build());
            _stateManager.FireConnect(socket);
        }
    }
}