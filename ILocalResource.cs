using System;

namespace CloudCopy
{
    interface ILocalResource
    {
        public string getBase64SourceString();
        public string getFileName();
    }


}