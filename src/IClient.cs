using System;
using System.Threading;
using System.Threading.Tasks;

namespace CloudCopy
{
    interface IClient
    {
        public Task<IRemoteFileMetadata> UploadFileAsync(ILocalResource Source, IRemoteResource Target);
        public Task DownloadFileAsync(IRemoteFileMetadata Source, ILocalResource Target);

    }
}
