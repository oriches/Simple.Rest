using System;
using System.Net;

namespace Simple.Rest.Common
{
    /// <summary>
    ///     The response from the RestClient, contains all the HTTP related statuses, cookies &amp; headers.
    /// </summary>
    public class RestResponse : IRestResponse
    {
        /// <summary>
        ///     Extracts the properties from the HTTP response.
        /// </summary>
        /// <param name="response">The HTTP response.</param>
        public RestResponse(HttpWebResponse response) : this(response, null)
        {
        }

        /// <summary>
        ///     Extracts the properties from the HTTP response and handles the exception generated during the request.
        /// </summary>
        /// <param name="response">The HTTP response.</param>
        /// <param name="exception">The exception generated during the HTTP request\response.</param>
        public RestResponse(HttpWebResponse response, Exception exception)
        {
            StatusCode = response.StatusCode;
            StatusDescription = response.StatusDescription;
            Cookies = response.Cookies;
            Headers = response.Headers;

            Exception = exception;
        }

        /// <summary>
        ///     The status code of the HTTP response.
        /// </summary>
        public HttpStatusCode StatusCode { get; }

        /// <summary>
        ///     The status code of the HTTP response.
        /// </summary>
        public string StatusDescription { get; }

        /// <summary>
        ///     The exception generated during the HTTP response.
        /// </summary>
        public Exception Exception { get; }

        /// <summary>
        ///     The cookies returned by the HTTP response.
        /// </summary>
        public CookieCollection Cookies { get; }

        /// <summary>
        ///     The headers returned by the HTTP response.
        /// </summary>
        public WebHeaderCollection Headers { get; }

        /// <summary>
        ///     Was the HTTP request successful
        /// </summary>
        public bool Successfully => StatusCode == HttpStatusCode.OK && Exception == null;
    }
}