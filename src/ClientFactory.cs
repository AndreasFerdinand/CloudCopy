namespace CloudCopy
{
    using System;
    using System.Net;
    using System.Net.Http;

    public class ClientFactory : IClientFactory
    {
        public C4CHttpClient CreateC4CHttpClient(string hostname, string username, string password)
        {
            return this.CreateC4CHttpClient(hostname, new NetworkCredential(username, password));
        }

        public C4CHttpClient CreateC4CHttpClient(string hostname, INetworkCredentialHandler networkCredentialHandler)
        {
            return this.CreateC4CHttpClient(hostname, networkCredentialHandler.GetCredentials());
        }

        public string GetBaseEndpointFromHostname(string hostname)
        {
            // "https://myXXXXXX.crm.ondemand.com/sap/c4c/odata/v1/c4codataapi/";
            return "https://" + hostname + "/sap/c4c/odata/v1/c4codataapi/";
        }

        private C4CHttpClient CreateC4CHttpClient(string hostname, NetworkCredential networkCredential)
        {
            C4CHttpClient cloudClient = new C4CHttpClient();

            Uri baseEndpoint = new Uri(this.GetBaseEndpointFromHostname(hostname));

            CredentialCache credentialCache = new CredentialCache();

            credentialCache.Add(baseEndpoint, "Basic", networkCredential);

            SocketsHttpHandler clienthandler = new SocketsHttpHandler();

            clienthandler.Credentials = credentialCache;
            clienthandler.CookieContainer = new CookieContainer();

            HttpClient client = new HttpClient(clienthandler);
            client.BaseAddress = baseEndpoint;

            cloudClient.SetHttpClient(client);

            return cloudClient;
        }
    }
}
