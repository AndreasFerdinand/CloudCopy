using System;
using System.Xml;

namespace CloudCopy
{
    public class C4CRemoteFileMetadata : IRemoteFileMetadata
    {
        XmlNode _FileRootNode;

        XmlNamespaceManager _XmlNamespaceManager;

        string _Filename;
        string _UUID;
        string _MimeType;
        string _User;
        DateTime _ChangedAt;

        string _CategoryCode;

        Uri _MetadataURI;
        Uri _DownloadURI;

        public C4CRemoteFileMetadata()
        {
            _XmlNamespaceManager = getDefaultXmlNamespaceManager();
        }

        public C4CRemoteFileMetadata( XmlNode fileRootNode ) : this()
        {
            _FileRootNode = fileRootNode;

            parseProperties();
        }

        public C4CRemoteFileMetadata(string metadataXML)  : this()
        {
            XmlDocument xmlDoc = new XmlDocument();
            
            xmlDoc.LoadXml(metadataXML);

            XmlElement root = xmlDoc.DocumentElement;

            _FileRootNode = xmlDoc.SelectSingleNode("//default:entry", _XmlNamespaceManager );

            parseProperties();
        }

        private void parseProperties()
        {
            XmlNode ObjectIDNode;

            Filename = getProperty("Name");
            UUID = getProperty("UUID");
            MimeType = getProperty("MimeType");
            CategoryCode = getProperty("CategoryCode");

            ObjectIDNode = _FileRootNode.SelectSingleNode(".//default:id",_XmlNamespaceManager);
            MetadataURI = new Uri( ObjectIDNode.InnerText );

            if (CategoryCode == "2")
            {
                DownloadURI = new Uri( getProperty("DocumentLink") );
            }
        }

        public static XmlNamespaceManager getDefaultXmlNamespaceManager()
        {
            XmlNamespaceManager namespaceManager = new XmlNamespaceManager(new NameTable());
            namespaceManager.AddNamespace("d", "http://schemas.microsoft.com/ado/2007/08/dataservices");
            namespaceManager.AddNamespace("m", "http://schemas.microsoft.com/ado/2007/08/dataservices/metadata");
            namespaceManager.AddNamespace(string.Empty,"http://www.w3.org/2005/Atom");
            namespaceManager.AddNamespace("default","http://www.w3.org/2005/Atom");

            return namespaceManager;
        }

        public string Filename { get => _Filename; set => _Filename = value; }
        public string UUID { get => _UUID; set => _UUID = value; }
        public string MimeType { get => _MimeType; set => _MimeType = value; }
        public string User { get => _User; set => _User = value; }
        public DateTime ChangedAt { get => _ChangedAt; set => _ChangedAt = value; }
        public Uri MetadataURI { get => _MetadataURI; set => _MetadataURI = value; }
        public Uri DownloadURI { get => _DownloadURI; set => _DownloadURI = value; }
        public string CategoryCode { get => _CategoryCode; set => _CategoryCode = value; }

        public void printMetdata()
        {
            Console.WriteLine("Remote Filename: " + Filename);
            Console.WriteLine("UUID: " + UUID);
            Console.WriteLine("MimeType: " + MimeType);
            Console.WriteLine("Metadata URI: " + MetadataURI.ToString());
            Console.WriteLine("Download URI: " + DownloadURI.ToString());
        }

        string ReplaceHost(string original, string newHostName) {
            var builder = new UriBuilder(original);
            builder.Host = newHostName;
            return builder.Uri.ToString();
        }

        public string getProperty(string PropertyName)
        {
            string PropertyValue;

            XmlNode ObjectIDNode = _FileRootNode.SelectSingleNode(".//m:properties/d:" + PropertyName,_XmlNamespaceManager);

            if ( ObjectIDNode == null )
            {
                throw new Exception("Property " + PropertyName + " not found");
            }

            PropertyValue = ObjectIDNode.InnerText;

            return PropertyValue;
        }
    }
}