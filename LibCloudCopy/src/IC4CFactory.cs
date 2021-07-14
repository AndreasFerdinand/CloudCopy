namespace CloudCopy
{
    using System;

    public interface IC4CFactory
    {
        public C4CHttpClient CreateC4CHttpClient(string hostname, string username, string password);

        public C4CHttpClient CreateC4CHttpClient(string hostname, INetworkCredentialHandler networkCredentialHandler);
    }
}
