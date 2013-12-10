namespace Simple.Rest
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Net;

    /// <summary>
    /// The response from the RestClient, contains all the HTTP related statuses, cookies &amp; headers.
    /// </summary>
    [Pure]
    public class RestResponse : IRestResponse
    {
        /// <summary>
        /// The status code of the HTTP response.
        /// </summary>
        public HttpStatusCode StatusCode { get; private set; }
        
        /// <summary>
        /// The status code of the HTTP response.
        /// </summary>
        public string StatusDescription { get; private set; }

        /// <summary>
        /// The exception generated during the HTTP response.
        /// </summary>
        public Exception Exception { get; private set; }

        /// <summary>
        /// The cookies returned by the HTTP response.
        /// </summary>
        public CookieCollection Cookies { get; private set; }

        /// <summary>
        /// The headers returned by the HTTP response.
        /// </summary>
        public WebHeaderCollection Headers { get; private set; }

        /// <summary>
        /// Was the HTTP request successful
        /// </summary>
        public bool Successfully { get { return StatusCode == HttpStatusCode.OK && Exception == null; } }

        /// <summary>
        /// Extracts the properties from the HTTP response.
        /// </summary>
        /// <param name="response">The HTTP response.</param>
        public RestResponse(HttpWebResponse response) : this(response, null)
        {
            Contract.Requires<ArgumentNullException>(response != null);
        }
        
        /// <summary>
        /// Extracts the properties from the HTTP response and handles the exception generated during the request.
        /// </summary>
        /// <param name="response">The HTTP response.</param>
        /// <param name="exception">The exception generated during the HTTP request\response.</param>
        public RestResponse(HttpWebResponse response, Exception exception)
        {
            Contract.Requires<ArgumentNullException>(response != null);

            StatusCode = response.StatusCode;
            StatusDescription = response.StatusDescription;
            Cookies = response.Cookies;
            Headers = response.Headers;

            Exception = exception;
        }
    }
}