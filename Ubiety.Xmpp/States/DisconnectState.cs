﻿// Copyright (c) Dieter Lunn. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.

namespace Ubiety.Xmpp.States
{
    /// <summary>
    ///     Disconnect starts the process of shutting down
    /// </summary>
    public class DisconnectState : State
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="DisconnectState" /> class
        /// </summary>
        public DisconnectState()
            : base("Disconnect")
        {
        }
    }
}