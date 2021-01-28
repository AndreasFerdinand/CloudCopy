namespace CloudCopy
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public class FileProcessingError : Exception
    {
        private string filename;

        public FileProcessingError()
        {
        }

        public FileProcessingError(string filename)
            : base(string.Format("Cannot process file {0}", filename))
        {
            this.Filename = filename;
        }

        public FileProcessingError(string filename, string message)
            : base(message)
        {
            this.Filename = filename;
        }

        public FileProcessingError(string filename, Exception innerException)
            : base(string.Format("Cannot process file {0}", filename), innerException)
        {
            this.Filename = filename;
        }

        public FileProcessingError(string filename, string message, Exception innerException)
            : base(message, innerException)
        {
            this.Filename = filename;
        }

        protected FileProcessingError(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public string Filename { get => this.filename; set => this.filename = value; }
    }
}