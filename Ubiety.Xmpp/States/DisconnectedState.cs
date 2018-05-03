// Copyright (c) Dieter Lunn. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.

namespace Ubiety.Xmpp.States
{
    /// <summary>
    ///     Disconnected and inital state of XMPP
    /// </summary>
    public class DisconnectedState : State
    {
        /// <summary>
        ///     Create a new instance of the disconnected state
        /// </summary>
        public DisconnectedState() : base("Disconnected") {}
    }
}