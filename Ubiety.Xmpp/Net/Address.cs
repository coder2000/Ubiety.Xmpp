// Copyright (c) Dieter Lunn. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.

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
    /// <summary>
    /// </summary>
    public sealed class Address
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly IConfiguration _config;
        private readonly Resolver _resolver;
        private int _srvAttempts;
        private IEnumerable<RecordSRV> _srvRecords = new List<RecordSRV>();

        /// <summary>
        /// </summary>
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

        /// <summary>
        /// </summary>
        public string Hostname { private get; set; }
        /// <summary>
        /// </summary>
        public bool IsIPv6 { get; private set; }

        /// <summary>
        /// </summary>
        public IPEndPoint NextIpAddress()
        {
            IPAddress address;

            if (IPAddress.TryParse(Hostname, out address))
                return new IPEndPoint(address, _config.GetValue<int>("XmppConfiguration:DefaultPort"));

            var srvAvailable = false;
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

            if (!string.IsNullOrEmpty(response.Error))
            {
                Logger.Error($"DNS Error: {response.Error}");
                throw new Exception(response.Error);
            }

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