﻿// Copyright 2017 Dieter Lunn
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;

namespace Ubiety.Xmpp.Common
{
    /// <summary>
    /// Event args used to pass string data
    /// </summary>
    public class DataEventArgs : EventArgs
    {
        /// <summary>
        /// Create a new instance of DataEventArgs
        /// </summary>
        /// <param name="data">Data for the args to store</param>
        public DataEventArgs(string data)
        {
            Data = data;
        }
        
        /// <summary>
        /// Data property to hold the string
        /// </summary>
        public string Data { get; }
    }
}