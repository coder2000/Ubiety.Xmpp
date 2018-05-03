// Copyright (c) Dieter Lunn. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.


using System;
using Ubiety.Xmpp.Common;

namespace Ubiety.Xmpp.Net
{
    /// <summary>
    /// </summary>
    public interface ISocket
    {
        /// <summary>
        /// </summary>
        bool IsConnected { get; }
        /// <summary>
        /// </summary>
        void Connect(string hostname);
        /// <summary>
        /// </summary>
        void Connect(JID jid);
        /// <summary>
        /// </summary>
        void Disconnect();
        /// <summary>
        /// </summary>
        void Write(string message);
        /// <summary>
        /// </summary>
        void StartSsl();
        /// <summary>
        /// </summary>
        event EventHandler<DataEventArgs> Data;
        /// <summary>
        /// </summary>
        event EventHandler Connected;
    }
}