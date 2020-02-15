using System;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization;

namespace CloudCopy
{
    class ClientFactory : IClientFactory
    {
        public C4CHttpClient createC4CHttpClient(string hostname, string username, string password)
        {
            return createC4CHttpClient(hostname,new NetworkCredential(username, password));
        }

        public C4CHttpClient createC4CHttpClient(string hostname, INetworkCredentialHandler networkCredentialHandler)
        {
            return createC4CHttpClient(hostname,networkCredentialHandler.GetCredentials());
        }

        private C4CHttpClient createC4CHttpClient(string hostname, NetworkCredential networkCredential )
        {
            C4CHttpClient C4CClient = new C4CHttpClient();


            Uri BaseEndpoint = new Uri(getBaseEndpointFromHostname(hostname));

            CredentialCache _CredentialCache = new CredentialCache();

            _CredentialCache.Add(BaseEndpoint, "Basic", networkCredential);

            SocketsHttpHandler clienthandler = new SocketsHttpHandler();

            clienthandler.Credentials = _CredentialCache;
            clienthandler.CookieContainer = new CookieContainer();

            HttpClient Client = new HttpClient(clienthandler);
            Client.BaseAddress = BaseEndpoint;


            C4CClient.setHttpClient(Client);

            return C4CClient;
        }

        public string getBaseEndpointFromHostname(string hostname)
        {
            //"https://myXXXXXX.crm.ondemand.com/sap/c4c/odata/v1/c4codataapi/";
            return "https://" + hostname + "/sap/c4c/odata/v1/c4codataapi/";
        }
    }
}
