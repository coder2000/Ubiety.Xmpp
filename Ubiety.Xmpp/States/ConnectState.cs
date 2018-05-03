// Copyright (c) Dieter Lunn. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.

using Ubiety.Xmpp.Net;

namespace Ubiety.Xmpp.States
{
    /// <summary>
    /// </summary>
    public class ConnectState : State
    {
        /// <summary>
        /// </summary>
        public ConnectState() : base("Connect") {}

        /// <summary>
        /// </summary>
        public static void Connect(ISocket socket)
        {
            socket.Connect("dieterlunn.ca");
        }
    }
}