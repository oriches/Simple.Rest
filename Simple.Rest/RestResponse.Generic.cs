using System;
using System.Net;
using Simple.Rest.Standard;

namespace Simple.Rest
{
    /// <summary>
    ///     Strongly typed response from the RestClient, contains all the HTTP related statuses, cookies &amp; headers.
    /// </summary>
    /// <typeparam name="T">The type of the resource.</typeparam>
    public class RestResponse<T> : RestResponse, IRestResponse<T> where T : class
    {
        /// <summary>
        ///     Constructor for a successful response.
        /// </summary>
        /// <param name="response">The HttpWebResponse instance.</param>
        /// <param name="resource">The response resource</param>
        public RestResponse(HttpWebResponse response, T resource)
            : this(response, null, resource)
        {
        }

        /// <summary>
        ///     Constructor for a failed response.
        /// </summary>
        /// <param name="response">The HttpWebResponse instance.</param>
        /// <param name="exception">The exception generated.</param>
        /// <param name="resource">The response resource</param>
        public RestResponse(HttpWebResponse response, Exception exception, object resource)
            : base(response, exception)
        {
            if (resource != null) Resource = (T) resource;
        }

        /// <summary>
        ///     The response resource.
        /// </summary>
        public T Resource { get; }
    }
}