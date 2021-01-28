namespace CloudCopy
{
    using System;

    public interface IClientFactory
    {
        public C4CHttpClient CreateC4CHttpClient(string hostname, string username, string password);

        public C4CHttpClient CreateC4CHttpClient(string hostname, INetworkCredentialHandler networkCredentialHandler);
    }
}
