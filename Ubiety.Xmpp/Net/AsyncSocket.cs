// Copyright (c) Dieter Lunn. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.


using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Ubiety.Xmpp.Common;

namespace Ubiety.Xmpp.Net
{
    /// <summary>
    /// </summary>
    public class AsyncSocket : ISocket, IDisposable
    {
        private readonly Address _address;
        private readonly IConfiguration _configuration;
        private CancellationTokenSource _cts;
        private Socket _socket;
        private Stream _stream;

        /// <summary>
        /// </summary>
        public AsyncSocket(IConfiguration configuration)
        {
            _configuration = configuration;
            _address = new Address(configuration);
        }

        /// <summary>
        /// </summary>
        public void Dispose()
        {
            _stream.Dispose();
            _socket.Dispose();
        }

        /// <summary>
        /// </summary>
        public bool IsConnected { get; private set; }

        /// <summary>
        /// </summary>
        public void Connect(string hostname)
        {
            if (hostname == null)
            {
                throw new ArgumentNullException(nameof(hostname));
            }
            
            _address.Hostname = hostname;
            var address = _address.NextIpAddress();

            _socket = _address.IsIPv6
                ? new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp)
                : new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            var args = new SocketAsyncEventArgs {RemoteEndPoint = address};
            args.Completed += ConnectCompleted;

            _socket.ConnectAsync(args);
        }

        /// <summary>
        /// </summary>
        public void Connect(JID jid)
        {
            if (jid == null)
            {
                throw new ArgumentNullException(nameof(jid));
            }
            
            Connect(jid.Server);
        }

        /// <summary>
        /// </summary>
        public void Disconnect()
        {
            IsConnected = false;
            _socket.Shutdown(SocketShutdown.Both);
        }

        /// <summary>
        /// </summary>
        public void Write(string message)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// </summary>
        public void StartSsl()
        {
            var useSsl = _configuration.GetValue<bool>("XmppConfiguration:UseSSL");
        }

        /// <summary>
        /// </summary>
        public event EventHandler<DataEventArgs> Data;
        /// <summary>
        /// </summary>
        public event EventHandler Connected;

        private void OnData(string data)
        {
            var args = new DataEventArgs(data);
            Data?.Invoke(this, args);
        }

        private void OnConnected()
        {
            Connected?.Invoke(this, new EventArgs());
        }

        private async void ConnectCompleted(object sender, SocketAsyncEventArgs e)
        {
            IsConnected = true;
            OnConnected();

            _cts = new CancellationTokenSource(5000);

            _stream = new NetworkStream(e.ConnectSocket);
            var data = await ReadData(_cts.Token);
            await ReadDataContinuous(data);
        }

        private async Task<string> ReadData(CancellationToken token)
        {
            var buffer = new byte[4096];
            var result = await _stream.ReadAsync(buffer, 0, buffer.Length, token);
            Array.Resize(ref buffer, result);
            return Encoding.ASCII.GetString(buffer);
        }

        private async Task ReadDataContinuous(string data)
        {
            while (true)
            {
                if (IsConnected == false)
                    break;

                OnData(data);

                var text = await ReadData(_cts.Token);
                data = text;
            }
        }
    }
}