using System;
using System.Threading;
using System.Threading.Tasks;

namespace CloudCopy
{
    interface IC4CQueryClient
    {
        public Task<string> getObjectIDFromID(string CollectionName, string ID, string humanReadableIDName);
    }
}
