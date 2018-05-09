// Copyright (c) Dieter Lunn. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.

using Stateless;
using Ubiety.Xmpp.Net;

namespace Ubiety.Xmpp.States
{
    /// <summary>
    ///     
    /// </summary>
    public class StateManager
    {
        private readonly StateMachine<State, StateTriggers> stateMachine;
        private readonly StateMachine<State, StateTriggers>.TriggerWithParameters<ISocket> connectTrigger;

        /// <summary>
        /// </summary>
        public StateManager()
        {
            stateMachine = new StateMachine<State, StateTriggers>(new DisconnectedState());

            stateMachine.Configure(new DisconnectState()).Permit(StateTriggers.Disconnect, new DisconnectedState());

            connectTrigger = stateMachine.SetTriggerParameters<ISocket>(StateTriggers.Connect);
            stateMachine.Configure(new DisconnectedState()).Permit(StateTriggers.Connect, new ConnectState());
            stateMachine.Configure(new ConnectState()).OnEntryFrom(connectTrigger, ConnectState.Connect);
        }

        /// <summary>
        /// </summary>
        public void Fire(StateTriggers triggers)
        {
            stateMachine.Fire(triggers);
        }

        /// <summary>
        /// </summary>
        public void FireConnect(ISocket socket)
        {
            stateMachine.Fire(connectTrigger, socket);
        }
    }
}