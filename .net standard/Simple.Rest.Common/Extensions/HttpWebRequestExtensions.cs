using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Simple.Rest.Common.Extensions
{
    /// <summary>
    ///     HTTP web request extensions - makes using HttpWebRequest easier by using Task&lt;T&gt;.
    /// </summary>
    public static class HttpWebRequestExtensions
    {
        /// <summary>
        ///     Async method for getting web request.
        /// </summary>
        /// <param name="request">The HttpWebRequest instance.</param>
        /// <returns>The request stream asynchronously.</returns>
        public static Task<Stream> GetRequestStreamAsync(this HttpWebRequest request)
        {
            var tcs = new TaskCompletionSource<Stream>();

            try
            {
                request.BeginGetRequestStream(iar =>
                {
                    try
                    {
                        var response = request.EndGetRequestStream(iar);
                        tcs.SetResult(response);
                    }
                    catch (Exception exc)
                    {
                        tcs.SetException(exc);
                    }
                }, null);
            }
            catch (Exception exc)
            {
                tcs.SetException(exc);
            }

            return tcs.Task;
        }

        /// <summary>
        ///     Async method for getting web response.
        /// </summary>
        /// <param name="request">The HttpWebRequest instance.</param>
        /// <returns>The response stream asynchronously.</returns>
        public static Task<HttpWebResponse> GetResponseAsync(this HttpWebRequest request)
        {
            var tcs = new TaskCompletionSource<HttpWebResponse>();

            try
            {
                request.BeginGetResponse(iar =>
                {
                    try
                    {
                        var response = (HttpWebResponse) request.EndGetResponse(iar);
                        tcs.SetResult(response);
                    }
                    catch (Exception exc)
                    {
                        tcs.SetException(exc);
                    }
                }, null);
            }
            catch (Exception exc)
            {
                tcs.SetException(exc);
            }

            return tcs.Task;
        }
    }
}