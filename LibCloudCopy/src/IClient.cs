namespace CloudCopy
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IClient
    {
        public Task<IRemoteFileMetadata> UploadFileAsync(ILocalResource source, IRemoteResource target);

        public Task DownloadFileAsync(IRemoteFileMetadata source, ILocalResource target);

        Task<C4CRemoteFileListing> GetFileListingAsync(IRemoteResource source);
    }
}
