namespace Simple.Rest
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Net;

    /// <summary>
    /// Strongly typed response from the RestClient, contains all the HTTP related statuses, cookies &amp; headers.
    /// </summary>
    /// <typeparam name="T">The type of the resource.</typeparam>
    public class RestResponse<T> : RestResponse, IRestResponse<T> where T : class
    {
        /// <summary>
        /// The response resource.
        /// </summary>
        public T Resource { get; }

        /// <summary>
        /// Constructor for a successful response.
        /// </summary>
        /// <param name="response">The HttpWebResponse instance.</param>
        /// <param name="resource">The response resource</param>
        public RestResponse(HttpWebResponse response, T resource) 
            : this(response, null, resource)
        {
            Contract.Requires<ArgumentNullException>(response != null);
            Contract.Requires<ArgumentNullException>(resource != null);
        }

        /// <summary>
        /// Constructor for a failed response.
        /// </summary>
        /// <param name="response">The HttpWebResponse instance.</param>
        /// <param name="exception">The exception generated.</param>
        /// <param name="resource">The response resource</param>
        public RestResponse(HttpWebResponse response, Exception exception, object resource)
            : base(response, exception)
        {
            Contract.Requires<ArgumentNullException>(response != null);
            
            if (resource != null)
            {
                Resource = (T) resource;
            }
        }
    }
}