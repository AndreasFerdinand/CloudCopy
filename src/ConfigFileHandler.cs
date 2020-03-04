using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Xml;

namespace CloudCopy
{
    class ConfigFileHandler : INetworkCredentialHandler
    {
        string _Filename;
        string _Username;
        string _Password;
        string _Hostname;

        public ConfigFileHandler()
        {
            Filename = getDefaultConfigFilePath();

            LoadData();
        }

        public static string getDefaultConfigFilePath()
        {
            string TempFilename = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

            TempFilename = TempFilename + Path.DirectorySeparatorChar + "CloudCopy" + Path.DirectorySeparatorChar + "default.xml";

            return TempFilename;
        }

        private void LoadData()
        {
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(Filename);

                XmlElement root = xmlDoc.DocumentElement;

                XmlNode Node = root.SelectSingleNode("Hostname");
                Hostname = Node.InnerText;

                Node = root.SelectSingleNode("Username");
                Username = Node.InnerText;

                Node = root.SelectSingleNode("Password");
                Password = Node.InnerText;
            }
            catch
            {
                throw new Exception("Cannot read credential file " + Filename);
            } 
        }

        public ConfigFileHandler(string filename)
        {
            Filename = filename;

            LoadData();
        }

        public string Filename { get => _Filename; set => _Filename = value; }
        public string Username { get => _Username; set => _Username = value; }
        public string Password { get => _Password; set => _Password = value; }
        public string Hostname { get => _Hostname; set => _Hostname = value; }

        public NetworkCredential GetCredentials()
        {
            return new NetworkCredential(Username, Password);
        }
    }
}