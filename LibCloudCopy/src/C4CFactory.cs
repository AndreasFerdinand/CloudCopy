namespace CloudCopy
{
    using System;
    using System.Net;
    using System.Net.Http;

    public class C4CFactory : IC4CFactory
    {
        public static string GetBaseEndpointFromHostname(string hostname)
        {
            // Uri Example: "https://myXXXXXX.crm.ondemand.com/sap/c4c/odata/v1/c4codataapi/"
            return "https://" + hostname + "/sap/c4c/odata/v1/c4codataapi/";
        }

        public C4CHttpClient CreateC4CHttpClient(string hostname, string username, string password)
        {
            return C4CFactory.CreateC4CHttpClient(hostname, new NetworkCredential(username, password));
        }

        public C4CHttpClient CreateC4CHttpClient(string hostname, INetworkCredentialHandler networkCredentialHandler)
        {
            return C4CFactory.CreateC4CHttpClient(hostname, networkCredentialHandler.GetCredentials());
        }

        private static C4CHttpClient CreateC4CHttpClient(string hostname, NetworkCredential networkCredential)
        {
            Uri baseEndpoint = new Uri(C4CFactory.GetBaseEndpointFromHostname(hostname));

            CredentialCache credentialCache = new CredentialCache();

            credentialCache.Add(baseEndpoint, "Basic", networkCredential);

            SocketsHttpHandler clienthandler = new SocketsHttpHandler();

            clienthandler.Credentials = credentialCache;
            clienthandler.CookieContainer = new CookieContainer();

            HttpClient client = new HttpClient(clienthandler);
            client.BaseAddress = baseEndpoint;

            C4CHttpClient cloudClient = new C4CHttpClient(client);

            return cloudClient;
        }
    }
}
