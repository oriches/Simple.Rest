using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Ionic.Zlib;
using Simple.Rest.Common;
using Simple.Rest.Common.Serializers;

namespace Simple.Rest
{
    /// <summary>
    ///     Class for resource orientated RESTful interface, supports verbs GET, POST, PUT &amp; DELETE.
    /// </summary>
    public sealed class RestClient : BaseRestClient
    {
        /// <summary>
        ///     Creates an instance with the JSON serializer.
        /// </summary>
        public RestClient()
            : base(new JsonSerializer(), new JsonSerializer())
        {
        }

        /// <summary>
        ///     Creates an instance with a customer serializer, e.g. XML or JSON.
        /// </summary>
        /// <param name="serializer"></param>
        public RestClient(ISerializer serializer)
            : base(serializer, serializer)
        {
        }

        /// <summary>
        ///     Creates an instance with specific serializers for requests &amp; responses.
        /// </summary>
        public RestClient(ISerializer requestSerializer, ISerializer responseSerializer)
            : base(requestSerializer, responseSerializer)
        {
        }

        protected override async Task WriteToRequestStream(HttpWebRequest request, byte[] body)
        {
            using (var requestStream = new BinaryWriter(await request.GetRequestStreamAsync()))
            {
                if (ShouldCompressWithGzip(request))
                {
                    var compressedBody = GZipStream.CompressBuffer(body);
                    requestStream.Write(compressedBody, 0, compressedBody.Length);
                }
                else if (ShouldCompressWithDeflate(request))
                {
                    var compressedBody = DeflateStream.CompressBuffer(body);
                    requestStream.Write(compressedBody, 0, compressedBody.Length);
                }
                else
                {
                    requestStream.Write(body, 0, body.Length);
                }
            }
        }

        protected override RestResponse<T> ProcessResponse<T>(HttpWebResponse response)
        {
            try
            {
                if (IsGzipCompressed(response))
                    using (var stream = new GZipStream(response.GetResponseStream(), CompressionMode.Decompress))
                    {
                        var result = Deserialize<T>(stream);
                        return new RestResponse<T>(response, result);
                    }

                if (IsDeflateCompressed(response))
                    using (var stream = new DeflateStream(response.GetResponseStream(), CompressionMode.Decompress))
                    {
                        var result = Deserialize<T>(stream);
                        return new RestResponse<T>(response, result);
                    }

                using (var stream = response.GetResponseStream())
                {
                    var result = Deserialize<T>(stream);
                    return new RestResponse<T>(response, result);
                }
            }
            catch (Exception ex)
            {
                return new RestResponse<T>(response, ex, default(T));
            }
        }

        private static bool IsDeflateCompressed(WebResponse response)
        {
            var encoding = response.Headers["Content-Encoding"];
            return !string.IsNullOrEmpty(encoding) && encoding.ToLower().Contains("deflate");
        }

        private static bool ShouldCompressWithDeflate(WebRequest request)
        {
            var encoding = request.Headers["Accept-Encoding"];
            return !string.IsNullOrEmpty(encoding) && encoding.ToLower().Contains("deflate");
        }
    }
}