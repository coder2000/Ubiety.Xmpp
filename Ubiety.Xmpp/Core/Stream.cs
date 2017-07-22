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

using System.Xml.Linq;
using Ubiety.Xmpp.Attributes;
using Ubiety.Xmpp.Common;

namespace Ubiety.Xmpp.Core
{
    [XmppTag(Namespaces.Stream, "stream", typeof(Stream))]
    public class Stream : Tag
    {
        public Stream() : base(XName.Get("stream", Namespaces.Stream))
        {
        }

        public Stream(XElement element) : base(element)
        {
        }

        public string From
        {
            get => GetAttributeValue("from");
            set => SetAttributeValue("from", value);
        }

        public string To
        {
            get => GetAttributeValue("to");
            set => SetAttributeValue("to", value);
        }

        public string Lang
        {
            get => GetAttributeValue(XName.Get("lang", Namespaces.Xml));
            set => SetAttributeValue(XName.Get("lang", Namespaces.Xml), value);
        }
    }
}