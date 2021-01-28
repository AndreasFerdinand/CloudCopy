namespace CloudCopy
{
    using System.Net.Http;
    using System.Threading.Tasks;

    public interface ILocalResource
    {
        public string GetBase64SourceString();

        public string GetFileName();

        public string GetPath();

        public Task WriteNewFile(HttpContent content);
    }
}