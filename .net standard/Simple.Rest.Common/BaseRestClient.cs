﻿using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Simple.Rest.Common.Serializers;

namespace Simple.Rest.Common
{
    /// <summary>
    ///     Class for resource orientated RESTful interface, supports verbs GET, POST, PUT &amp; DELETE.
    /// </summary>
    public abstract class BaseRestClient : IRestClient
    {
        /// <summary>
        ///     Creates an instance with the JSON serializer.
        /// </summary>
        protected BaseRestClient()
            : this(new JsonSerializer(), new JsonSerializer())
        {
        }

        /// <summary>
        ///     Creates an instance with a customer serializer, e.g. XML or JSON.
        /// </summary>
        /// <param name="serializer"></param>
        protected BaseRestClient(ISerializer serializer)
            : this(serializer, serializer)
        {
        }

        /// <summary>
        ///     Creates an instance with specific serializers for requests &amp; responses.
        /// </summary>
        protected BaseRestClient(ISerializer requestSerializer, ISerializer responseSerializer)
        {
            RequestSerializer = requestSerializer;
            ResponseSerializer = responseSerializer;

            Headers = new WebHeaderCollection();
            Cookies = new CookieCollection();
        }

        /// <summary>
        ///     Serializer used for request resource types.
        /// </summary>
        public ISerializer RequestSerializer { get; }

        /// <summary>
        ///     Serializer used for response resource types.
        /// </summary>
        public ISerializer ResponseSerializer { get; }

        /// <summary>
        ///     Cookies container used for the HTTP request.
        /// </summary>
        public CookieCollection Cookies { get; }

        /// <summary>
        ///     HTTP headers collection for the HTTP request.
        /// </summary>
        public WebHeaderCollection Headers { get; }

        /// <summary>
        ///     Credentials used for the HTTP request.
        /// </summary>
        public ICredentials Credentials { get; set; }

        /// <summary>
        ///     Requests the resource asynchronously.
        /// </summary>
        /// <typeparam name="T">The resource type</typeparam>
        /// <param name="url">The URL to GET the resource</param>
        /// <returns>
        ///     Returns the resource wrapped in a Task&lt;IRestResponse&lt;T&gt;&gt;, the interface contains the resource,
        ///     status code &amp; description, headers &amp; cookies.
        /// </returns>
        public Task<IRestResponse<T>> GetAsync<T>(Uri url) where T : class
        {
            return ExecuteRequest<T>(url, HttpMethod.Get);
        }

        /// <summary>
        ///     Requests the resource be stored under the supplied URL asynchronously. If the URL refers to an already existing
        ///     resource, it is modified.
        ///     If the URL does not point to an existing resource, then the server can create the resource with that URL.
        /// </summary>
        /// <typeparam name="T">The resource type</typeparam>
        /// <param name="url">The URL to PUT the resource</param>
        /// <param name="resource">The resource to be PUT</param>
        /// <returns>
        ///     Returns the resource wrapped in a Task&lt;IRestResponse&lt;T&gt;&gt;, the interface contains the resource,
        ///     status code &amp; description, headers &amp; cookies.
        /// </returns>
        public Task<IRestResponse> PutAsync<T>(Uri url, T resource) where T : class
        {
            return ExecuteRequest(url, HttpMethod.Put, resource);
        }

        /// <summary>
        ///     Requests the server accept the resource asynchronously. The resource is identified by the URL.
        /// </summary>
        /// <typeparam name="T">The resource type</typeparam>
        /// <param name="url">The URL to POST the resource</param>
        /// <param name="resource">The resource to be POST'd</param>
        /// <returns>
        ///     Returns the resource wrapped in a Task&lt;IRestResponse&lt;T&gt;&gt;, the interface contains the resource,
        ///     status code &amp; description, headers &amp; cookies.
        /// </returns>
        public Task<IRestResponse<T>> PostAsync<T>(Uri url, T resource) where T : class
        {
            return ExecuteRequest<T, T>(url, HttpMethod.Post, resource);
        }

