using System;

namespace CloudCopy
{
    interface IRemoteFileListing
    {
        void listFiles();
        void listFiles(IOutputOptions outputOptions);

        void removeNotMatchingWildcard(string pattern);
        void removeNotMatchingRegex(string pattern);
    }
}