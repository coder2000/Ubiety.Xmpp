using Ubiety.Xmpp.Net;

namespace Ubiety.Xmpp.States
{
    /// <summary>
    /// </summary>
    public class ConnectState : State
    {
        /// <summary>
        /// </summary>
        public ConnectState() : base("Connect") {}

        /// <summary>
        /// </summary>
        public static void Connect(ISocket socket)
        {
            socket.Connect("dieterlunn.ca");
        }
    }
}