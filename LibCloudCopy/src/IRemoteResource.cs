namespace CloudCopy
{
    using System.Threading;
    using System.Threading.Tasks;

    public interface IRemoteResource
    {
        string TypeCode { get; set; }

        public Task<string> GetSubPathAsync();

        public Task<string> GetSubPathAsync(CancellationToken cancellationToken);
    }
}