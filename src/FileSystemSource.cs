using System;
using System.IO;

namespace CloudCopy
{
    class FileSystemSource : ILocalResource
    {
        string _FilePath;

        public FileSystemSource(string FilePath)
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
    }

}