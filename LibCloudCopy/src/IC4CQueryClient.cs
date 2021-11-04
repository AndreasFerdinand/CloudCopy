namespace CloudCopy
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IC4CQueryClient
    {
        public Task<string> GetObjectIDFromUserFriendlyId(string collectionName, string userFriendlyId, string humanReadableIDName);
        public Task<string> GetObjectIDFromUserFriendlyId(string collectionName, string userFriendlyId, string humanReadableIDName, CancellationToken cancellationToken);
    }
}
