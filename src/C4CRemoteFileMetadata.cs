namespace CloudCopy
{
    using System;
    using System.Globalization;
    using System.Xml;

    public class C4CRemoteFileMetadata : IRemoteFileMetadata, IFormattable
    {
        private XmlNode fileXmlRootNode;
        private XmlNamespaceManager xmlNamespaceManager;
        private string filename;
        private string uuid;
        private string mimeType;
        private string user;
        private DateTime changedAt;
        private string categoryCode;
        private Uri metadataURI;
        private Uri downloadURI;

        public C4CRemoteFileMetadata()
        {
            this.xmlNamespaceManager = GetDefaultXmlNamespaceManager();
        }

        public C4CRemoteFileMetadata(XmlNode fileRootNode)
            : this()
        {
            this.fileXmlRootNode = fileRootNode;

            this.ParseProperties();
        }

        public C4CRemoteFileMetadata(string metadataXML)
            : this()
        {
            XmlDocument xmlDoc = new XmlDocument();

            xmlDoc.LoadXml(metadataXML);

            XmlElement root = xmlDoc.DocumentElement;

            this.fileXmlRootNode = xmlDoc.SelectSingleNode("//default:entry", this.xmlNamespaceManager);

            this.ParseProperties();
        }

        public string Filename { get => this.filename; set => this.filename = value; }

        public string UUID { get => this.uuid; set => this.uuid = value; }

        public string MimeType { get => this.mimeType; set => this.mimeType = value; }

        public string User { get => this.user; set => this.user = value; }

        public DateTime ChangedAt { get => this.changedAt; set => this.changedAt = value; }

        public Uri MetadataURI { get => this.metadataURI; set => this.metadataURI = value; }

        public Uri DownloadURI { get => this.downloadURI; set => this.downloadURI = value; }

        public string CategoryCode { get => this.categoryCode; set => this.categoryCode = value; }

        public static XmlNamespaceManager GetDefaultXmlNamespaceManager()
        {
            XmlNamespaceManager namespaceManager = new XmlNamespaceManager(new NameTable());
            namespaceManager.AddNamespace("d", "http://schemas.microsoft.com/ado/2007/08/dataservices");
            namespaceManager.AddNamespace("m", "http://schemas.microsoft.com/ado/2007/08/dataservices/metadata");
            namespaceManager.AddNamespace(string.Empty, "http://www.w3.org/2005/Atom");
            namespaceManager.AddNamespace("default", "http://www.w3.org/2005/Atom");

            return namespaceManager;
        }

        public void PrintMetdata()
        {
            Console.WriteLine("Remote Filename: " + this.Filename);
            Console.WriteLine("UUID: " + this.UUID);
            Console.WriteLine("MimeType: " + this.MimeType);
            Console.WriteLine("Metadata URI: " + this.MetadataURI.ToString());
            Console.WriteLine("Download URI: " + this.DownloadURI.ToString());
        }

        public string GetProperty(string propertyName)
        {
            string propertyValue;

            XmlNode objectIDNode = this.fileXmlRootNode.SelectSingleNode(".//m:properties/d:" + propertyName, this.xmlNamespaceManager);

            if (objectIDNode == null)
            {
                throw new Exception("Property " + propertyName + " not found");
            }

            propertyValue = objectIDNode.InnerText;

            return propertyValue;
        }

        public override string ToString()
        {
            return this.ToString("G", CultureInfo.CurrentCulture);
        }

        public string ToString(string format)
        {
           return this.ToString(format, CultureInfo.CurrentCulture);
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (string.IsNullOrEmpty(format))
            {
                format = "G";
            }

            if (formatProvider == null)
            {
                formatProvider = CultureInfo.CurrentCulture;
            }

            switch (format.ToUpperInvariant())
            {
                case "F":
                case "G":
                    return this.Filename;
                case "D":
                    return this.CategoryCode == "2" ? this.DownloadURI.ToString() : string.Empty;
                case "FD":
                    return this.ToString("F") + " " + this.ToString("D");
                case "LS":
                    return this.ChangedAt.ToString(formatProvider) + "  " + this.Filename;
                default:
                    throw new FormatException(string.Format("The {0} format string is not supported.", format));
            }
        }

        private void ParseProperties()
        {
            XmlNode objectIDNode;

            this.Filename = this.GetProperty("Name");
            this.UUID = this.GetProperty("UUID");
            this.MimeType = this.GetProperty("MimeType");
            this.CategoryCode = this.GetProperty("CategoryCode");

            this.ChangedAt = DateTime.Parse(this.GetProperty("LastUpdatedOn"));

            objectIDNode = this.fileXmlRootNode.SelectSingleNode(".//default:id", this.xmlNamespaceManager);
            this.MetadataURI = new Uri(objectIDNode.InnerText);

            if (this.CategoryCode == "2")
            {
                this.DownloadURI = new Uri(this.GetProperty("DocumentLink"));
            }
        }

        private string ReplaceHost(string original, string newHostName)
        {
            var builder = new UriBuilder(original);
            builder.Host = newHostName;
            return builder.Uri.ToString();
        }
    }
}