namespace CloudCopy
{
    using System;
    using System.IO;
    using System.Net;
    using System.Xml;

    public class ConfigFileHandler : INetworkCredentialHandler
    {
        private string filename;
        private string username;
        private string password;
        private string hostname;

        public ConfigFileHandler()
        {
            this.Filename = GetDefaultConfigFilePath();

            this.LoadData();
        }

        public ConfigFileHandler(string filename)
        {
            this.Filename = filename;

            this.LoadData();
        }

        public string Filename { get => this.filename; set => this.filename = value; }

        public string Username { get => this.username; set => this.username = value; }

        public string Password { get => this.password; set => this.password = value; }

        public string Hostname { get => this.hostname; set => this.hostname = value; }

        public static string GetDefaultConfigFilePath()
        {
            string tempFilename = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

            tempFilename = tempFilename + Path.DirectorySeparatorChar + "CloudCopy" + Path.DirectorySeparatorChar + "default.xml";

            return tempFilename;
        }

        public NetworkCredential GetCredentials()
        {
            return new NetworkCredential(this.Username, this.Password);
        }

        private void LoadData()
        {
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(this.Filename);

                XmlElement root = xmlDoc.DocumentElement;

                XmlNode node = root.SelectSingleNode("Hostname");
                this.Hostname = node.InnerText;

                node = root.SelectSingleNode("Username");
                this.Username = node.InnerText;

                node = root.SelectSingleNode("Password");
                this.Password = node.InnerText;
            }
            catch
            {
                throw new Exception("Cannot read credential file " + this.Filename);
            }
        }
    }
}