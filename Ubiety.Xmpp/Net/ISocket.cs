// Copyright (c) Dieter Lunn. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.

using System;
using Ubiety.Xmpp.Common;

namespace Ubiety.Xmpp.Net
{
    /// <summary>
    ///     Socket interface
    /// </summary>
    public interface ISocket
    {
        /// <summary>
        ///     Fires when a socket receives data
        /// </summary>
        event EventHandler<DataEventArgs> Data;

        /// <summary>
        ///     Fires when a socket is connected
        /// </summary>
        event EventHandler Connected;

        /// <summary>
        ///     Gets a value if a socket is connected or not
        /// </summary>
        bool IsConnected { get; }

        /// <summary>
        ///     Connects a socket to a server
        /// </summary>
        /// <param name="hostname">Hostname of the server to connect to</param>
        void Connect(string hostname);

        /// <summary>
        ///     Connects a socket to a server
        /// </summary>
        /// <param name="jid">Jabber ID of the user to connect to</param>
        void Connect(JID jid);

        /// <summary>
        ///     Disconnects a socket from a server
        /// </summary>
        void Disconnect();

        /// <summary>
        ///     Write a message to a connected server
        /// </summary>
        /// <param name="message">Message to send to the server</param>
        void Write(string message);

        /// <summary>
        ///     Start SSL encryption on the socket
        /// </summary>
        void StartSsl();
    }
}