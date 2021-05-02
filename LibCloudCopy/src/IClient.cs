namespace CloudCopy
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IClient
    {
        public Task<IRemoteFileMetadata> UploadFileAsync(ILocalResource source, IRemoteResource target);

        public Task DownloadFileAsync(IRemoteFileMetadata source, ILocalResource target);

        Task<List<C4CRemoteFileMetadata>> GetFileListingAsync(IRemoteResource source);
    }
}
