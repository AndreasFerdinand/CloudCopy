using System;
using System.Collections.Generic;

namespace CloudCopy
{
    interface IRemoteFileListing<T> : IEnumerable<T>
    {
        void listFiles();
        void listFiles(IOutputOptions outputOptions);

        void removeNotMatchingWildcard(string pattern);
        void removeNotMatchingRegex(string pattern);
    }
}