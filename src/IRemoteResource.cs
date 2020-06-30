namespace CloudCopy
{
    using System;
    using System.Threading.Tasks;

    public interface IRemoteResource
    {
        string TypeCode { get; set; }

        public Task<string> GetSubPathAsync();
    }
}