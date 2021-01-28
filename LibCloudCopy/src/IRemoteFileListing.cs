namespace CloudCopy
{
    using System.Collections.Generic;

    public interface IRemoteFileListing<out T> : IEnumerable<T>
    {
        void ListFiles();

        void ListFiles(IOutputOptions outputOptions);

        void RemoveNotMatchingWildcard(string pattern);

        void RemoveNotMatchingRegex(string pattern);

        void RemoveEmptyURIs();
    }
}