namespace CloudCopy
{
    using System;

    public interface IRemoteFileMetadata
    {
        public string Filename { get; set; }

        public string UUID { get; set; }

        public string MimeType { get; set; }

        public Uri MetadataURI { get; set; }

        public Uri DownloadURI { get; set; }

        DateTime ChangedAt { get; set; }

        string CategoryCode { get; set; }
        
        string User { get; set; }

        public void PrintMetdata();

        public string GetProperty(string propertyName);
    }
}