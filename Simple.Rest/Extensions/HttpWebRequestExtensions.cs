namespace Simple.Rest.Extensions
{
    using System;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Net;
    using System.Threading.Tasks;

    /// <summary>
    /// HTTP web request extensions - makes using HttpWebRequest easier by using Task&lt;T&gt;.
    /// </summary>
    public static class HttpWebRequestExtensions
    {
        /// <summary>
        /// Async method for getting web request.
        /// </summary>
        /// <param name="request">The HttpWebRequest instance.</param>
        /// <returns>The request stream asynchronuously.</returns>
        public static Task<Stream> GetRequestStreamAsync(this HttpWebRequest request)
        {
            Contract.Requires<ArgumentNullException>(request != null);

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
        /// Async method for getting web response.
        /// </summary>
        /// <param name="request">The HttpWebRequest instance.</param>
        /// <returns>The response stream asynchronuously.</returns>
        public static Task<HttpWebResponse> GetResponseAsync(this HttpWebRequest request)
        {
            Contract.Requires<ArgumentNullException>(request != null);

            var tcs = new TaskCompletionSource<HttpWebResponse>();

            try
            {
                request.BeginGetResponse(iar =>
                {
                    try
                    {
                        var response = (HttpWebResponse)request.EndGetResponse(iar);
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