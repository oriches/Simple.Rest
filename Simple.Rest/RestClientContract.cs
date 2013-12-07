namespace Simple.Rest
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Net;
    using System.Threading.Tasks;
    using Serializers;

    [ContractClassFor(typeof(IRestClient))]
    internal abstract class RestClientContract : IRestClient
    {
        public ISerializer RequestSerializer { get; private set; }

        public ISerializer ResponseSerializer { get; private set; }

        public CookieCollection Cookies { get; private set; }

        public WebHeaderCollection Headers { get; private set; }

        public ICredentials Credentials { get; set; }

        public Task<IRestResponse<T>> GetAsync<T>(Uri url) where T : class
        {
            Contract.Requires<ArgumentNullException>(url != null);

            return null;
        }

        public Task<IRestResponse> PutAsync<T>(Uri url, T resource) where T : class
        {
            Contract.Requires<ArgumentNullException>(url != null);
            Contract.Requires<ArgumentNullException>(resource != null);

            return null;
        }

        public Task<IRestResponse<T>> PostAsync<T>(Uri url, T resource) where T : class
        {
            Contract.Requires<ArgumentNullException>(url != null);
            Contract.Requires<ArgumentNullException>(resource != null);

            return null;
        }

        public Task<IRestResponse> DeleteAsync(Uri url)
        {
            Contract.Requires<ArgumentNullException>(url != null);

            return null;
        }
    }
}