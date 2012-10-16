namespace Simple.Rest
{
    using System;
    using System.Net;

    public class RestResponse<T> : RestResponse, IRestResponse<T> where T : class
    {
        public T Resource { get; private set; }

        public RestResponse(HttpWebResponse response, object resource) 
            : this(response, null, resource)
        {
        }

        public RestResponse(HttpWebResponse response, Exception exception, object resource)
            : base(response, exception)
        {
            if (resource != null)
            {
                Resource = (T) resource;
            }
        }
    }
}