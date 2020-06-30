namespace CloudCopy
{
    using System;
    using System.IO;
    using System.Net.Http;
    using System.Threading.Tasks;

    public class FileSystemResource : ILocalResource
    {
        private string filePath;

        public FileSystemResource(string filePath)
        {
            this.filePath = filePath;
        }

        public string GetBase64SourceString()
        {
            byte[] bytes = File.ReadAllBytes(this.filePath);
            string base64File = Convert.ToBase64String(bytes);

            return base64File;
        }

        public string GetFileName()
        {
            return Path.GetFileName(this.filePath);
        }

        public string GetPath()
        {
            return this.filePath;
        }

        public async Task WriteNewFile(HttpContent content)
        {
            using (var fileToWrite = new FileStream(this.GetPath(), FileMode.Create, FileAccess.Write))
            {
                await content.CopyToAsync(fileToWrite);
            }
        }
    }
}