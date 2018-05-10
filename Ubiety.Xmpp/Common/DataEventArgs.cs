// Copyright (c) Dieter Lunn. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.

using System;

namespace Ubiety.Xmpp.Common
{
    /// <summary>
    ///     Event args used to pass string data
    /// </summary>
    public class DataEventArgs : EventArgs
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="DataEventArgs" /> class
        /// </summary>
        /// <param name="data">Data for the args to store</param>
        public DataEventArgs(string data)
        {
            this.Data = data;
        }

        /// <summary>
        ///     Gets the current data
        /// </summary>
        public string Data { get; }
    }
}