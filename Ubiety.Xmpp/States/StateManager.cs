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

using Stateless;
using Ubiety.Xmpp.Net;

namespace Ubiety.Xmpp.States
{
    public class StateManager
    {
        private readonly StateMachine<State, StateTriggers> _stateMachine;
        private readonly StateMachine<State, StateTriggers>.TriggerWithParameters<ISocket> _connectTrigger;
        
        public StateManager()
        {
            _stateMachine = new StateMachine<State, StateTriggers>(new DisconnectedState());

            _stateMachine.Configure(new DisconnectState()).Permit(StateTriggers.Disconnect, new DisconnectedState());

            _connectTrigger = _stateMachine.SetTriggerParameters<ISocket>(StateTriggers.Connect);
            _stateMachine.Configure(new DisconnectedState()).Permit(StateTriggers.Connect, new ConnectState());
            _stateMachine.Configure(new ConnectState()).OnEntryFrom(_connectTrigger, ConnectState.Connect);
        }

        public void Fire(StateTriggers triggers)
        {
            _stateMachine.Fire(triggers);
        }

        public void FireConnect(ISocket socket)
        {
            _stateMachine.Fire(_connectTrigger, socket);
        }
    }
}