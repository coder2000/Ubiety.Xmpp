//Copyright 2017 Dieter Lunn
//
//Licensed under the Apache License, Version 2.0 (the "License");
//you may not use this file except in compliance with the License.
//You may obtain a copy of the License at
//
//http://www.apache.org/licenses/LICENSE-2.0
//
//Unless required by applicable law or agreed to in writing, software
//distributed under the License is distributed on an "AS IS" BASIS,
//WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//See the License for the specific language governing permissions and
//limitations under the License.

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