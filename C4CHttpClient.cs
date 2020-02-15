using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace CloudCopy
{
    class C4CHttpClient : IClient, IC4CQueryClient
    {
        HttpClient _HttpClient;
        string _CSRFToken;

        static SemaphoreSlim _SemaphoreSlim = new SemaphoreSlim(1,1);

        public async Task<object> DownloadFileAsync(IRemoteResource Source, ILocalResource Target)
        {
            await fetchCsrfTokenAsync();
            throw new NotImplementedException();
        }

        public async Task<C4CRemoteFileListing> GetFileListingAsync(IRemoteResource Source)
        {
            await fetchCsrfTokenAsync();

            string query = "?$select=UUID,MimeType,Name,DocumentLink,CategoryCode&$orderby=Name";

            var ResponseMessage = await _HttpClient.GetAsync(_HttpClient.BaseAddress.ToString() + Source.getSubPath() + query);

            var content = await ResponseMessage.Content.ReadAsStringAsync();

            C4CRemoteFileListing FileListing = new C4CRemoteFileListing(content);

            return FileListing;
        }

        public async Task<IRemoteFileMetadata> UploadFileAsync(ILocalResource Source, IRemoteResource Target)
        {
            await fetchCsrfTokenAsync();

            var request = new HttpRequestMessage();

            request.RequestUri = new Uri(_HttpClient.BaseAddress.ToString() + Target.getSubPath());
            request.Method = System.Net.Http.HttpMethod.Post;
            request.Headers.Add("x-csrf-token", _CSRFToken);
            request.Headers.Add("Accept", "application/xml");
            request.Headers.Add("odata-no-response-payload","true");

            string requestcontent = "{\"TypeCode\": \"10001\",\"Name\": \"" + Source.getFileName() + "\",\"CategoryCode\": \"2\",\"Binary\": \"" + Source.getBase64SourceString() + "\"}";

            request.Content = new StringContent(requestcontent,Encoding.UTF8, "application/json");

            var ResponseMessage = await _HttpClient.SendAsync(request);

            var content = await ResponseMessage.Content.ReadAsStringAsync();


            //we send a second request to read the metadate of the newly uploaded file
            HttpHeaders headers = ResponseMessage.Headers;
            IEnumerable<string> values;
            string MetadataUri = "";

            if (headers.TryGetValues("Location", out values))
            {
                MetadataUri = values.First();
            }

            if ( string.IsNullOrEmpty(MetadataUri))
            {
                throw new NotImplementedException();
            }

            //read back file metadata
            string MetadataRequestUri = MetadataUri + "?$select=UUID,MimeType,Name,DocumentLink";
            ResponseMessage =  await _HttpClient.GetAsync(MetadataRequestUri);

            content = await ResponseMessage.Content.ReadAsStringAsync();

            C4CRemoteFileMetadata Metadata = new C4CRemoteFileMetadata(content);


            return Metadata;
        }

        private async Task<string> fetchCsrfTokenAsync()
        {
            await _SemaphoreSlim.WaitAsync();

            //Console.Write("Semaphore.WaitAsync()");
            
            try
            {
                if ( !string.IsNullOrEmpty( _CSRFToken ) )
                {
                    return _CSRFToken;
                }

                var request = new HttpRequestMessage();
                request.Method = System.Net.Http.HttpMethod.Get;
                request.Headers.Add("x-csrf-token", "fetch");
                request.Headers.Add("Accept", "application/json");
                

                var ResponseMessage = await _HttpClient.SendAsync(request);

                if ( !ResponseMessage.IsSuccessStatusCode )
                {
                    throw new NotImplementedException();
                }

                HttpHeaders headers = ResponseMessage.Headers;
                
                IEnumerable<string> values;

                if (headers.TryGetValues("x-csrf-token", out values))
                {
                    _CSRFToken = values.First();

                    return _CSRFToken;
                }
                else
                {
                    throw new NotImplementedException();
                }
            }
            finally
            {
                _SemaphoreSlim.Release();
            }
        }

        public void setHttpClient(HttpClient _HttpClient )
        {
            this._HttpClient = _HttpClient;
        }

        public async Task<string> getObjectIDFromID(string CollectionName, string ID, string humanReadableIDName)
        {
            var request = new HttpRequestMessage();
            
            request.Method = System.Net.Http.HttpMethod.Get;
            request.RequestUri = new Uri(_HttpClient.BaseAddress.ToString() + CollectionName + "?$filter=" + humanReadableIDName + " eq '" + ID + "'&$select=ObjectID");

            request.Headers.Add("Accept", "application/xml");

            var ResponseMessage = await _HttpClient.SendAsync(request);

            var content = await ResponseMessage.Content.ReadAsStringAsync();



            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(content);

            XmlNamespaceManager mgr = new XmlNamespaceManager(xmlDoc.NameTable);
            mgr.AddNamespace("d", "http://schemas.microsoft.com/ado/2007/08/dataservices");
            mgr.AddNamespace("m", "http://schemas.microsoft.com/ado/2007/08/dataservices/metadata");
            mgr.AddNamespace(String.Empty,"http://www.w3.org/2005/Atom");
            mgr.AddNamespace("default","http://www.w3.org/2005/Atom");

            XmlElement root = xmlDoc.DocumentElement;

            XmlNode ObjectIDNode = root.SelectSingleNode("//m:properties/d:ObjectID",mgr);

            return ObjectIDNode.InnerText;
        }
    }
}
