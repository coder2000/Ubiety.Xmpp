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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using Heijden.Dns.Portable;
using Heijden.DNS;
using Microsoft.Extensions.Configuration;
using NLog;

namespace Ubiety.Xmpp.Net
{
    public sealed class Address
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly IConfiguration _config;
        private readonly Resolver _resolver;
        private int _srvAttempts;
        private IEnumerable<RecordSRV> _srvRecords = new List<RecordSRV>();

        public Address(IConfiguration config)
        {
            _config = config;

            Logger.Debug("DNS Servers:");
            foreach (var dnsServer in GetDnsServers())
                Logger.Debug(dnsServer.ToString());

            _resolver = new Resolver(GetDnsServers())
            {
                UseCache = true,
                Timeout = TimeSpan.FromSeconds(5),
                TransportType = TransportType.Tcp
            };

            _resolver.OnVerbose += (sender, args) => { Logger.Debug($"DNS verbose message: {args.Message}"); };
        }

        public string Hostname { private get; set; }
        public bool IsIPv6 { get; private set; }

        public IPEndPoint NextIpAddress()
        {
            IPAddress address;

            if (IPAddress.TryParse(Hostname, out address))
                return new IPEndPoint(address, _config.GetValue<int>("XmppConfiguration:DefaultPort"));

            bool srvAvailable;
            if (!_srvRecords.Any())
                srvAvailable = ResolveSrv();

            if (srvAvailable || (_srvRecords.Any() && _srvAttempts <= _srvRecords.Count()))
            {
                Logger.Debug("Found SRV record...");
                var port = _srvRecords.ElementAt(_srvAttempts).PORT;
                var target = _srvRecords.ElementAt(_srvAttempts).TARGET;

                Logger.Debug($"Resolving {target}...");
                address = Resolve(target);

                _srvAttempts++;

                Logger.Debug($"Found address {address}...");

                return new IPEndPoint(address, port);
            }

            Logger.Debug($"No SRV. Resolving {Hostname}...");
            address = Resolve(Hostname);
            Logger.Debug($"Found address {address}...");
            return new IPEndPoint(address, _config.GetValue<int>("XmppConfiguration:DefaultPort"));
        }

        private bool ResolveSrv()
        {
            Logger.Debug("Resolving SRV records...");
            var response = _resolver.Query($"_xmpp-client._tcp.{Hostname}", QType.SRV).Result;

            if (response.header.ANCOUNT <= 0)
            {
                Logger.Info("No SRV results");
                return false;
            }

            _srvRecords = response.RecordsSRV.OrderBy(record => record.PRIORITY)
                .ThenByDescending(record => record.WEIGHT);

            return true;
        }

        private IPAddress Resolve(string hostname)
        {
            Response response;

            if (Socket.OSSupportsIPv6 && _config.GetValue<bool>("XmppConfiguration:UseIPv6"))
            {
                Logger.Debug("Resolving IPv6 address...");
                response = _resolver.Query(hostname, QType.AAAA).Result;

                if (response.RecordsAAAA.Length > 0)
                {
                    IsIPv6 = true;
                    return response.RecordsAAAA.FirstOrDefault().Address;
                }
            }

            Logger.Debug("Resolving IPv4 address...");
            response = _resolver.Query(hostname, QType.A).Result;

            return response.RecordsA.FirstOrDefault().Address;
        }

        private IPEndPoint[] GetDnsServers()
        {
            var addresses = new List<string>();
            for (var i = 0; i < 4; i++)
            {
                var server = _config.GetValue<string>($"XmppConfiguration:DnsServers:{i}");
                if (server == null) continue;
                addresses.Add(server);
            }

            return addresses.Select(IPAddress.Parse).Select(ip => new IPEndPoint(ip, 53)).ToArray();
        }
    }
}