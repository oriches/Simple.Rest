namespace Simple.Rest
{
    using System;
    using System.Net;

    public class RestResponse : IRestResponse
    {
        public HttpStatusCode StatusCode { get; private set; }

        public string StatusDescription { get; private set; }

        public Exception Exception { get; private set; }

        public CookieCollection Cookies { get; private set; }

        public WebHeaderCollection Headers { get; private set; }

        public bool Successfully { get { return StatusCode == HttpStatusCode.OK && Exception == null; } }

        public RestResponse(HttpWebResponse response) : this(response, null)
        {
        }

        public RestResponse(HttpWebResponse response, Exception exception)
        {
            StatusCode = response.StatusCode;
            StatusDescription = response.StatusDescription;
            Cookies = response.Cookies;
            Headers = response.Headers;

            Exception = exception;
        }
    }
}