using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace CloudCopy
{
    class FileSystemResource : ILocalResource
    {
        string _FilePath;

        public FileSystemResource(string FilePath)
        {
            _FilePath = FilePath;
        }

        public string getBase64SourceString()
        {
            Byte[] bytes = File.ReadAllBytes(_FilePath);
            String Base64File = Convert.ToBase64String(bytes);

            return Base64File;
        }

        public string getFileName()
        {
            return Path.GetFileName(_FilePath);
        }

        public string getPath()
        {
            return _FilePath;
        }

        public async Task writeNewFile(HttpContent content)
        {
            using( var fileToWrite = new FileStream(getPath(),FileMode.Create,FileAccess.Write) )
            {
                await content.CopyToAsync(fileToWrite);
            }
        }
    }

}