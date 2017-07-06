using System;

namespace Ubiety.Xmpp.States
{
    public abstract class State : IEquatable<State>
    {
        private readonly string _name;

        protected State(string name)
        {
            _name = name;
        }
        
        public bool Equals(State other)
        {
            return this == other;
        }

        public override bool Equals(object obj)
        {
            return this == (State) obj;
        }

        public static bool operator ==(State x, State y)
        {
            return ReferenceEquals(x, y) || ((object) x != null && (object) y != null) && (x.GetType() == y.GetType());
        }
        public static bool operator !=(State x, State y)
        {
            return !(x == y);
        }

        public override int GetHashCode()
        {
            return _name.GetHashCode();
        }
    }
}