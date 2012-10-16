namespace Simple.Rest
{
    using System;
    using System.Net;

    public interface IRestResponse
    {
        HttpStatusCode StatusCode { get; }

        string StatusDescription { get; }

        Exception Exception { get; }

        CookieCollection Cookies { get; }

        WebHeaderCollection Headers { get; }

        bool Successfully { get; }
    }
}