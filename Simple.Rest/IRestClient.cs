namespace Simple.Rest
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Net;
    using System.Threading.Tasks;
    using Serializers;

    /// <summary>
    /// Interface for resource orientated RESTful interface, supports verbs GET, POST, PUT &amp; DELETE.
    /// </summary>
    [ContractClass(typeof(RestClientContract))]
    public interface IRestClient
    {
        /// <summary>
        /// Serializer used for request resource types.
        /// </summary>
        ISerializer RequestSerializer { get; }

        /// <summary>
        /// Serializer used for response resource types.
        /// </summary>
        ISerializer ResponseSerializer { get; }

        /// <summary>
        /// Cookies collection used for the HTTP request.
        /// </summary>
        CookieCollection Cookies { get; }

        /// <summary>
        /// HTTP headers collection for the HTTP request.
        /// </summary>
        WebHeaderCollection Headers { get; }

        /// <summary>
        /// Credentials used for the HTTP request.
        /// </summary>
        ICredentials Credentials { get; set; }

        /// <summary>
        /// Requests the resource asynchronuously.
        /// </summary>
        /// <typeparam name="T">The resource type</typeparam>
        /// <param name="url">The URL to GET the resource</param>
        /// <returns>Returns the resource wrapped in a Task&lt;IRestResponse&lt;T&gt;&gt;, the interface contains the resource, status code &amp; description, headers &amp; cookies.</returns>
        Task<IRestResponse<T>> GetAsync<T>(Uri url) where T : class;
        
        /// <summary>
        /// Requests the resource be stored under the supplied URL asynchronuously. If the URL refers to an already existing resource, it is modified.
        /// If the URL does not point to an existing resource, then the server can create the resource with that URL.
        /// </summary>
        /// <typeparam name="T">The resource type</typeparam>
        /// <param name="url">The URL to PUT the resource</param>
        /// <param name="resource">The resource to be PUT</param>
        /// <returns>Returns the resource wrapped in a Task&lt;IRestResponse&lt;T&gt;&gt;, the interface contains the resource, status code &amp; description, headers &amp; cookies.</returns>
        Task<IRestResponse> PutAsync<T>(Uri url, T resource) where T : class;

        /// <summary>
        /// Requests the server accept the resource asynchronuously. The resource is identified by the URL.
        /// </summary>
        /// <typeparam name="T">The resource type</typeparam>
        /// <param name="url">The URL to POST the resource</param>
        /// <param name="resource">The resource to be POST'd</param>
        /// <returns>Returns the resource wrapped in a Task&lt;IRestResponse&lt;T&gt;&gt;, the interface contains the resource, status code &amp; description, headers &amp; cookies.</returns>
        Task<IRestResponse<T>> PostAsync<T>(Uri url, T resource) where T : class;

        /// <summary>
				/// Requests the server accept the resource asynchronuously and receives a disparate return type. The resource is identified by the URL.
				/// </summary>
				/// <typeparam name="T">The resource type</typeparam>
				/// <typeparam name="R">The return type</typeparam>
				/// <param name="url">The URL to POST the resource</param>
				/// <param name="resource">The resource to be POST'd</param>
				/// <returns>Returns the return type wrapped in a Task&lt;IRestResponse&lt;&gt;&gt;, the interface contains the return, status code &amp; description, headers &amp; cookies.</returns>
        Task<IRestResponse<R>> PostAsync<T, R>(Uri url, T resource)
						where T : class
						where R : class;

				/// <summary>
				/// Deletes a resource asynchronously. The resource is identified by the URL.
				/// </summary>
				/// <param name="url">The URL to GET the resource</param>
				/// <returns>Returns the result in a Task&lt;IRestResponse&gt;, the interface contains the status code &amp; description, headers &amp; cookies.</returns>
				Task<IRestResponse> DeleteAsync(Uri url);
    }
}