namespace Simple.Rest.Extensions
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Net;

    public static class RestClientExtensions
    {
        public static IRestClient WithGzipEncoding(this IRestClient restClient)
        {
            Contract.Requires<ArgumentNullException>(restClient != null);

            restClient.Headers["Accept-Encoding"] = "gzip";
            restClient.Headers["Content-Encoding"] = "gzip";

            return restClient;
        }

        public static IRestClient WithDeflateEncoding(this IRestClient restClient)
        {
            Contract.Requires<ArgumentNullException>(restClient != null);

            restClient.Headers["Accept-Encoding"] = "deflate";
            restClient.Headers["Content-Encoding"] = "deflate";

            return restClient;
        }

        public static IRestClient WithCredentials(this IRestClient restClient, ICredentials credentials)
        {
            Contract.Requires<ArgumentNullException>(restClient != null);
            Contract.Requires<ArgumentNullException>(credentials != null);

            restClient.Credentials = credentials;

            return restClient;
        }

        public static IRestClient WithCookie(this IRestClient restClient, Cookie cookie)
        {
            Contract.Requires<ArgumentNullException>(restClient != null);
            Contract.Requires<ArgumentNullException>(cookie != null);

            restClient.Cookies.Add(cookie);

            return restClient;
        }

        public static IRestClient WithCookie(this IRestClient restClient, string name, string value)
        {
            Contract.Requires<ArgumentNullException>(restClient != null);
            Contract.Requires<ArgumentNullException>(name != null);
            
            restClient.Cookies.Add(new Cookie(name, value));

            return restClient;
        }
    }
}