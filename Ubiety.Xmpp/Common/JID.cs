// Copyright (c) Dieter Lunn. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.

using System;
using System.Text;
using System.Text.RegularExpressions;
using Gnu.Inet.Encoding;

namespace Ubiety.Xmpp.Common
{
    /// <summary>
    ///     Jabber ID implementation
    /// </summary>
    public struct JID : IEquatable<JID>
    {
        private string resource;
        private string server;
        private string user;
        private string xid;

        /// <summary>
        ///     Initializes a new instance of the <see cref="JID" /> class.
        /// </summary>
        /// <param name="xid">String form of a JID like "user@server.com/home"</param>
        public JID(string xid)
            : this()
        {
            this.XmppId = xid;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="JID" /> class.
        /// </summary>
        /// <param name="user">Username to be authenticated</param>
        /// <param name="server">Server address to lookup and connect to</param>
        /// <param name="resource">Resource to bind to - may be blank</param>
        public JID(string user, string server, string resource = "")
            : this()
        {
            this.User = user;
            this.Server = server;
            this.Resource = resource;
        }

        /// <summary>
        ///     Gets the username of the user.
        /// </summary>
        public string User
        {
            get => this.Unescape();
            private set
            {
                var tmp = Escape(value);
                this.user = Stringprep.NodePrep(tmp);
            }
        }

        /// <summary>
        ///     Gets the server the user is logged into.
        /// </summary>
        public string Server
        {
            get => this.server;
            private set => this.server = (value == null) ? null : Stringprep.NamePrep(value);
        }

        /// <summary>
        ///     Gets the resource the user is communicating from.
        /// </summary>
        public string Resource
        {
            get => this.resource;
            private set => this.resource = (value == null) ? null : Stringprep.ResourcePrep(value);
        }

        /// <summary>
        ///     Gets or sets the string representation of the id.
        /// </summary>
        private string XmppId
        {
            get => this.xid ?? this.BuildJid();
            set => this.Parse(value);
        }

        /// <summary>
        ///     Implicitly converts a string to a <see cref="JID" />
        /// </summary>
        /// <param name="jid">String to convert</param>
        /// <returns>Jabber ID version of the string</returns>
        public static implicit operator JID(string jid)
        {
            return new JID(jid);
        }

        /// <summary>
        ///     Implicitly converts a <see cref="JID" /> to a string
        /// </summary>
        /// <param name="jid">JID to convert</param>
        /// <returns>String version of the JID</returns>
        public static implicit operator string(JID jid)
        {
            return jid.XmppId;
        }

        /// <summary>
        ///     Does one JID equal another
        /// </summary>
        /// <param name="one">First JID</param>
        /// <param name="two">Second JID</param>
        /// <returns>True if the JIDs are equal</returns>
        public static bool operator ==(JID one, JID two)
        {
            return one.Equals(two);
        }

        /// <summary>
        ///     Are the two JIDs not equal
        /// </summary>
        /// <param name="one">First JID</param>
        /// <param name="two">Second JID</param>
        /// <returns>True if the JIDs are not equal</returns>
        public static bool operator !=(JID one, JID two)
        {
            return !one.Equals(two);
        }

        /// <summary>
        ///     Convert a string into a <see cref="JID" />
        /// </summary>
        /// <param name="jid">String to convert to a JID</param>
        /// <returns>JID version of the string</returns>
        public static JID ToJID(string jid)
        {
            return new JID(jid);
        }

        /// <summary>
        ///     Convert a <see cref="JID" /> to a string
        /// </summary>
        /// <param name="jid">JID to convert to a string</param>
        /// <returns>String version of the JID</returns>
        public static string FromJID(JID jid)
        {
            return jid.XmppId;
        }

        /// <summary>
        ///     This this JID equal another
        /// </summary>
        /// <param name="other">JID to compare equality to</param>
        /// <returns>True if the JIDs are equal</returns>
        public bool Equals(JID other)
        {
            return this.XmppId.Equals(other.XmppId, StringComparison.InvariantCulture);
        }

        /// <summary>
        ///     Unique hash for an object to be used as a key in dictionaries etc...
        /// </summary>
        /// <returns>Hash code based on Jid parts</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                var hash = 17;

                hash = (hash * 23) + this.User.GetHashCode();
                hash = hash * (23 + this.Resource.GetHashCode());
                hash = (hash * 23) + this.Server.GetHashCode();

                return hash;
            }
        }

