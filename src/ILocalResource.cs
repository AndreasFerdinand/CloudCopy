using System;
using System.Threading.Tasks;
using System.Net.Http;

namespace CloudCopy
{
    interface ILocalResource
    {
        public string getBase64SourceString();
        public string getFileName();

        public string getPath();

        public Task writeNewFile(HttpContent content);
    }


}