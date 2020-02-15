using System;

namespace CloudCopy
{
    interface IRemoteFileListing
    {
        void listFiles();
        void listFiles(IOutputOptions outputOptions);
    }
}