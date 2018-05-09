// Copyright (c) Dieter Lunn. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.

using System;

namespace Ubiety.Xmpp.States
{
    /// <summary>
    ///     Base state type
    /// </summary>
    public abstract class State : IEquatable<State>
    {
        private readonly string name;

        /// <summary>
        ///     Construct a new state instance
        /// </summary>
        /// <param name="name">Name of the state</param>
        protected State(string name)
        {
            this.name = name;
        }

        /// <summary>
        ///     Are the states equal?
        /// </summary>
        /// <param name="x">First state</param>
        /// <param name="y">Second state</param>
        /// <returns>True if the states are equal</returns>
        public static bool operator ==(State x, State y)
        {
            return ReferenceEquals(x, y) || ((object)x != null && (object)y != null) && (x.GetType() == y.GetType());
        }

        /// <summary>
        ///     Are the states not equal?
        /// </summary>
        public static bool operator !=(State x, State y)
        {
            return !(x == y);
        }

        /// <summary>
        ///     Does this state equal another?
        /// </summary>
        /// <param name="other">State to compare the current instance too</param>
        /// <returns>
        ///     True if states are equal
        /// </returns>
        public bool Equals(State other)
        {
            return this == other;
        }

        /// <summary>
        /// </summary>
        public override bool Equals(object obj)
        {
            return this == (State)obj;
        }

        /// <summary>
        /// </summary>
        public override int GetHashCode()
        {
            return name.GetHashCode();
        }
    }
}