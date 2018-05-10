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
        ///     Initializes a new instance of the <see cref="StateManager" /> class
        /// </summary>
        public StateManager()
        {
            this.stateMachine = new StateMachine<State, StateTrigger>(new DisconnectedState());

            this.stateMachine.Configure(new DisconnectState())
                .Permit(StateTrigger.Disconnect, new DisconnectedState());

            this.stateMachine.Configure(new DisconnectedState())
                .Permit(StateTrigger.Connect, new ConnectState());

            this.connectTrigger = this.stateMachine.SetTriggerParameters<ISocket>(StateTrigger.Connect);
            this.stateMachine.Configure(new ConnectState())
                .OnEntryFrom(this.connectTrigger, ConnectState.Connect);
        }

        /// <summary>
        ///     Change the current state to the desired state
        /// </summary>
        /// <param name="trigger">State to trigger the change to</param>
        public void ChangeState(StateTrigger trigger)
        {
            this.stateMachine.Fire(trigger);
        }

        /// <summary>
        ///     Change to the connect state
        /// </summary>
        /// <param name="socket">Socket used to make the connection</param>
        public void FireConnect(ISocket socket)
        {
            this.stateMachine.Fire(this.connectTrigger, socket);
        }
    }
}