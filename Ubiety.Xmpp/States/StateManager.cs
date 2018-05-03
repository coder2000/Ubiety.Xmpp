using Stateless;
using Ubiety.Xmpp.Net;

namespace Ubiety.Xmpp.States
{
    /// <summary>
    /// </summary>
    public class StateManager
    {
        private readonly StateMachine<State, StateTriggers> _stateMachine;
        private readonly StateMachine<State, StateTriggers>.TriggerWithParameters<ISocket> _connectTrigger;
        
        /// <summary>
        /// </summary>
        public StateManager()
        {
            _stateMachine = new StateMachine<State, StateTriggers>(new DisconnectedState());

            _stateMachine.Configure(new DisconnectState()).Permit(StateTriggers.Disconnect, new DisconnectedState());

            _connectTrigger = _stateMachine.SetTriggerParameters<ISocket>(StateTriggers.Connect);
            _stateMachine.Configure(new DisconnectedState()).Permit(StateTriggers.Connect, new ConnectState());
            _stateMachine.Configure(new ConnectState()).OnEntryFrom(_connectTrigger, ConnectState.Connect);
        }

        /// <summary>
        /// </summary>
        public void Fire(StateTriggers triggers)
        {
            _stateMachine.Fire(triggers);
        }

        /// <summary>
        /// </summary>
        public void FireConnect(ISocket socket)
        {
            _stateMachine.Fire(_connectTrigger, socket);
        }
    }
}