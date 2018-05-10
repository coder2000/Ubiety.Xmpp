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
        ///     Initializes a new instance of the <see cref="ConnectState" /> class
        /// </summary>
        public ConnectState()
            : base("Connect")
        {
        }

        /// <summary>
        ///     Initiate connection to the server
        /// </summary>
        /// <param name="socket">Socket to connect to</param>
        public static void Connect(ISocket socket)
        {
            socket.Connect("dieterlunn.ca");
        }
    }
}