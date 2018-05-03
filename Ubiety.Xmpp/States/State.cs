// Copyright (c) Dieter Lunn. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.

using System;

namespace Ubiety.Xmpp.States
{
    /// <summary>
    /// </summary>
    public abstract class State : IEquatable<State>
    {
        private readonly string _name;

        /// <summary>
        /// </summary>
        protected State(string name)
        {
            _name = name;
        }
        
        /// <summary>
        /// </summary>
        public bool Equals(State other)
        {
            return this == other;
        }

        /// <summary>
        /// </summary>
        public override bool Equals(object obj)
        {
            return this == (State) obj;
        }

        /// <summary>
        /// </summary>
        public static bool operator ==(State x, State y)
        {
            return ReferenceEquals(x, y) || ((object) x != null && (object) y != null) && (x.GetType() == y.GetType());
        }

        /// <summary>
        /// </summary>
        public static bool operator !=(State x, State y)
        {
            return !(x == y);
        }

        /// <summary>
        /// </summary>
        public override int GetHashCode()
        {
            return _name.GetHashCode();
        }
    }
}