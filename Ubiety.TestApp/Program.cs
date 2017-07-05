using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Ubiety.Xmpp.Net;
using Ubiety.Xmpp.States;

namespace Ubiety.TestApp
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            var manager = new StateManager();

            var configBuilder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("XmppConfig.json");
            
            var socket = new AsyncSocket(configBuilder.Build());
            manager.FireConnect(socket);
            Console.WriteLine("Hello World!");
        }
    }
}