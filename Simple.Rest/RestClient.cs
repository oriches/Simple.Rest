namespace Simple.Rest
{
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Net;
    using System;
    using System.Threading.Tasks;
    using Ionic.Zlib;
    using Serializers;
    using Extensions;

    /// <summary>
    /// Class for resource orientated RESTful interface, supports verbs GET, POST, PUT &amp; DELETE.
    /// </summary>
    public sealed class RestClient : IRestClient
    {
        internal enum HttpMethod
        {
            Get,
            Post,
            Put,
            Delete
        }

        /// <summary>
        /// Creates an instance with the JSON serializer.
        /// </summary>
        public RestClient()
            : this(new JsonSerializer(), new JsonSerializer())
        {
        }

        /// <summary>
        /// Creates an instance with a customer serializer, e.g. XML or JSON.
        /// </summary>
        /// <param name="serializer"></param>
        public RestClient(ISerializer serializer)
            : this(serializer, serializer)
        {
        }

        /// <summary>
        /// Creates an instance with specifiic serializers for requests &amp; responses.
        /// </summary>
        public RestClient(ISerializer requestSerializer, ISerializer responseSerializer)
        {
            RequestSerializer = requestSerializer;
            ResponseSerializer = responseSerializer;

            Headers = new WebHeaderCollection();
            Cookies = new CookieCollection();
        }

        /// <summary>
        /// Serializer used for request resource types.
        /// </summary>
        public ISerializer RequestSerializer { get; private set; }

        /// <summary>
        /// Serializer used for response resource types.
        /// </summary>
        public ISerializer ResponseSerializer { get; private set; }

        /// <summary>
        /// Cookies container used for the HTTP request.
        /// </summary>
        public CookieCollection Cookies { get; private set; }

        /// <summary>
        /// HTTP headers collection for the HTTP request.
        /// </summary>
        public WebHeaderCollection Headers { get; private set; }

        /// <summary>
        /// Credentials used for the HTTP request.
        /// </summary>
        public ICredentials Credentials { get; set; }
        
        /// <summary>
        /// Requests the resource asynchronuously.
        /// </summary>
        /// <typeparam name="T">The resource type</typeparam>
        /// <param name="url">The URL to GET the resource</param>
        /// <returns>Returns the resource wrapped in a Task&lt;IRestResponse&lt;T&gt;&gt;, the interface contains the resource, status code &amp; description, headers &amp; cookies.</returns>
        [Pure]
        public Task<IRestResponse<T>> GetAsync<T>(Uri url) where T : class
        {
            return ExecuteRequest<T>(url, HttpMethod.Get);
        }

        /// <summary>
        /// Requests the resource be stored under the supplied URL asynchronuously. If the URL refers to an already existing resource, it is modified.
        /// If the URL does not point to an existing resource, then the server can create the resource with that URL.
        /// </summary>
        /// <typeparam name="T">The resource type</typeparam>
        /// <param name="url">The URL to PUT the resource</param>
        /// <param name="resource">The resource to be PUT</param>
        /// <returns>Returns the resource wrapped in a Task&lt;IRestResponse&lt;T&gt;&gt;, the interface contains the resource, status code &amp; description, headers &amp; cookies.</returns>
        [Pure]
        public Task<IRestResponse> PutAsync<T>(Uri url, T resource) where T : class
        {
            return ExecuteRequest(url, HttpMethod.Put, resource);
        }

        /// <summary>
        /// Requests the server accept the resource asynchronuously. The resource is identified by the URL.
        /// </summary>
        /// <typeparam name="T">The resource type</typeparam>
        /// <param name="url">The URL to POST the resource</param>
        /// <param name="resource">The resource to be POST'd</param>
        /// <returns>Returns the resource wrapped in a Task&lt;IRestResponse&lt;T&gt;&gt;, the interface contains the resource, status code &amp; description, headers &amp; cookies.</returns>
        [Pure]
        public Task<IRestResponse<T>> PostAsync<T>(Uri url, T resource) where T : class
        {
            return ExecuteRequest<T, T>(url, HttpMethod.Post, resource);
        }

        /// <summary>
        /// Deletes a resource asynchronously. The resource is identified by the URL.
        /// </summary>
        /// <param name="url">The URL to GET the resource</param>
        /// <returns>Returns the result in a Task&lt;IRestResponse&gt;, the interface contains the status code &amp; description, headers &amp; cookies.</returns>
        [Pure]
        public Task<IRestResponse> DeleteAsync(Uri url)
        {
            return ExecuteRequest(url, HttpMethod.Delete);
        }

        private Task<IRestResponse> ExecuteRequest(Uri url, HttpMethod method)
        {
            var request = CreateRequest(url, method);
            var task = NoBodyRequest(request);

            return task;
        }

        private Task<IRestResponse> ExecuteRequest<T>(Uri url, HttpMethod method, T resource) where T : class
        {
            var request = CreateRequest(url, method);
            var task = WithBodyRequest(request, resource);

            return task;
        }

        private Task<IRestResponse<T>> ExecuteRequest<T>(Uri url, HttpMethod method) where T : class
        {
            var request = CreateRequest(url, method);
            var task = NoBodyRequest<T>(request);

            return task;
        }

        private Task<IRestResponse<TResponse>> ExecuteRequest<TRequest, TResponse>(Uri url, HttpMethod method, TRequest resource)
            where TRequest : class
            where TResponse : class
        {
            var request = CreateRequest(url, method);
            var task = WithBodyRequest<TRequest, TResponse>(request, resource);

            return task;
        }

        private HttpWebRequest CreateRequest(Uri url, HttpMethod method)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = method.ToString();
            request.Accept = ResponseSerializer.ContentType;

            if (Cookies.Count != 0)
            {
                request.CookieContainer = new CookieContainer();
                request.CookieContainer.Add(url, Cookies);
            }

            if (Headers.Count != 0)
            {
                request.Headers = Headers;
            }

            if (Credentials != null)
            {
                request.Credentials = Credentials;
            }

            return request;
        }

        private async Task<IRestResponse<T>> NoBodyRequest<T>(HttpWebRequest request) where T : class
        {
            var tcs = new TaskCompletionSource<IRestResponse<T>>();
            try
            {
                HttpWebResponse response = null;
                try
                {
                    response = await request.GetResponseAsync();
                    tcs.SetResult(ProcessResponse<T>(response));
                }
                catch (Exception exn)
                {
                    tcs.SetException(exn);
                }
                finally
                {
                    if (response != null)
                    {
                        response.Dispose();
                    }
                }
            }
            catch (Exception exn)
            {
                tcs.SetException(exn);
            }

            return await tcs.Task;
        }

        private static async Task<IRestResponse> NoBodyRequest(HttpWebRequest request)
        {
            var tcs = new TaskCompletionSource<RestResponse>();

            try
            {
                HttpWebResponse response = null;
                try
                {
                    response = await request.GetResponseAsync();
                    tcs.SetResult(new RestResponse(response));
                }
                catch (Exception exn)
                {
                    tcs.SetException(exn);
                }
                finally
                {
                    if (response != null)
                    {
                        response.Dispose();
                    }
                }
            }
            catch (Exception exn)
            {
                tcs.SetException(exn);
            }

            return await tcs.Task;
        }

        private async Task<IRestResponse<TResponse>> WithBodyRequest<TRequest, TResponse>(HttpWebRequest request, TRequest resource)
            where TRequest : class
            where TResponse : class
        {
            var tcs = new TaskCompletionSource<RestResponse<TResponse>>();

            try
            {
                var body = Serialize(resource);
                request.ContentType = RequestSerializer.ContentType;

                await WriteToRequestStream(request, body);

                HttpWebResponse response = null;
                try
                {
                    response = await request.GetResponseAsync();
                    tcs.SetResult(ProcessResponse<TResponse>(response));
                }
                catch (Exception exn)
                {
                    tcs.SetException(exn);
                }
                finally
                {
                    if (response != null)
                    {
                        response.Dispose();
                    }
                }
            }
            catch (Exception exn)
            {
                tcs.SetException(exn);
            }

            return await tcs.Task;
        }

        private async Task<IRestResponse> WithBodyRequest<T>(HttpWebRequest request, T resource) where T : class
        {
            var tcs = new TaskCompletionSource<RestResponse>();

            try
            {
                var body = Serialize(resource);
                request.ContentType = RequestSerializer.ContentType;

                await WriteToRequestStream(request, body);

                HttpWebResponse response = null;
                try
                {
                    response = await request.GetResponseAsync();
                    tcs.SetResult(new RestResponse(response));
                }
                catch (Exception exn)
                {
                    tcs.SetException(exn);
                }
                finally
                {
                    if (response != null)
                    {
                        response.Dispose();
                    }
                }
            }
            catch (Exception exn)
            {
                tcs.SetException(exn);
            }

            return await tcs.Task;
        }

        private static async Task WriteToRequestStream(HttpWebRequest request, byte[] body)
        {
            using (var requestStream = new BinaryWriter(await request.GetRequestStreamAsync()))
            {
                if (ShouldCompressWithGzip(request))
                {
                    var compressedBody = GZipStream.CompressBuffer(body);
                    requestStream.Write(compressedBody, 0, compressedBody.Length);
                }
                else if (ShouldCompressWithDeflate(request))
                {
                    var compressedBody = DeflateStream.CompressBuffer(body);
                    requestStream.Write(compressedBody, 0, compressedBody.Length);
                }
                else
                {
                    requestStream.Write(body, 0, body.Length);
                }
            }
        }

        private RestResponse<T> ProcessResponse<T>(HttpWebResponse response) where T : class
        {
            Contract.Requires<ArgumentNullException>(response != null);

            try
            {
                if (IsGzipCompressed(response))
                {
                    using (var stream = new GZipStream(response.GetResponseStream(), CompressionMode.Decompress))
                    {
                        var result = Deserialize<T>(stream);
                        return new RestResponse<T>(response, result);
                    } 
                }
                
                if (IsDeflateCompressed(response))
                {
                    using (var stream = new DeflateStream(response.GetResponseStream(), CompressionMode.Decompress))
                    {
                        var result = Deserialize<T>(stream);
                        return new RestResponse<T>(response, result);
                    }
                }

                using (var stream = response.GetResponseStream())
                {
                    var result = Deserialize<T>(stream);
                    return new RestResponse<T>(response, result);
                }
            }
            catch (Exception ex)
            {
                return new RestResponse<T>(response, ex, default(T));
            }
        }

        private static bool IsGzipCompressed(WebResponse response)
        {
            Contract.Requires<ArgumentNullException>(response != null);

            var encoding = response.Headers["Content-Encoding"];
            return !string.IsNullOrEmpty(encoding) && encoding.ToLower().Contains("gzip");
        }

        private static bool IsDeflateCompressed(WebResponse response)
        {
            Contract.Requires<ArgumentNullException>(response != null);

            var encoding = response.Headers["Content-Encoding"];
            return !string.IsNullOrEmpty(encoding) && encoding.ToLower().Contains("deflate");
        }

        private static bool ShouldCompressWithGzip(WebRequest request)
        {
            Contract.Requires<ArgumentNullException>(request != null);

            var encoding = request.Headers["Accept-Encoding"];
            return !string.IsNullOrEmpty(encoding) && encoding.ToLower().Contains("gzip");
        }

        private static bool ShouldCompressWithDeflate(WebRequest request)
        {
            Contract.Requires<ArgumentNullException>(request != null);

            var encoding = request.Headers["Accept-Encoding"];
            return !string.IsNullOrEmpty(encoding) && encoding.ToLower().Contains("deflate");
        }

        private byte[] Serialize<T>(T resource) where T : class
        {
            Contract.Requires<ArgumentNullException>(resource != null);

            byte[] result;

            using (var stream = RequestSerializer.Serialize(resource))
            {
                result = new byte[stream.Length];
                stream.Read(result, 0, result.Length);
            }

            return result;
        }

        private T Deserialize<T>(Stream stream) where T : class
        {
            Contract.Requires<ArgumentNullException>(stream != null);

            var result = ResponseSerializer.Deserialize<T>(stream);

            return result;
        }
    }
}
