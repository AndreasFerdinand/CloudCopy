using System;
using System.Net;
using System.Net.Http;

namespace CloudCopy
{
    interface INetworkCredentialHandler
    {
        NetworkCredential GetCredentials();
    }
}