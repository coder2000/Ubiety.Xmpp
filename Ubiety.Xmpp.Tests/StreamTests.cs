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
using System.Xml.Linq;
using Ubiety.Xmpp.Common;
using Ubiety.Xmpp.Core;
using Xunit;

namespace Ubiety.Xmpp.Tests
{
    public class StreamTests
    {
        [Fact]
        public void NewTag()
        {
            var stream1 = new XElement(XName.Get("stream", Namespaces.Stream));
            var stream2 = new Stream();

            Assert.True(XNode.DeepEquals(stream1, stream2));
        }

        [Fact]
        public void CopyTag()
        {
            var stream1 = new XElement(XName.Get("stream", Namespaces.Stream));
            var copy = new Stream(stream1);
            var stream2 = new Stream();

            Assert.True(XNode.DeepEquals(copy, stream2));
        }
    }
}