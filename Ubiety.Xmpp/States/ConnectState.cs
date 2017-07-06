using Ubiety.Xmpp.Net;

namespace Ubiety.Xmpp.States
{
    public class ConnectState : State
    {
        public ConnectState() : base("Connect") {}

        public static void Connect(ISocket socket)
        {
            socket.Connect("dieterlunn.ca");
        }
    }
}