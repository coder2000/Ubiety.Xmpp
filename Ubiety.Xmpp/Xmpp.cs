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