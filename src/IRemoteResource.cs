using System;
using System.Threading.Tasks;

namespace CloudCopy
{
    interface IRemoteResource
    {
        public Task<string> getSubPathAsync();

    }

}