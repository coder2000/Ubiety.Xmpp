// Copyright (c) Dieter Lunn. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.

using Stateless;
using Ubiety.Xmpp.Net;

namespace Ubiety.Xmpp.States
{
    /// <summary>
    ///     Manage the state of the library and protocol
    /// </summary>
    public class StateManager
    {
        private readonly StateMachine<State, StateTrigger> stateMachine;
        private readonly StateMachine<State, StateTrigger>.TriggerWithParameters<ISocket> connectTrigger;

        /// <summary>
        /// </summary>
        public StateManager()
        {
            stateMachine = new StateMachine<State, StateTrigger>(new DisconnectedState());

            stateMachine.Configure(new DisconnectState()).Permit(StateTrigger.Disconnect, new DisconnectedState());

            connectTrigger = stateMachine.SetTriggerParameters<ISocket>(StateTrigger.Connect);
            stateMachine.Configure(new DisconnectedState()).Permit(StateTrigger.Connect, new ConnectState());
            stateMachine.Configure(new ConnectState()).OnEntryFrom(connectTrigger, ConnectState.Connect);
        }

        /// <summary>
        /// </summary>
        public void Fire(StateTrigger triggers)
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