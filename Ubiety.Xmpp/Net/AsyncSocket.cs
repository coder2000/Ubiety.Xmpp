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
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Ubiety.Xmpp.Common;

namespace Ubiety.Xmpp.Net
{
    public class AsyncSocket : ISocket, IDisposable
    {
        private readonly Address _address;
        private IConfiguration _configuration;
        private CancellationTokenSource _cts;
        private Socket _socket;
        private Stream _stream;

        public AsyncSocket(IConfiguration configuration)
        {
            _configuration = configuration;
            _address = new Address(configuration);
        }

        public void Dispose()
        {
            _stream.Dispose();
            _socket.Dispose();
        }

        public bool IsConnected { get; private set; }

        public void Connect(string hostname)
        {
            _address.Hostname = hostname;
            var address = _address.NextIpAddress();

            _socket = _address.IsIPv6
                ? new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp)
                : new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            var args = new SocketAsyncEventArgs {RemoteEndPoint = address};
            args.Completed += ConnectCompleted;

            _socket.ConnectAsync(args);
        }

        public void Connect(JID jid)
        {
            Connect(jid.Server);
        }

        public void Disconnect()
        {
            _socket.Shutdown(SocketShutdown.Both);
        }

        public void Write(string message)
        {
            throw new NotImplementedException();
        }

        public void StartSsl()
        {
            throw new NotImplementedException();
        }

        public event EventHandler<DataEventArgs> Data;
        public event EventHandler Connected;

        protected virtual void OnData(DataEventArgs args)
        {
            Data?.Invoke(this, args);
        }

        protected virtual void OnConnected()
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

                var args = new DataEventArgs(data);
                OnData(args);

                var text = await ReadData(_cts.Token);
                data = text;
            }
        }
    }
}