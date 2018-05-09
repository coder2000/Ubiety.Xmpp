// Copyright (c) Dieter Lunn. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.

using Ubiety.Xmpp.Net;

namespace Ubiety.Xmpp.States
{
    /// <summary>
    ///     Connect state of the protocol
    /// </summary>
    public class ConnectState : State
    {
        /// <summary>
        ///     Create a new instance of the Connect state
        /// </summary>
        public ConnectState() : base("Connect") { }

        /// <summary>
        ///     Initiate connection to the server
        /// </summary>
        public static void Connect(ISocket socket)
        {
            socket.Connect("dieterlunn.ca");
        }
    }
}