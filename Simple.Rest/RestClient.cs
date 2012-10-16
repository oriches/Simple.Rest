namespace Simple.Rest
{
    using System.IO;
    using System.Net;
    using System;
    using System.Threading.Tasks;
    using Serializers;
    using Extensions;

    public class RestClient : IRestClient
    {
        protected enum HttpMethod
        {
            Get,
            Post,
            Put,
            Delete
        }

        public RestClient(ISerializer serializer)
            : this(serializer, serializer)
        {
        }

        public RestClient(ISerializer requestSerializer, ISerializer responseSerializer)
        {
            RequestSerializer = requestSerializer;
            ResponseSerializer = responseSerializer;

            Headers = new WebHeaderCollection();
            Cookies = new CookieCollection();
        }

        public ISerializer RequestSerializer { get; set; }

        public ISerializer ResponseSerializer { get; set; }

        public CookieCollection Cookies { get; set; }

        public WebHeaderCollection Headers { get; set; }

        public ICredentials Credentials { get; set; }

        public virtual Task<IRestResponse<T>> GetAsync<T>(Uri url) where T : class
        {
            return ExecuteRequest<T>(url, HttpMethod.Get);
        }

        public virtual Task<IRestResponse> PutAsync<T>(Uri url, T resource) where T : class
        {
            return ExecuteRequest(url, HttpMethod.Put, resource);
        }

        public virtual Task<IRestResponse<T>> PostAsync<T>(Uri url, T resource) where T : class
        {
            return ExecuteRequest<T, T>(url, HttpMethod.Post, resource);
        }

        public virtual Task<IRestResponse> DeleteAsync(Uri url)
        {
            return ExecuteRequest(url, HttpMethod.Delete);
        }

        protected virtual Task<IRestResponse> ExecuteRequest(Uri url, HttpMethod method)
        {
            var request = CreateRequest(url, method);
            var task = NoBodyRequest(request);

            return task;
        }

        protected virtual Task<IRestResponse> ExecuteRequest<T>(Uri url, HttpMethod method, T resource) where T : class
        {
            var request = CreateRequest(url, method);
            var task = WithBodyRequest(request, resource);

            return task;
        }

        protected virtual Task<IRestResponse<T>> ExecuteRequest<T>(Uri url, HttpMethod method) where T : class
        {
            var request = CreateRequest(url, method);
            var task = NoBodyRequest<T>(request);

            return task;
        }

        protected virtual Task<IRestResponse<TResponse>> ExecuteRequest<TRequest, TResponse>(Uri url, HttpMethod method, TRequest resource)
            where TRequest : class
            where TResponse : class
        {
            var request = CreateRequest(url, method);
            var task = WithBodyRequest<TRequest, TResponse>(request, resource);

            return task;
        }

        protected virtual HttpWebRequest CreateRequest(Uri url, HttpMethod method)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = method.ToString();
            request.Accept = ResponseSerializer.ContentType;

            if (Cookies != null && Cookies.Count != 0)
            {
                request.CookieContainer = new CookieContainer();
                request.CookieContainer.Add(url, Cookies);
            }

            if (Headers != null && Headers.Count != 0)
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

        private async Task<IRestResponse> NoBodyRequest(HttpWebRequest request)
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

                using (var requestStream = new BinaryWriter(await request.GetRequestStreamAsync()))
                {
                    requestStream.Write(body, 0, body.Length);
                }

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

                using (var requestStream = new BinaryWriter(await request.GetRequestStreamAsync()))
                {
                    requestStream.Write(body, 0, body.Length);
                }

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

        private RestResponse<T> ProcessResponse<T>(HttpWebResponse response) where T : class
        {
            try
            {
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
    }
}