        /// <summary>
        ///     Requests the server accept the resource asynchronously and receives a disparate return type. The resource is
        ///     identified by the URL.
        /// </summary>
        /// <typeparam name="T">The resource type</typeparam>
        /// <typeparam name="R">The return type</typeparam>
        /// <param name="url">The URL to POST the resource</param>
        /// <param name="resource">The resource to be POST'd</param>
        /// <returns>
        ///     Returns the return type wrapped in a Task&lt;IRestResponse&lt;&gt;&gt;, the interface contains the return,
        ///     status code &amp; description, headers &amp; cookies.
        /// </returns>
        public Task<IRestResponse<R>> PostAsync<T, R>(Uri url, T resource)
            where T : class
            where R : class
        {
            return ExecuteRequest<T, R>(url, HttpMethod.Post, resource);
        }

        /// <summary>
        ///     Deletes a resource asynchronously. The resource is identified by the URL.
        /// </summary>
        /// <param name="url">The URL to GET the resource</param>
        /// <returns>
        ///     Returns the result in a Task&lt;IRestResponse&gt;, the interface contains the status code &amp; description,
        ///     headers &amp; cookies.
        /// </returns>
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

        private Task<IRestResponse<TResponse>> ExecuteRequest<TRequest, TResponse>(Uri url, HttpMethod method,
            TRequest resource)
            where TRequest : class
            where TResponse : class
        {
            var request = CreateRequest(url, method);
            var task = WithBodyRequest<TRequest, TResponse>(request, resource);

            return task;
        }

        private HttpWebRequest CreateRequest(Uri url, HttpMethod method)
        {
            var request = (HttpWebRequest) WebRequest.Create(url);
            request.Method = method.ToString();
            request.Accept = ResponseSerializer.ContentType;

            if (Cookies.Count != 0)
            {
                request.CookieContainer = new CookieContainer();
                request.CookieContainer.Add(url, Cookies);
            }

            if (Headers.Count != 0) request.Headers = Headers;

            if (Credentials != null) request.Credentials = Credentials;

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
                    response = (HttpWebResponse) await request.GetResponseAsync();
                    tcs.SetResult(ProcessResponse<T>(response));
                }
                catch (Exception exn)
                {
                    tcs.SetException(exn);
                }
                finally
                {
                    response?.Dispose();
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
                    response = (HttpWebResponse) await request.GetResponseAsync();
                    tcs.SetResult(new RestResponse(response));
                }
                catch (Exception exn)
                {
                    tcs.SetException(exn);
                }
                finally
                {
                    response?.Dispose();
                }
            }
            catch (Exception exn)
            {
                tcs.SetException(exn);
            }

            return await tcs.Task;
        }

        private async Task<IRestResponse<TResponse>> WithBodyRequest<TRequest, TResponse>(HttpWebRequest request,
            TRequest resource)
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
                    response = (HttpWebResponse) await request.GetResponseAsync();
                    tcs.SetResult(ProcessResponse<TResponse>(response));
                }
                catch (Exception exn)
                {
                    tcs.SetException(exn);
                }
                finally
                {
                    response?.Dispose();
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
                    response = (HttpWebResponse) await request.GetResponseAsync();
                    tcs.SetResult(new RestResponse(response));
                }
                catch (Exception exn)
                {
                    tcs.SetException(exn);
                }
                finally
                {
                    response?.Dispose();
                }
            }
            catch (Exception exn)
            {
                tcs.SetException(exn);
            }

            return await tcs.Task;
        }

        protected abstract Task WriteToRequestStream(HttpWebRequest request, byte[] body);

        protected abstract RestResponse<T> ProcessResponse<T>(HttpWebResponse response) where T : class;

        protected static bool IsGzipCompressed(WebResponse response)
        {
            var encoding = response.Headers["Content-Encoding"];
            return !string.IsNullOrEmpty(encoding) && encoding.ToLower().Contains("gzip");
        }

        protected static bool ShouldCompressWithGzip(WebRequest request)
        {
            var encoding = request.Headers["Accept-Encoding"];
            return !string.IsNullOrEmpty(encoding) && encoding.ToLower().Contains("br");
        }

        protected byte[] Serialize<T>(T resource) where T : class
        {
            byte[] result;

            using (var stream = RequestSerializer.Serialize(resource))
            {
                result = new byte[stream.Length];
                stream.Read(result, 0, result.Length);
            }

            return result;
        }

        protected T Deserialize<T>(Stream stream) where T : class
        {
            var result = ResponseSerializer.Deserialize<T>(stream);

            return result;
        }

        internal enum HttpMethod
        {
            Get,
            Post,
            Put,
            Delete
        }
    }
}