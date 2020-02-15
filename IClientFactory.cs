using System;

namespace CloudCopy
{
    interface IClientFactory
    {
        public C4CHttpClient createC4CHttpClient(string hostname,string username, string password);
        public C4CHttpClient createC4CHttpClient(string hostname,INetworkCredentialHandler networkCredentialHandler);


    }
}
