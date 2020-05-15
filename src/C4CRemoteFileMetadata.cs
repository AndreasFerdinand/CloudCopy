using System;
using System.Xml;

namespace CloudCopy
{
    class C4CRemoteFileMetadata : IRemoteFileMetadata
    {
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

        }

        public C4CRemoteFileMetadata(string metadataXML)
        {
   
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(metadataXML);

            XmlNamespaceManager mgr = new XmlNamespaceManager(xmlDoc.NameTable);
            mgr.AddNamespace("d", "http://schemas.microsoft.com/ado/2007/08/dataservices");
            mgr.AddNamespace("m", "http://schemas.microsoft.com/ado/2007/08/dataservices/metadata");
            mgr.AddNamespace(string.Empty,"http://www.w3.org/2005/Atom");
            mgr.AddNamespace("default","http://www.w3.org/2005/Atom");

            XmlElement root = xmlDoc.DocumentElement;

            XmlNode ObjectIDNode = root.SelectSingleNode("//m:properties/d:Name",mgr);
            Filename = ObjectIDNode.InnerText;

            ObjectIDNode = root.SelectSingleNode("//m:properties/d:UUID",mgr);
            UUID = ObjectIDNode.InnerText;

            ObjectIDNode = root.SelectSingleNode("//m:properties/d:MimeType",mgr);
            MimeType = ObjectIDNode.InnerText;

            ObjectIDNode = root.SelectSingleNode("//m:properties/d:DocumentLink",mgr);
            DownloadURI = new Uri( ObjectIDNode.InnerText );

            ObjectIDNode = root.SelectSingleNode("//default:id",mgr);
            MetadataURI = new Uri( ObjectIDNode.InnerText );
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
    }
}