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
using Ubiety.Xmpp.Common;

namespace Ubiety.Xmpp.Net
{
    public interface ISocket
    {
        bool IsConnected { get; }
        void Connect(string hostname);
        void Connect(JID jid);
        void Disconnect();
        void Write(string message);
        void StartSsl();
        event EventHandler<DataEventArgs> Data;
        event EventHandler Connected;
    }
}