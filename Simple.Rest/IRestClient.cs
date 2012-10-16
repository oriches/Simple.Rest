namespace Simple.Rest
{
    using System;
    using System.Net;
    using System.Threading.Tasks;
    using Serializers;

    public interface IRestClient
    {
        ISerializer RequestSerializer { get; set; }

        ISerializer ResponseSerializer { get; set; }

        CookieCollection Cookies { get; set; }

        WebHeaderCollection Headers { get; set; }

        ICredentials Credentials { get; set; }

        Task<IRestResponse<T>> GetAsync<T>(Uri url) where T : class;

        Task<IRestResponse> PutAsync<T>(Uri url, T resource) where T : class;

        Task<IRestResponse<T>> PostAsync<T>(Uri url, T resource) where T : class;

        Task<IRestResponse> DeleteAsync(Uri url);
    }
}