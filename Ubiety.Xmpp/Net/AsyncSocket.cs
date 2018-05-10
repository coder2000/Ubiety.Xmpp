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
    ///     Protocol socket for asynchronous connections
    /// </summary>
    public sealed class AsyncSocket : ISocket, IDisposable
    {
        private readonly Address address;
        private readonly IConfiguration configuration;
        private CancellationTokenSource cts;
        private Socket socket;
        private Stream stream;

        /// <summary>
        ///     Initializes a new instance of the <see cref="AsyncSocket" /> class
        /// </summary>
        /// <param name="configuration">Library configuration</param>
        public AsyncSocket(IConfiguration configuration)
        {
            this.configuration = configuration;
            this.address = new Address(configuration);
        }

        /// <summary>
        ///     Fires when new data is available from the socket
        /// </summary>
        public event EventHandler<DataEventArgs> Data;

        /// <summary>
        ///     Fires when the socket is connected
        /// </summary>
        public event EventHandler Connected;

        /// <summary>
        ///     Gets a value whether the socket is connected or not
        /// </summary>
        public bool IsConnected { get; private set; }

        /// <summary>
        ///     Connect the socket to a server
        /// </summary>
        /// <param name="hostname">Server hostname to connect to</param>
        public void Connect(string hostname)
        {
            if (hostname == null)
            {
                throw new ArgumentNullException(nameof(hostname));
            }

            this.address.Hostname = hostname;
            var address = this.address.NextIpAddress();

            this.socket = this.address.IsIPv6
                ? new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp)
                : new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            var args = new SocketAsyncEventArgs { RemoteEndPoint = address };
            args.Completed += ConnectCompleted;

            this.socket.ConnectAsync(args);
        }

        /// <summary>
        ///     Connect the socket to a server
        /// </summary>
        /// <param name="jid">Jabber user id to connect to</param>
        public void Connect(JID jid)
        {
            if (jid == null)
            {
                throw new ArgumentNullException(nameof(jid));
            }

            Connect(jid.Server);
        }

        /// <summary>
        ///     Disconnect the socket from the server
        /// </summary>
        public void Disconnect()
        {
            IsConnected = false;
            this.socket.Shutdown(SocketShutdown.Both);
        }

        /// <summary>
        ///     Disposes the current instance
        /// </summary>
        public void Dispose()
        {
            if (IsConnected)
            {
                Disconnect();
            }

            this.stream.Dispose();
            this.socket.Dispose();

            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     Write a string message to the server
        /// </summary>
        /// <param name="message">String to send to the server</param>
        public void Write(string message)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Start SSL encryption of the socket
        /// </summary>
        public void StartSsl()
        {
            var useSsl = this.configuration.GetValue<bool>("XmppConfiguration:UseSSL");
        }

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

            this.cts = new CancellationTokenSource(5000);

            this.stream = new NetworkStream(e.ConnectSocket);
            var data = await ReadData(this.cts.Token);
            await ReadDataContinuous(data);
        }

        private async Task<string> ReadData(CancellationToken token)
        {
            var buffer = new byte[4096];
            var result = await this.stream.ReadAsync(buffer, 0, buffer.Length, token);
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

                var text = await ReadData(this.cts.Token);
                data = text;
            }
        }
    }
}