namespace Simple.Rest
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Net;

    public class RestResponse<T> : RestResponse, IRestResponse<T> where T : class
    {
        public T Resource { get; private set; }

        public RestResponse(HttpWebResponse response, object resource) 
            : this(response, null, resource)
        {
            Contract.Requires<ArgumentNullException>(response != null);
            Contract.Requires<ArgumentNullException>(resource != null);
        }

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