namespace CloudCopy
{
    public interface ICloudClientFactory
    {
        C4CHttpClient CreateCloudClient(string hostname, string username);
    }
}