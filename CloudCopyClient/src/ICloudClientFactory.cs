namespace CloudCopy
{
    public interface ICloudClientFactory
    {
        C4CHttpClient CreateCloudClient(string Hostname, string Username);
    }

}