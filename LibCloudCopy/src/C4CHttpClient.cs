namespace CloudCopy
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Xml;

    public class C4CHttpClient : IClient, IC4CQueryClient
    {
        protected HttpClient httpClient;
        private static SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);
        private string CSRFToken;

        public async Task<string> GetXMLStringAsync(string subPath)
        {
            return await GetXMLStringAsync( subPath, new CancellationToken() );
        }

        public async Task<string> GetXMLStringAsync(string subPath, CancellationToken cancellationToken)
        {
            HttpResponseMessage responseMessage;
            var request = new HttpRequestMessage();

            request.RequestUri = new Uri(subPath, UriKind.Relative);
            request.Headers.Add("Accept", "application/xml");
            request.Method = System.Net.Http.HttpMethod.Get;

            responseMessage = await this.httpClient.SendAsync(request,cancellationToken);

            responseMessage.EnsureSuccessStatusCode();

            return await responseMessage.Content.ReadAsStringAsync();
        }

        public async Task DownloadFileAsync(IRemoteFileMetadata source, ILocalResource target)
        {
            await DownloadFileAsync(source, target, new CancellationToken() );
        }

        public async Task DownloadFileAsync(IRemoteFileMetadata source, ILocalResource target, CancellationToken cancellationToken)
        {
            // await fetchCsrfTokenAsync();
            try
            {
                var responseMessage = await this.httpClient.GetAsync(source.DownloadURI, cancellationToken);

                responseMessage.EnsureSuccessStatusCode();

                await target.WriteNewFile(responseMessage.Content);
            }
            catch (Exception ex)
            {
                throw new C4CClientException("An error occured while downloading file.", ex);
            }
        }

        public async Task<List<C4CRemoteFileMetadata>> GetFileListingAsync(IRemoteResource source)
        {
            return await GetFileListingAsync(source, new CancellationToken() );
        }

        public async Task<List<C4CRemoteFileMetadata>> GetFileListingAsync(IRemoteResource source, CancellationToken cancellationToken)
        {
            // await fetchCsrfTokenAsync();
            HttpResponseMessage responseMessage;
            List<C4CRemoteFileMetadata> receivedMetadata = new List<C4CRemoteFileMetadata>();

            string query = "?$select=UUID,MimeType,Name,DocumentLink,CategoryCode,LastUpdatedOn&$orderby=Name";

            string sourceSubPath = await source.GetSubPathAsync();

            try
            {
                responseMessage = await this.httpClient.GetAsync(sourceSubPath + query, cancellationToken);

                responseMessage.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                throw new C4CClientException("Error occured while receiving file listing", ex);
            }

            var content = await responseMessage.Content.ReadAsStringAsync();

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(content);

            XmlNamespaceManager mgr = C4CRemoteFileMetadata.GetDefaultXmlNamespaceManager();

            XmlNodeList xmlNodes = xmlDoc.SelectNodes("//default:entry", mgr);

            foreach (XmlNode elementNode in xmlNodes)
            {
                C4CRemoteFileMetadata currentFile = new C4CRemoteFileMetadata(elementNode);

                receivedMetadata.Add(currentFile);
            }

            return receivedMetadata;
        }

        public async Task<IRemoteFileMetadata> UploadFileAsync(ILocalResource source, IRemoteResource target)
        {
            return await UploadFileAsync(source, target, new CancellationToken() );
        }

        public async Task<IRemoteFileMetadata> UploadFileAsync(ILocalResource source, IRemoteResource target, CancellationToken cancellationToken)
        {
            await this.FetchCsrfTokenAsync(cancellationToken).ConfigureAwait(false);

            HttpResponseMessage responseMessage;
            string content;

            var subPath = await target.GetSubPathAsync();

            try
            {
                var request = new HttpRequestMessage();

                request.RequestUri = new Uri(subPath, UriKind.Relative);
                request.Method = System.Net.Http.HttpMethod.Post;
                request.Headers.Add("x-csrf-token", this.CSRFToken);
                request.Headers.Add("Accept", "application/xml");
                request.Headers.Add("odata-no-response-payload", "true");

                string requestcontent = "{\"TypeCode\": \"" + target.TypeCode + "\",\"Name\": \"" + source.GetFileName() + "\",\"CategoryCode\": \"2\",\"Binary\": \"" + source.GetBase64SourceString() + "\"}";

                request.Content = new StringContent(requestcontent, Encoding.UTF8, "application/json");

                responseMessage = await this.httpClient.SendAsync(request, cancellationToken);

                responseMessage.EnsureSuccessStatusCode();

                await responseMessage.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                throw new C4CClientException("Error occured while uploading file", ex);
            }

            // we send a second request to read the metadata of the newly uploaded file
            HttpHeaders headers = responseMessage.Headers;
            IEnumerable<string> values;
            string metadataUri = string.Empty;

            if (headers.TryGetValues("Location", out values))
            {
                metadataUri = values.First();
            }

            if (string.IsNullOrEmpty(metadataUri))
            {
                throw new C4CClientException("Location-header not provided by remote host");
            }

            // read back file metadata
            string metadataRequestUri = metadataUri + "?$select=UUID,MimeType,Name,DocumentLink,CategoryCode,LastUpdatedOn";
            responseMessage = await this.httpClient.GetAsync(metadataRequestUri, cancellationToken);

            content = await responseMessage.Content.ReadAsStringAsync();

            C4CRemoteFileMetadata metadata = new C4CRemoteFileMetadata(content);

            return metadata;
        }

        public void SetHttpClient(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public HttpClient GetHttpClient()
        {
            return this.httpClient;
        }


        public async Task<string> GetObjectIDFromUserFriendlyId(string collectionName, string userFriendlyId, string userFriendlyIDName)
        {
            return await GetObjectIDFromUserFriendlyId(collectionName,userFriendlyId,userFriendlyIDName, new CancellationToken() );
        }

        public async Task<string> GetObjectIDFromUserFriendlyId(string collectionName, string userFriendlyId, string userFriendlyIDName, CancellationToken cancellationToken)
        {
            HttpResponseMessage responseMessage;

            try
            {
                var request = new HttpRequestMessage();

                request.Method = System.Net.Http.HttpMethod.Get;
                request.RequestUri = new Uri(collectionName + "?$filter=" + userFriendlyIDName + " eq '" + userFriendlyId + "'&$select=ObjectID", UriKind.Relative);

                request.Headers.Add("Accept", "application/xml");

                responseMessage = await this.httpClient.SendAsync(request);

                responseMessage.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                throw new C4CClientException("An error occured while requesting Object ID from User-friendly ID", ex);
            }

            var content = await responseMessage.Content.ReadAsStringAsync();

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(content);

            XmlNamespaceManager mgr = new XmlNamespaceManager(xmlDoc.NameTable);
            mgr.AddNamespace("d", "http://schemas.microsoft.com/ado/2007/08/dataservices");
            mgr.AddNamespace("m", "http://schemas.microsoft.com/ado/2007/08/dataservices/metadata");
            mgr.AddNamespace(string.Empty, "http://www.w3.org/2005/Atom");
            mgr.AddNamespace("default", "http://www.w3.org/2005/Atom");

            XmlElement root = xmlDoc.DocumentElement;

            XmlNode objectIDNode = root.SelectSingleNode("//m:properties/d:ObjectID", mgr);

            if (objectIDNode == null)
            {
                throw new C4CClientException("Object with User-friendly ID " + userFriendlyId + " not found.");
            }

            return objectIDNode.InnerText;
        }

        private async Task<string> FetchCsrfTokenAsync(CancellationToken cancellationToken)
        {
            await semaphoreSlim.WaitAsync().ConfigureAwait(false);

            HttpResponseMessage responseMessage;

            try
            {
                if (!string.IsNullOrEmpty(this.CSRFToken))
                {
                    return this.CSRFToken;
                }

                try
                {
                    var request = new HttpRequestMessage();
                    request.Method = System.Net.Http.HttpMethod.Get;
                    request.Headers.Add("x-csrf-token", "fetch");
                    request.Headers.Add("Accept", "application/json");

                    responseMessage = await this.httpClient.SendAsync(request,cancellationToken);

                    responseMessage.EnsureSuccessStatusCode();
                }
                catch (Exception ex)
                {
                    throw new C4CClientException("An error occured during CSRF-Token request", ex);
                }

                HttpHeaders headers = responseMessage.Headers;

                IEnumerable<string> values;

                if (headers.TryGetValues("x-csrf-token", out values))
                {
                    this.CSRFToken = values.First();

                    return this.CSRFToken;
                }
                else
                {
                    throw new C4CClientException("x-csrf-token-header not provided by remote host");
                }
            }
            finally
            {
                semaphoreSlim.Release();
            }
        }
    }
}