        /// <summary>
        ///     Gets a string representation of the class
        /// </summary>
        /// <returns>String version of the JID</returns>
        public override string ToString()
        {
            return this.XmppId;
        }

        /// <summary>
        ///     Does one JID equal another
        /// </summary>
        /// <param name="obj">JID to compare the current instance to</param>
        /// <returns>True if the JIDs are equal</returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (obj is string)
            {
                return this.XmppId.Equals(obj);
            }

            if (!(obj is JID))
            {
                return false;
            }

            return this.XmppId.Equals(((JID)obj).XmppId, StringComparison.InvariantCulture);
        }

        private static string Escape(string user)
        {
            var u = new StringBuilder();
            var count = 0;

            foreach (var c in user)
            {
                switch (c)
                {
                    case ' ':
                        if ((count == 0) || (count == (user.Length - 1)))
                        {
                            throw new FormatException("Username cannot start or end with a space");
                        }

                        u.Append(@"\20");
                        break;
                    case '"':
                        u.Append(@"\22");
                        break;
                    case '&':
                        u.Append(@"\26");
                        break;
                    case '\'':
                        u.Append(@"\27");
                        break;
                    case '/':
                        u.Append(@"\2f");
                        break;
                    case ':':
                        u.Append(@"\3a");
                        break;
                    case '<':
                        u.Append(@"\3c");
                        break;
                    case '>':
                        u.Append(@"\3e");
                        break;
                    case '@':
                        u.Append(@"\40");
                        break;
                    case '\\':
                        u.Append(@"\5c");
                        break;
                    default:
                        u.Append(c);
                        break;
                }

                count++;
            }

            return u.ToString();
        }

        /// <summary>
        ///     Builds a string version of an XID from the three parts.
        /// </summary>
        /// <returns>string version of xid</returns>
        private string BuildJid()
        {
            var sb = new StringBuilder();
            if (this.user != null)
            {
                sb.Append(this.user);
                sb.Append("@");
            }

            sb.Append(this.server);
            if (this.resource != null)
            {
                sb.Append("/");
                sb.Append(this.resource);
            }

            this.xid = sb.ToString();
            return this.xid;
        }

        /// <summary>
        ///     Takes a string xid and breaks it into its parts.
        /// </summary>
        /// <param name="id">JID to convert to string parts</param>
        private void Parse(string id)
        {
            var at = id.IndexOf('@');
            var slash = id.IndexOf('/');

            if (at == -1)
            {
                if (slash == -1)
                {
                    this.Server = id;
                }
                else
                {
                    this.Server = id.Substring(0, slash);
                    this.Resource = id.Substring(slash + 1);
                }
            }
            else
            {
                if (slash == -1)
                {
                    this.User = id.Substring(0, at);
                    this.Server = id.Substring(at + 1);
                }
                else
                {
                    if (at < slash)
                    {
                        this.User = id.Substring(0, at);
                        this.Server = id.Substring(at + 1, slash - at - 1);
                        this.Resource = id.Substring(slash + 1);
                    }
                    else
                    {
                        this.Server = id.Substring(0, slash);
                        this.Resource = id.Substring(slash + 1);
                    }
                }
            }
        }

        private string Unescape()
        {
            var re = new Regex(@"\\([2-5][0267face])");
            var u = re.Replace(this.user, (Match m) =>
            {
                switch (m.Groups[1].Value)
                {
                    case "20":
                        return " ";
                    case "22":
                        return "\"";
                    case "26":
                        return "&";
                    case "27":
                        return "'";
                    case "2f":
                        return "/";
                    case "3a":
                        return ":";
                    case "3c":
                        return "<";
                    case "3e":
                        return ">";
                    case "40":
                        return "@";
                    case "5c":
                        return @"\";
                    default:
                        return m.Groups[0].Value;
                }
            });

            return u;
        }
    }
}