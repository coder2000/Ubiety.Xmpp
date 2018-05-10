namespace Ubiety.TestApp
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            var xmpp = new Xmpp.XmppClient();
            xmpp.Connect();
        }
    }
}