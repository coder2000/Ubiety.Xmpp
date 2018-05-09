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
        ///     Creates a new JID from a string representation
        /// </summary>
        /// <param name="xid">String form of a JID like "user@server.com/home"</param>
        public JID(string xid) : this()
        {
            XmppId = xid;
        }

        /// <summary>
        ///     Creates a new JID from its parts
        /// </summary>
        /// <param name="user">Username to be authenticated</param>
        /// <param name="server">Server address to lookup and connect to</param>
        /// <param name="resource">Resource to bind to - may be blank</param>
        public JID(string user, string server, string resource = "") : this()
        {
            User = user;
            Server = server;
            Resource = resource;
        }

        /// <summary>
        ///     Gets the username of the user.
        /// </summary>
        public string User
        {
            get => Unescape();
            private set
            {
                var tmp = Escape(value);
                user = Stringprep.NodePrep(tmp);
            }
        }

        /// <summary>
        ///     Gets the server the user is logged into.
        /// </summary>
        public string Server
        {
            get => server;
            private set => server = (value == null) ? null : Stringprep.NamePrep(value);
        }

        /// <summary>
        ///     Gets the resource the user is communicating from.
        /// </summary>
        public string Resource
        {
            get => resource;
            private set => resource = (value == null) ? null : Stringprep.ResourcePrep(value);
        }

        /// <summary>
        ///     String representation of the id.
        /// </summary>
        private string XmppId
        {
            get => xid ?? BuildJid();
            set => Parse(value);
        }

        /// <summary>
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(JID other)
        {
            return XmppId.Equals(other.XmppId);
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

                hash = hash*23 + User.GetHashCode();
                hash = hash*23 + Resource.GetHashCode();
                hash = hash*23 + Server.GetHashCode();

                return hash;
            }
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return XmppId;
        }

        /// <summary>
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (obj is string)
            {
                return XmppId.Equals(obj);
            }

            if (!(obj is JID))
            {
                return false;
            }

            return XmppId.Equals(((JID)obj).XmppId);
        }

        /// <summary>
        /// </summary>
        /// <param name="one"></param>
        /// <param name="two"></param>
        /// <returns></returns>
        public static bool operator ==(JID one, JID two)
        {
            return one.Equals(two);
        }

        /// <summary>
        /// </summary>
        /// <param name="one"></param>
        /// <param name="two"></param>
        /// <returns></returns>
        public static bool operator !=(JID one, JID two)
        {
            return !one.Equals(two);
        }

        /// <summary>
        /// </summary>
        /// <param name="one"></param>
        /// <returns></returns>
        public static implicit operator JID(string one)
        {
            return new JID(one);
        }

        /// <summary>
        /// </summary>
        /// <param name="one"></param>
        /// <returns></returns>
        public static implicit operator string(JID one)
        {
            return one.XmppId;
        }

        /// <summary>
        ///     Builds a string version of an XID from the three parts.
        /// </summary>
        /// <returns>string version of xid</returns>
        private string BuildJid()
        {
            var sb = new StringBuilder();
            if (user != null)
            {
                sb.Append(user);
                sb.Append("@");
            }

            sb.Append(server);
            if (resource != null)
            {
                sb.Append("/");
                sb.Append(resource);
            }

            xid = sb.ToString();
            return xid;
        }

        /// <summary>
        ///     Takes a string xid and breaks it into its parts.
        /// </summary>
        private void Parse(string id)
        {
            var at = id.IndexOf('@');
            var slash = id.IndexOf('/');

            if (at == -1)
            {
                if (slash == -1)
                {
                    Server = id;
                }
                else
                {
                    Server = id.Substring(0, slash);
                    Resource = id.Substring(slash + 1);
                }
            }
            else
            {
                if (slash == -1)
                {
                    User = id.Substring(0, at);
                    Server = id.Substring(at + 1);
                }
                else
                {
                    if (at < slash)
                    {
                        User = id.Substring(0, at);
                        Server = id.Substring(at + 1, slash - at - 1);
                        Resource = id.Substring(slash + 1);
                    }
                    else
                    {
                        Server = id.Substring(0, slash);
                        Resource = id.Substring(slash + 1);
                    }
                }
            }
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
                            throw new FormatException("Username cannot start or end with a space");
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

        private string Unescape()
        {
            var re = new Regex(@"\\([2-5][0267face])");
            var u = re.Replace(user, delegate(Match m)
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