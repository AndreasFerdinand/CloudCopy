using System;

namespace CloudCopy
{
    public class TargetDescription
    {
        private string username = string.Empty;
        private string hostname = string.Empty;
        private string collection = string.Empty;
        private string identifier = string.Empty;

        public TargetDescription(string targetDescription)
        {
            /*
            // [User]@[Hostname]:<Collection>:<Identifier>
            // <Identifier> :== #<ID> | <UUID>
            // admin@my123456.crm.ondemand.com:ServiceRequestCollection:bb0812c2b4174491bca22734aa33c6be
            // @my123456.crm.ondemand.com:ServiceRequestCollection:#8311
            // @:ServiceRequestCollection:#4431
            */

            string tempTargetDescription;
            string[] targetChunks = targetDescription.Split('@');

            if (targetChunks.Length == 1)
            {
                tempTargetDescription = targetChunks[0];
            }
            else if (targetChunks.Length == 2)
            {
                this.Username = targetChunks[0];
                tempTargetDescription = targetChunks[1];
            }
            else
            {
                throw new Exception("Cannot parse target description.");
            }

            targetChunks = tempTargetDescription.Split(':');

            if (targetChunks.Length == 2)
            {
                this.Collection = targetChunks[0];
                this.Identifier = targetChunks[1];
            }
            else if (targetChunks.Length == 3)
            {
                this.Hostname = targetChunks[0];
                this.Collection = targetChunks[1];
                this.Identifier = targetChunks[2];
            }
            else
            {
                throw new Exception("Cannot parse target description.");
            }
        }

        public TargetDescription(string username, string hostname, string collection, string identifier)
        {
            this.Username = username;
            this.Hostname = hostname;
            this.Collection = collection;
            this.Identifier = identifier;
        }

        public string Username { get => this.username; set => this.username = value; }

        public string Hostname { get => this.hostname; set => this.hostname = value; }

        public string Collection { get => this.collection; set => this.collection = value; }

        public string Identifier { get => this.identifier; set => this.identifier = value; }
    }
}