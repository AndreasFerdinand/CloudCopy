namespace CloudCopy
{
    public class TargetDescription
    {
        private string username = string.Empty;
        private string hostname = string.Empty;
        private string collection = string.Empty;
        private string identifier = string.Empty;

        public TargetDescription()
        {
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