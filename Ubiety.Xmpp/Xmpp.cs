using System.IO;
using Microsoft.Extensions.Configuration;
using Ubiety.Xmpp.Net;
using Ubiety.Xmpp.States;

namespace Ubiety.Xmpp
{
    public class Xmpp
    {
        private readonly IConfigurationBuilder _configuration;
        private readonly StateManager _stateManager;

        public Xmpp()
        {
            _configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("XmppConfig.json");
            
            _stateManager = new StateManager();
        }

        public void Connect()
        {
            var socket = new AsyncSocket(_configuration.Build());
            _stateManager.FireConnect(socket);
        }
    }
}