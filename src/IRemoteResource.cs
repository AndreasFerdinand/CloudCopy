using System;
using System.Threading.Tasks;

namespace CloudCopy
{
    interface IRemoteResource
    {
        string TypeCode { get; set; }

        public Task<string> getSubPathAsync();

    }

}