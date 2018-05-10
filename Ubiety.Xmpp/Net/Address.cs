// Copyright (c) Dieter Lunn. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using Heijden.DNS;
using Heijden.Dns.Portable;
using Microsoft.Extensions.Configuration;
using NLog;

namespace Ubiety.Xmpp.Net
{
    /// <summary>
    ///     Network address for an XMPP server
    /// </summary>
    public sealed class Address
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly IConfiguration config;
        private readonly Resolver resolver;
        private int srvAttempts;
        private IEnumerable<RecordSRV> srvRecords = new List<RecordSRV>();

        /// <summary>
        ///     Initializes a new instance of the <see cref="Address" /> class
        /// </summary>
        /// <param name="config">Library configuration</param>
        public Address(IConfiguration config)
        {
            this.config = config;

            Logger.Debug("DNS Servers:");
            foreach (var dnsServer in GetDnsServers())
                Logger.Debug(dnsServer.ToString());

            this.resolver = new Resolver(GetDnsServers())
            {
                UseCache = true,
                Timeout = TimeSpan.FromSeconds(5),
                TransportType = TransportType.Tcp
            };

            this.resolver.OnVerbose += (sender, args) => { Logger.Debug($"DNS verbose message: {args.Message}"); };
        }

        /// <summary>
        ///     Gets or sets the server hostname
        /// </summary>
        public string Hostname { get; set; }

        /// <summary>
        ///     Gets a value whether the address is IPv6 or not
        /// </summary>
        public bool IsIPv6 { get; private set; }

        /// <summary>
        ///     Gets the next ip address for the server
        /// </summary>
        /// <returns><see cref="IPEndPoint" /> of the server from dns</returns>
        public IPEndPoint NextIpAddress()
        {
            IPAddress address;

            if (IPAddress.TryParse(Hostname, out address))
                return new IPEndPoint(address, this.config.GetValue<int>("XmppConfiguration:DefaultPort"));

            var srvAvailable = false;
            if (!this.srvRecords.Any())
                srvAvailable = ResolveSrv();

            if (srvAvailable || (this.srvRecords.Any() && this.srvAttempts <= this.srvRecords.Count()))
            {
                Logger.Debug("Found SRV record...");
                var port = this.srvRecords.ElementAt(this.srvAttempts).PORT;
                var target = this.srvRecords.ElementAt(this.srvAttempts).TARGET;

                Logger.Debug($"Resolving {target}...");
                address = Resolve(target);

                this.srvAttempts++;

                Logger.Debug($"Found address {address}...");

                return new IPEndPoint(address, port);
            }

            Logger.Debug($"No SRV. Resolving {Hostname}...");
            address = Resolve(Hostname);
            Logger.Debug($"Found address {address}...");
            return new IPEndPoint(address, this.config.GetValue<int>("XmppConfiguration:DefaultPort"));
        }

        private bool ResolveSrv()
        {
            Logger.Debug("Resolving SRV records...");
            var response = this.resolver.Query($"_xmpp-client._tcp.{Hostname}", QType.SRV).Result;

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

            this.srvRecords = response.RecordsSRV.OrderBy(record => record.PRIORITY)
                .ThenByDescending(record => record.WEIGHT);

            return true;
        }

        private IPAddress Resolve(string hostname)
        {
            Response response;

            if (Socket.OSSupportsIPv6 && this.config.GetValue<bool>("XmppConfiguration:UseIPv6"))
            {
                Logger.Debug("Resolving IPv6 address...");
                response = this.resolver.Query(hostname, QType.AAAA).Result;

                if (response.RecordsAAAA.Length > 0)
                {
                    IsIPv6 = true;
                    return response.RecordsAAAA.FirstOrDefault().Address;
                }
            }

            Logger.Debug("Resolving IPv4 address...");
            response = this.resolver.Query(hostname, QType.A).Result;

            return response.RecordsA.FirstOrDefault().Address;
        }

        private IPEndPoint[] GetDnsServers()
        {
            var addresses = new List<string>();
            for (var i = 0; i < 4; i++)
            {
                var server = this.config.GetValue<string>($"XmppConfiguration:DnsServers:{i}");
                if (server == null) continue;
                addresses.Add(server);
            }

            return addresses.Select(IPAddress.Parse).Select(ip => new IPEndPoint(ip, 53)).ToArray();
        }
    }
}