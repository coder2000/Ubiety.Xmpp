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

namespace Ubiety.Xmpp.Net
{
    public class Address
    {
        private readonly IConfiguration _config;
        private readonly Resolver _resolver;
        private int _srvAttempts;
        private IEnumerable<RecordSRV> _srvRecords = new List<RecordSRV>();

        public Address(IConfiguration config)
        {
            _resolver = new Resolver(Resolver.GetDnsServers())
            {
                UseCache = true,
                Timeout = TimeSpan.FromSeconds(5),
                TransportType = TransportType.Tcp
            };

            _config = config;
        }

        public string Hostname { get; set; }
        public bool IsIPv6 { get; set; }

        public IPEndPoint NextIpAddress()
        {
            IPAddress address;

            if (IPAddress.TryParse(Hostname, out address))
                return new IPEndPoint(address, _config.GetValue<int>("XmppConfiguration:DefaultPort"));

            ResolveSrv();

            if (_srvRecords.Any() && _srvAttempts <= _srvRecords.Count())
            {
                var port = _srvRecords.ElementAt(_srvAttempts).PORT;
                var target = _srvRecords.ElementAt(_srvAttempts).TARGET;

                address = Resolve(target);

                _srvAttempts++;

                return new IPEndPoint(address, port);
            }

            address = Resolve(Hostname);
            return new IPEndPoint(address, _config.GetValue<int>("XmppConfiguration:DefaultPort"));
        }

        private bool ResolveSrv()
        {
            var response = _resolver.Query("_xmpp-client._tcp." + Hostname, QType.SRV);

            if (response.Result.header.ANCOUNT <= 0) return false;

            _srvRecords = from record in response.Result.Answers
                where record.RECORD is RecordSRV
                orderby ((RecordSRV) record.RECORD).PRIORITY descending
                select record.RECORD as RecordSRV;

            return true;
        }

        private IPAddress Resolve(string hostname)
        {
            Response response;

            if (Socket.OSSupportsIPv6 && _config.GetValue<bool>("XmppConfiguration:UseIPv6"))
            {
                var r = _resolver.Query(hostname, QType.AAAA);
                response = r.Result;

                if (response.Answers.Count > 0)
                {
                    IsIPv6 = true;
                    return ((RecordAAAA) response.Answers[0].RECORD).Address;
                }
            }

            var r2 = _resolver.Query(hostname, QType.A);
            response = r2.Result;

            return response.Answers.Select(answer => answer.RECORD).OfType<RecordA>().Select(a => a.Address)
                .FirstOrDefault();
        }
    }
}