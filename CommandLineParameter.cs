using System;
using System.Threading.Tasks;

namespace CloudCopy
{
    class TargetDescription
    {
            string _Username = "";
            string _Hostname = "";
            string _Collection = "";
            string _Identifier = "";

        public TargetDescription()
        {
        }

        public TargetDescription(string username, string hostname, string collection, string identifier)
        {
            Username = username;
            Hostname = hostname;
            Collection = collection;
            Identifier = identifier;
        }

        public string Username { get => _Username; set => _Username = value; }
        public string Hostname { get => _Hostname; set => _Hostname = value; }
        public string Collection { get => _Collection; set => _Collection = value; }
        public string Identifier { get => _Identifier; set => _Identifier = value; }
    }
}

