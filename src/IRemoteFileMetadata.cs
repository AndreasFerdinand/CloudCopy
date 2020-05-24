using System;


namespace CloudCopy
{
    interface IRemoteFileMetadata
    {
        public string Filename { get; set; }
        public string UUID{ get; set; }
        public string MimeType{ get; set; }
        public Uri MetadataURI{ get; set; }
        public Uri DownloadURI{ get; set; }

        public void printMetdata();

        public string getProperty(string PropertyName);
    }

}

