namespace CloudCopy
{
    using System;
    using System.Net;
    using System.Net.Http;

    public interface INetworkCredentialHandler
    {
        NetworkCredential GetCredentials();
    }
}