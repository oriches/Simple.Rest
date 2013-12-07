namespace Simple.Rest
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Net;

    public interface IRestResponse
    {
        /// <summary>
        /// The status code of the HTTP response.
        /// </summary>
        HttpStatusCode StatusCode { get; }

        /// <summary>
        /// The status code of the HTTP response.
        /// </summary>
        string StatusDescription { get; }

        /// <summary>
        /// The exception generated during the HTTP response.
        /// </summary>
        Exception Exception { get; }

        /// <summary>
        /// The cookies returned by the HTTP response.
        /// </summary>
        CookieCollection Cookies { get; }

        /// <summary>
        /// The headers returned by the HTTP response.
        /// </summary>
        WebHeaderCollection Headers { get; }
        
        /// <summary>
        /// Was the HTTP request successful
        /// </summary>
        bool Successfully { get; }
    }
}